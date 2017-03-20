using Core.Batch.Engine.Helpers;

namespace Core.Batch.Engine.Base
{
    public struct OperationResponseMessage
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
    }
}
