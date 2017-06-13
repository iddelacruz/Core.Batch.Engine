namespace Core.Batch.Engine.Helpers
{
    /// <summary>
    /// Tipo de notificación a enviar.
    /// </summary>
    public enum NotificationType : byte
    {
        Ok,
        Failed
    }

    /// <summary>
    /// Estado de la operación.
    /// </summary>
    public enum OperationState : byte
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
    public enum ApplicationState :byte
    {
        Ok,
        Retry,
    }
}
