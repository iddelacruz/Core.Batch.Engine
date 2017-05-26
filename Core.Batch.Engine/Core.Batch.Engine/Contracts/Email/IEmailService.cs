using Core.Batch.Engine.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface IEmailService: ISignal
    {
        IEmailContent Content { get; }

        Task PrepareContentAsync(NotificationType type);
    }
}
