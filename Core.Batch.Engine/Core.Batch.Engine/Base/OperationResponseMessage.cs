using Core.Batch.Engine.Helpers;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Value type to manage the operation responses.
    /// </summary>
    public struct OperationResponseMessage
    {
        /// <summary>
        /// The operation status.
        /// </summary>
        public OperationStatus Status { get; set; }

        /// <summary>
        /// Self-rated property that verify if the operation was Ok.
        /// </summary>
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
