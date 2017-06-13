using Core.Batch.Engine.Helpers;

namespace Core.Batch.Engine.Base
{
    /// <summary>
    /// Tipo valor encargado de manejar encapsular las respuestas de las operaciones.
    /// </summary>
    public struct OperationResponseMessage
    {
        /// <summary>
        /// El estado de la operación.
        /// </summary>
        public OperationState Status { get; set; }

        /// <summary>
        /// Propiedad autocalculada que retorna el estado de la operación.
        /// </summary>
        public bool IsSuccessStatusCode
        {
            get
            {
                if(Status == OperationState.Ok)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
