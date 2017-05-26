using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface IEmailHeader
    {
        string From { get; }

        string To { get; }

        string Subject { get; }
    }
}
