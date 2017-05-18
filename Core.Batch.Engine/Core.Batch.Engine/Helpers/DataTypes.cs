using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Helpers
{
    /// <summary>
    /// Tipo de notificación a enviar.
    /// </summary>
    public enum NotificationType : byte
    {
        Ok,
        Failed,
        Error
    }

    /// <summary>
    /// Estado de la operación.
    /// </summary>
    public enum OperationStatus : byte
    {
        NotExecuted,
        InProgress,
        Ok,
        Failed
    }

    /// <summary>
    /// Estado de la sesión.
    /// </summary>
    public enum SessionState : byte
    {
        Initial,
        InProgress,
        Completed,
        Uncompleted
    }

    /// <summary>
    /// Estado de la Aplicación
    /// </summary>
    public enum ApplicationStatus :byte
    {
        Ok,
        Retry,
    }
}
