using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Class responsible for handling the operations contained in each session.
    /// </summary>
    public sealed class Application : IApplication
    {
        public IAppSession Session { get; private set; }

        INotification notification;

        /// <summary>
        /// Create a new instance of <see cref="Application"/>
        /// </summary>
        /// <param name="session">Session injected by dependency.</param>
        /// <param name="notification">The type of notification needed.</param>
        public Application(IAppSession session, INotification notification)
        {
            if(session != null)
            {
                Session = session;
                Session.App = this;
            }
            else
            {
                throw new ArgumentNullException("IAppSession");
            }

            if(notification != null)
            {
                this.notification = notification;
            }
            else
            {
                throw new ArgumentNullException("INotification");
            }
        }

        /// <summary>
        /// It executes each one of the operations of the session.
        /// </summary>
        public async Task ExecuteAsync()
        {
            if(Session != null)
            {
                Session.State = SessionState.InProgress;
                do
                {
                    var operation = Session.OperationsRemaining.Dequeue();
                    var exec = await ExecuteAsync(operation);
                    if (exec == -1)
                    {
                        return;
                    };
                }
                while (Session.OperationsRemaining.Count > 0);
                await CloseSessionAndNotify(SessionState.Completed, NotificationType.Ok);
            }
            else
            {
                throw new NullReferenceException("Session object can not be null.");
            }
        }

        /// <summary>
        /// This method is invoked to retrieve a session that has not completed successfully.
        /// </summary>
        public async Task ResumeAsync()
        {
            Session = null;
            var recovered = await AppSession.RecoverAsync();
            if(recovered != null)
            {
                Session = recovered;

                if ((Session.OperationsResult != null) && (Session.OperationsRemaining != null))
                {
                    var operation = Session
                        .OperationsResult
                        .Where(x => x.Status == OperationStatus.Failed)
                        .SingleOrDefault();

                    if (operation != null)
                    {
                        var result = await ExecuteAsync(operation);
                        if (result == 1)
                        {
                            if (Session.OperationsRemaining.Count > 0)
                            {
                                await ExecuteAsync();
                            }
                            else
                            {
                                await CloseSessionAndNotify(SessionState.Completed, NotificationType.Ok);
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("Exception: No se encontrado la operación fallida.");
                    }
                }
            }
            else
            {
                throw new Exception("Exception: Estás intentando recuperar una sesión que no existe.");
            }
        }

        /// <summary>
        /// Help method that is responsible for receiving the operation and executing it.
        /// </summary>
        /// <param name="operation">Operation to be executed.</param>
        async Task<int> ExecuteAsync(IOperation operation)
        {
            var response = await operation.SendAsync();
            return await ValidateAsync(operation, response);
        }

        /// <summary>
        /// Help method that verifies the results of operations.
        /// </summary>
        /// <param name="operation">Operation executed.</param>
        /// <param name="response">Response received.</param>
        /// <remarks>
        ///  1 if the operation status is correct.
        /// -1 if the status of the operation is incorrect.
        /// </remarks>
        async Task<int> ValidateAsync(IOperation operation, OperationResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                //Esto hay que moverlo a la clase Operation si es necesario.
                operation.Status = OperationStatus.Ok;
                await Session.StoreAsync(operation);
                return 1;
            }
            else
            {
                if (await TryOperationAsync(operation))
                {
                    return 1;
                }
                //Esto hay que moverlo a la clase Operation si es necesario.
                operation.Status = OperationStatus.Failed;
                await Session.StoreAsync(operation);
                await CloseSessionAndNotify(SessionState.Uncompleted, NotificationType.Failed);
                return -1;
            }
        }

        /// <summary>
        /// Help method that is responsible for doing the retries when an operation is incorrect.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns>True if the operation was executed successfully. False if not.</returns>
        async Task<bool> TryOperationAsync(IOperation operation = null)
        {
            if(operation != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var result = await operation.SendAsync();
                    if (result.IsSuccessStatusCode)
                    {
                        await ValidateAsync(operation, result);
                        return true;
                    }
                    Thread.Sleep(1000);
                }
            }
            return false;
        }

        /// <summary>
        /// Help method to chage session state, persist data and notify.
        /// </summary>
        /// <param name="sessionState">The current session state.</param>
        /// <param name="notificationType">The current notification type.</param>
        async Task CloseSessionAndNotify(SessionState sessionState, NotificationType notificationType)
        {
            Session.State = sessionState;
            await Session.FlushAsync();
            await notification.NotifyAsync(notificationType);
        }

        #region Dispose pattern
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Session != null)
                {
                    Session.Dispose();
                }
            }
        }
        #endregion
    }
}
