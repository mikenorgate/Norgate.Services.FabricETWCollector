using System;
using System.Collections.Generic;

namespace Norgate.Services.FabricETWCollector.Core
{
    public class MetricValue
    {
        public MetricValue()
        {
            Properties = new Dictionary<string, object>();
        }

        public string Name { get; set; }
        public string Context { get; set; }
        public Dictionary<string, object> Properties { get; set; }
        public string[] Tags { get; set; }
        public string Type { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
