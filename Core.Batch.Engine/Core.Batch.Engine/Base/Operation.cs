using Core.Batch.Engine.Contracts;
using Core.Batch.Engine.Helpers;
using System;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Base
{
    public abstract class Operation : IOperation
    {
        public Guid OperationID { get; private set; }

        public OperationStatus Status { get; set; } = OperationStatus.NotExecuted;

        public Operation()
        {
            OperationID = Guid.NewGuid();
        }

        public abstract Task<OperationResponseMessage> SendAsync();
    }
}
