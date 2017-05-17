using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using System;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    public sealed class EmailNotification : INotification
    {
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
