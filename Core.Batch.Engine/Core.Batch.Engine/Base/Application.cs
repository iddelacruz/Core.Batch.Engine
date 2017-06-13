using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using Toyota.Email.Library.Contracts;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Clase responsable de manejar las operaciones que se encuentran en casa sesión.
    /// </summary>
    public sealed class Application
    {
        IEmailSender sender;

        /// <summary>
        /// Mensaje que irá asociado al correo electrónico enviado
        /// y a las distintas respuestas que retornarán las operaciones.
        /// </summary>
        public string Message { get; private set; } = string.Empty;

        /// <summary>
        /// Determina el estado completo de la aplicación.
        /// </summary>
        public ApplicationState AppStatus { get; private set; }

        /// <summary>
        /// Sesión asociada.
        /// </summary>
        public AppSession Session { get; private set; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="Application"/>
        /// </summary>
        /// <param name="session">Sesión inyectada por dependencia.</param>
        /// <param name="sender">El tipo de notificación que utilizará.</param>
        public Application(AppSession session, IEmailSender sender)
        {
            if (session == null)
                throw new ArgumentNullException("AppSession");
            if (sender == null)
                throw new ArgumentNullException("IEmailSender");
            Session = session;
            Session.App = this;
            this.sender = sender;
        }

        /// <summary>
        /// Ejecuta cada una de las operaciones de la sesión.
        /// </summary>
        public async Task ExecuteAsync()
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

        /// <summary>
        /// Recupera la sesión que no ha completado correctamente y lanza la ejecución 
        /// de las operaciones que han fallado <see cref="OperationState.Failed"/>
        /// y las que aun no se han ejecutado <see cref="OperationState.NotExecuted"/>.
        /// </summary>
        public async Task ResumeAsync()
        {
            Session = null;
            var recovered = await AppSession.RecoverAsync();

            if(recovered != null)
            {
                Session = recovered;
                Session.App = this;

                if ((Session.OperationsResult != null) && (Session.OperationsRemaining != null))
                {
                    var operation = Session
                        .OperationsResult
                        .Where(x => x.Status == OperationState.Failed)
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
                        //TODO: LOG
#if DEBUG
                        System.Diagnostics.Debug.WriteLine(" No se encontrado la operación fallida.");
#endif
                        Message = "No se encontrado la operación fallida.";
                        AppStatus = ApplicationState.Retry;
                    }
                }
                else
                {
                    //TODO: LOG
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("No se han encontrado operaciones para ejecutar.");
#endif
                    Message = "No se han encontrado operaciones para ejecutar.";
                    AppStatus = ApplicationState.Retry;
                }
            }
            else
            {
                //TODO: LOG
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Estás intentando recuperar una sesión que no existe.");
#endif
                Message = "Estás intentando recuperar una sesión que no existe.";
                AppStatus = ApplicationState.Retry;
            }
        }

        /// <summary>
        /// Recibe una operación y la ejecuta.
        /// </summary>
        /// <param name="operation">Operación que será ejecutada.</param>
        async Task<int> ExecuteAsync(IOperation operation)
        {
            try
            {
                var response = await operation.SendAsync();
                return await ValidateAsync(operation, response);
            }
            catch (Exception)
            {
                //TODO: Aquí hay que hacer log de las posibles excepciones.
                throw;
            }

        }

        /// <summary>
        /// Verifica los resultados de las operaciones.
        /// </summary>
        /// <param name="operation">Operación ejecutada.</param>
        /// <param name="response">Respuesta recibida al ejecutar la operación.</param>
        /// <remarks>
        ///  1 Si el estado de la operación es correcto.
        /// -1 Si el estado de la operación es incorrecto.
        /// </remarks>
        async Task<int> ValidateAsync(IOperation operation, OperationResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                operation.Status = OperationState.Ok;
                await Session.StoreAsync(operation);
                return 1;
            }
            else
            {
                if (await TryOperationAsync(operation))
                {
                    return 1;
                }
                else
                {
                    operation.Status = OperationState.Failed;
                    await Session.StoreAsync(operation);
                    await CloseSessionAndNotify(SessionState.Uncompleted, NotificationType.Failed);
                    return -1;
                }
            }
        }

        /// <summary>
        /// Realiza los reintentos necesarios cuando el estado de la operación no es <see cref="OperationState.Ok"/>
        /// </summary>
        /// <param name="operation">La operación que se intentará ejecutar.</param>
        /// <returns>Verdadero si la operación se ejecuta correctamente, falso si no.</returns>
        async Task<bool> TryOperationAsync(IOperation operation = null)
        {
            if(operation != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var result = await operation.SendAsync();
                        if (result.IsSuccessStatusCode)
                        {
                            await ValidateAsync(operation, result);
                            return true;
                        }
                        await Task.Delay(1000);
                    }
                    catch (Exception)
                    {
                        //TODO: LOG.
                        throw;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Se encarga de cambiar el estado de la sesión, persistir los datos de la sesión 
        /// y enviar un anotificación sobre el estado del sistema.
        /// </summary>
        /// <param name="sessionState">El estado actual de la sesión.</param>
        /// <param name="notificationType">Determina el tipo de notificación a enviar.</param>
        async Task CloseSessionAndNotify(SessionState sessionState, NotificationType notificationType)
        {
            if (sessionState == SessionState.Uncompleted)
            {
                AppStatus = ApplicationState.Retry;
            }
            else
            {
                AppStatus = ApplicationState.Ok;
            }

            Session.State = sessionState;
            await Session.FlushAsync();

            switch (notificationType)
            {
                case NotificationType.Ok:
                    await sender.SendSuccessEmailAsync();
                    break;
                case NotificationType.Failed:
                    await sender.SendErrorEmailAsync();
                    break;
            }
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
