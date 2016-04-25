using Norgate.Services.FabricETWCollector.Core;
using System;

namespace Norgate.Services.FabricETWCollector.Service
{
    public class ServiceFabricEnvironment : IEnvironment
    {
        public string ApplicationName { get; set; }
        public long InstanceId { get; set; }
        public Guid PartitionId { get; set; }
        public string ServiceTypeName { get; set; }
    }
}
