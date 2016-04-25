using Microsoft.Diagnostics.Tracing;
using System;

namespace Norgate.Services.FabricETWCollector.Core
{
    public interface IDrain : IObserver<TraceEvent>, IObserver<MetricValue>
    {
    }
}
