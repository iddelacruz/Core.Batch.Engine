using Core.Batch.Engine.Helpers;
using System;

namespace Core.Batch.Engine.Base
{
    public class OperationResponseMessage:IDisposable
    {
        public OperationStatus Status { get; set; }

        public bool IsSuccessStatusCode
        {
            get
            {
                if(Status == OperationStatus.Ok)
                {
                    return true;
                }
                return false;
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

    }
}
