using Metrics;
using Microsoft.Diagnostics.Tracing;
using Norgate.Services.FabricETWCollector.Drains.InfluxDb;
using System;

namespace Norgate.Services.FabricETWCollector.Service
{
    public class ServiceConfig
    {
        public EventProviderConfig[] EventProviders { get; set; }
        public PerfCounterConfig PerfCounters { get; set; }
        public DrainConfig Drains { get; set; }
    }

    public class EventProviderConfig
    {
        public string Name { get; set; }
        public TraceEventLevel Level { get; set; }
    }

    public class PerfCounterConfig
    {
        public class PerfCounter
        {
            public string Name { get; set; }
            public string CounterCategory { get; set; }
            public string CounterName { get; set; }
            public string CounterInstance { get; set; }
            public Unit Unit { get; set; }
        }

        public class RemoteMetric
        {
            public string Name { get; set; }
            public string Url { get; set; }
            public TimeSpan CollectionInterval { get; set; }
        }

        public TimeSpan CollectionInterval { get; set; }
        public PerfCounter[] PerfCounters { get; set; }
        public RemoteMetric[] RemoteMetrics { get; set; }
    }

    public class DrainConfig
    {
        public InfluxDbDrainConfig InfluxDb { get; set; }
    }
}
