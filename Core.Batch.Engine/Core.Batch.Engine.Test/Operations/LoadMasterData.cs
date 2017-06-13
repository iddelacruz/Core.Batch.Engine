using Core.Batch.Engine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Test.Operations
{
    public class LoadMasterData : Operation
    {
        public async override Task<OperationResponseMessage> SendAsync()
        {
            return new OperationResponseMessage
            {
                Status = Helpers.OperationState.Ok
            };
        }
    }
}
