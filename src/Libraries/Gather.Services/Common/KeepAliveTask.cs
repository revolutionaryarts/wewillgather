using System.Net;
using Gather.Core.Domain.Common;
using Gather.Core.Infrastructure;
using Gather.Services.Tasks;

namespace Gather.Services.Common
{
    public class KeepAliveTask : ITask
    {
        public void Execute()
        {
            var coreSettings = EngineContext.Current.Resolve<CoreSettings>();
            using (var wc = new WebClient())
            {
                wc.DownloadString(coreSettings.Domain + "keep-alive");
            }
        }
    }
}