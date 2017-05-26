using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using System;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Clase encargada de enviar notificaciones mediante correo electrónico.
    /// </summary>
    public sealed class EmailNotification : INotification
    {
        IEmailService Service { get; set; }

        /// <summary>
        /// Crea una nueva instancia de <see cref="EmailNotification"/>
        /// </summary>
        /// <param name="service">
        /// Servicio que se encargará de preparar la configuración 
        /// y el envío del correo electrónico.
        /// </param>
        public EmailNotification(IEmailService service)
        {
            if (service == null)
            {
                throw new ArgumentNullException("IEmailService");
            }
            Service = service;
        }

        /// <summary>
        /// Envía la notificación por correo electrónico.
        /// </summary>
        /// <param name="type">Tipo de notificación a enviar.</param>
        public async Task NotifyAsync(NotificationType type)
        {
            await Service.SendAsync();
            System.Diagnostics.Debug.WriteLine("Email enviado correctamente");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
