using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Helpers
{
    public enum NotificationType : byte
    {
        Ok,
        Failed
    }

    public enum OperationStatus : byte
    {
        NotExecuted,
        InProgress,
        Ok,
        Failed
    }

    public enum SessionState : byte
    {
        Initial,
        InProgress,
        Completed,
        Uncompleted
    }
}
