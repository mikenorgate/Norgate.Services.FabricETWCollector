using System;

namespace Norgate.Services.FabricETWCollector.Drains.InfluxDb
{
    public class InfluxDbDrainConfig
    {
        public string Url { get; set; }
        public string DatabaseName { get; set; }
        public TimeSpan MaxBufferTime { get; set; }
        public int MaxBufferSize { get; set; }
    }
}
