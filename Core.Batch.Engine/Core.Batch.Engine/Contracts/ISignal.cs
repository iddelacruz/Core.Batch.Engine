using Core.Batch.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface ISignal
    {
        Task<OperationResponseMessage> SendAsync();
    }
}
