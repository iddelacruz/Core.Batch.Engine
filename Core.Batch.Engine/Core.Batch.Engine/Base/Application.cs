using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Clase encargada de manejar las sesiones y las operaciones contenidas en cada sesión.
    /// </summary>
    public sealed class Application : IApplication
    {
        public IAppSession Session { get; private set; }

        INotification notification;

        const string connectionString = "Banense.Pattern.Architecture.SessionStorage";

        /// <summary>
        /// Crea una nueva instancia de <see cref="Application"/>
        /// </summary>
        /// <param name="session">Sesión inyectada por dependencia.</param>
        /// <param name="notification">El tipo de notificación que se necesite.</param>
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
        /// Ejecuta cada una de las operaciones de la sesión.
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
                await CloseSessionAsOkAsync();
            }
            else
            {
                throw new NullReferenceException("Session object can not be null.");
            }
        }

        /// <summary>
        /// Este método es invocado para recuperar una sesión que no ha finalizado correctamente.
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
                                await CloseSessionAsOkAsync();
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
        /// Método de ayuda que se encarga de recibir la operación y de ejecutarla.
        /// </summary>
        /// <param name="operation">Operación a ejecutar.</param>
        async Task<int> ExecuteAsync(IOperation operation)
        {
            var response = await operation.SendAsync();
            return await ValidateAsync(operation, response);
        }

        /// <summary>
        /// Método de ayuda que se encarga de verificar el resultado de las operaciones.
        /// </summary>
        /// <param name="operation">Operación ejecutada.</param>
        /// <param name="response">Respuesta recibida.</param>
        /// <remarks>
        ///  1 si el estado de la operación es correcta.
        /// -1 si el estado de la operación es incorrecta.
        /// </remarks>
        async Task<int> ValidateAsync(IOperation operation, OperationResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                operation.Status = OperationStatus.Ok;//Esto hay que moverlo a la clase Operation si es necesario.
                await Session.StoreAsync(operation);
                return 1;
            }
            else
            {
                if (await TryOperationAsync(operation))
                {
                    return 1;
                }
                operation.Status = OperationStatus.Failed;//Esto hay que moverlo a la clase Operation si es necesario.
                await Session.StoreAsync(operation);
                await CloseSessionAsFailedAsync();
                return -1;
            }
        }

        /// <summary>
        /// Método de ayuda que se encarga hacer los reintentos
        /// cuando una operación es incorrecta.
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
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
        /// Método de ayuda para cambiar el estado de la sesión a <see cref="SessionState.Completed"/>
        /// Almacenar la sesión en el almacenamiento local 
        /// y enviar una notificación (correo electrónico) con estado <see cref="NotificationType.Ok"/>
        /// </summary>
        async Task CloseSessionAsOkAsync()
        {
            Session.State = SessionState.Completed;
            await Session.FlushAsync();
            await notification.NotifyAsync(NotificationType.Ok);
        }

        /// <summary>
        /// Método de ayuda para cambiar el estado de la sesión a <see cref="SessionState.Uncompleted"/>
        /// Almacenar la sesión en el almacenamiento local 
        /// y enviar una notificación (correo electrónico) con estado <see cref="NotificationType.Failed"/>
        /// </summary>
        async Task CloseSessionAsFailedAsync()
        {
            Session.State = SessionState.Uncompleted;
            await Session.FlushAsync();
            await notification.NotifyAsync(NotificationType.Failed);
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
