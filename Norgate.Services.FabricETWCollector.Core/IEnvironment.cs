using System;

namespace Norgate.Services.FabricETWCollector.Core
{
    public interface IEnvironment
    {
        string ApplicationName { get; }
        long InstanceId { get; }
        Guid PartitionId { get; }
        string ServiceTypeName { get; }
    }
}
