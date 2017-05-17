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
        /// <summary>
        /// Envía la notificación.
        /// </summary>
        /// <param name="type">Tipo de notificación a enviar.</param>
        public async Task NotifyAsync(NotificationType type)
        {
            System.Diagnostics.Debug.WriteLine("Email enviado correctamente");
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
