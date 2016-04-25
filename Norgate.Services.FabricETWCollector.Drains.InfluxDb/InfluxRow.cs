using Vibrant.InfluxDB.Client.Rows;

namespace Norgate.Services.FabricETWCollector.Drains.InfluxDb
{
    public class InfluxRow : DynamicInfluxRow, IHaveMeasurementName
    {
        public string MeasurementName { get; set; }
    }
}
