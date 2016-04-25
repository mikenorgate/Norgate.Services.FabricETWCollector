using Microsoft.Diagnostics.Tracing;
using Norgate.Services.FabricETWCollector.Core;
using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Vibrant.InfluxDB.Client;

namespace Norgate.Services.FabricETWCollector.Drains.InfluxDb
{
    public class InfluxDbDrain : IDrain
    {
        private InfluxClient _client;
        private Subject<InfluxRow> _buffer;
        private InfluxDbDrainConfig _config;
        private IEnvironment _environment;

        public InfluxDbDrain(InfluxDbDrainConfig config, IEnvironment environment)
        {
            _config = config;
            _environment = environment;

            _client = new InfluxClient(new Uri(_config.Url));
            _buffer = new Subject<InfluxRow>();
            _buffer = new Subject<InfluxRow>();
            Observable.Buffer(_buffer, _config.MaxBufferTime, _config.MaxBufferSize).Subscribe(async r => await WriteRows(r));
        }

        public void OnCompleted()
        {
        }

        public void OnError(Exception error)
        {
            ServiceEventSource.Current.Error(error.ToString());
        }

        public void OnNext(MetricValue value)
        {
            var tags = string.Join(",", value.Tags);

            var row = new InfluxRow();
            row.MeasurementName = string.IsNullOrEmpty(value.Context) ? value.Name : value.Context + "_" + value.Context;
            row.Timestamp = value.Timestamp;
            if (!string.IsNullOrEmpty(tags))
                row.SetTag("Tags", tags);

            row.SetTag("Name", value.Name);
            row.SetTag("Type", value.Type);
            row.SetTag("ApplicationName", _environment.ApplicationName);
            row.SetTag("InstanceId", _environment.InstanceId.ToString());
            row.SetTag("PartitionId", _environment.PartitionId.ToString());
            row.SetTag("ServiceTypeName", _environment.ServiceTypeName);
            foreach (var prop in value.Properties)
            {
                var payloadValue = prop.Value;
                if (prop.Value.GetType().IsValueType)
                {
                    payloadValue = payloadValue.ToString();
                }
                row.SetField(prop.Key, payloadValue);
            }
            _buffer.OnNext(row);
        }

        public void OnNext(TraceEvent value)
        {
            if (value.PayloadNames.Length == 0)
                return;

            var row = new InfluxRow();
            row.MeasurementName = value.ProviderName;
            row.Timestamp = value.TimeStamp;
            row.SetTag("ActivityID", value.ActivityID.ToString());
            row.SetTag("EventName", value.EventName);
            row.SetTag("ID", value.ID.ToString());
            row.SetTag("Level", value.Level.ToString());
            if (!string.IsNullOrEmpty(value.ProcessName))
                row.SetTag("ProcessName", value.ProcessName);
            row.SetTag("ProviderName", value.ProviderName);
            row.SetTag("ApplicationName", _environment.ApplicationName);
            row.SetTag("InstanceId", _environment.InstanceId.ToString());
            row.SetTag("PartitionId", _environment.PartitionId.ToString());
            row.SetTag("ServiceTypeName", _environment.ServiceTypeName);
            for (var i = 0; i < value.PayloadNames.Length; i++)
            {
                try
                {
                    var payloadValue = value.PayloadValue(i);
                    if (payloadValue.GetType().IsValueType)
                    {
                        payloadValue = payloadValue.ToString();
                    }
                    row.SetField(value.PayloadNames[i], payloadValue);
                }
                catch
                {
                    //Skip if unable to read payload value
                }
            }
            _buffer.OnNext(row);
        }

        private async Task WriteRows(IList<InfluxRow> rows)
        {
            if (rows.Count == 0)
                return;

            try
            {
                await _client.WriteAsync(_config.DatabaseName, rows);
            }
            catch (Exception ex)
            {
                ServiceEventSource.Current.Error(ex.ToString());
            }
        }
    }
}
