using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Batch.Engine.Base;
using System.Threading.Tasks;
using Core.Batch.Engine.Test.Operations;

namespace Core.Batch.Engine.Test
{
    [TestClass]
    public class ApplicationTest
    {
        [TestMethod]
        public async Task Main()
        {
            //Operaciones;
            var masterOp = new LoadMasterData();
            var loadModelOp = new LoadModels();
            var loadSpecOp = new LoadSpecifications();

            var session = new AppSession();
            await session.RegisterOperationAsync(masterOp);
            await session.RegisterOperationAsync(loadModelOp);
            await session.RegisterOperationAsync(loadSpecOp);
            var notification = new EmailNotification(null);
            var app = new Application(session, notification);
            await app.ExecuteAsync();
            if(app.AppStatus == Helpers.ApplicationStatus.Retry)
            {
                await app.ResumeAsync();
            }
        }
    }
}
