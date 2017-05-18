using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Batch.Engine.Contracts
{
    public interface IApplication : IDisposable
    {
        string Message { get; }

        IAppSession Session { get; }

        Task ExecuteAsync();

        Task ResumeAsync();
    }
}
