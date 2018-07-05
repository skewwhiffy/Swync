using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Swync.Core.Onedrive;

namespace Swync.Api.Controllers.Api.Onedrive
{
    [Route("api/onedrive/[controller]")]
    public class ProcessesController : Controller
    {
        [HttpGet]
        [Route("status")]
        public ProcessesStatus GetStatus()
        {
            return new ProcessesStatus(
                SingleProcessStatus.Stopped,
                OnedriveMetaSync.Singleton.Running
                    ? SingleProcessStatus.Started
                    : OnedriveMetaSync.Singleton.Exceptions.IsEmpty
                        ? SingleProcessStatus.Stopped
                        : SingleProcessStatus.Faulted
            );
        }
    }

    public class ProcessesStatus
    {
        public ProcessesStatus(SingleProcessStatus onedriveSyncStatus, SingleProcessStatus onedriveMetaSyncStatus)
        {
            OnedriveSyncStatus = onedriveSyncStatus;
            OnedriveMetaSyncStatus = onedriveMetaSyncStatus;
        }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SingleProcessStatus OnedriveSyncStatus { get; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public SingleProcessStatus OnedriveMetaSyncStatus { get; }
    }

    public enum SingleProcessStatus
    {
        Started,
        Stopped,
        Faulted
    }
}