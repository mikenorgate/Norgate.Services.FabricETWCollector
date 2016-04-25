using Metrics.Reporters;
using System;
using Metrics;
using Metrics.MetricData;
using System.Reactive.Subjects;
using Norgate.Services.FabricETWCollector.Core;
using System.Collections.Generic;

namespace Norgate.Services.FabricETWCollector.Service
{
    class ObservableMetricReport : BaseReport
    {
        private Subject<MetricValue> _metricStream;
        public IObservable<MetricValue> MetricSream
        {
            get
            {
                return _metricStream;
            }
        }

        public ObservableMetricReport()
        {
            _metricStream = new Subject<MetricValue>();
        }

        protected override string FormatContextName(IEnumerable<string> contextStack, string contextName)
        {
            return contextName;
        }

        protected override string FormatMetricName<T>(string context, MetricValueSource<T> metric)
        {
            return context + "|" + metric.Name;
        }

        protected override void ReportCounter(string name, CounterValue value, Unit unit, MetricTags tags)
        {
            var nameSplit = name.Split('|');
            var metricValue = new MetricValue()
            {
                Name = nameSplit[1],
                Context = nameSplit[0],
                Tags = tags.Tags,
                Type = "Counter",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("Count", value.Count);
            foreach (var item in value.Items)
            {
                metricValue.Properties.Add(item.Item + "_Count", item.Count);
                metricValue.Properties.Add(item.Item + "_Percent", item.Percent);
            }
            metricValue.Properties.Add("Unit", unit.Name);
            _metricStream.OnNext(metricValue);
        }

        protected override void ReportGauge(string name, double value, Unit unit, MetricTags tags)
        {
            var nameSplit = name.Split('|');
            var metricValue = new MetricValue()
            {
                Name = nameSplit[1],
                Context = nameSplit[0],
                Tags = tags.Tags,
                Type = "Gauge",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("Value", value);
            metricValue.Properties.Add("Unit", unit.Name);
            _metricStream.OnNext(metricValue);
        }

        protected override void ReportHealth(HealthStatus status)
        {
            var metricValue = new MetricValue()
            {
                Name = "Health",
                Type = "Health",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("IsHealthy", status.IsHealthy);
            foreach (var result in status.Results)
            {
                metricValue.Properties.Add(result.Name + "_IsHealthy", result.Check.IsHealthy);
            }
            _metricStream.OnNext(metricValue);
        }

        protected override void ReportHistogram(string name, HistogramValue value, Unit unit, MetricTags tags)
        {
            var nameSplit = name.Split('|');
            var metricValue = new MetricValue()
            {
                Name = nameSplit[1],
                Context = nameSplit[0],
                Tags = tags.Tags,
                Type = "Histogram",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("Count", value.Count);
            metricValue.Properties.Add("LastUserValue", value.LastUserValue);
            metricValue.Properties.Add("LastValue", value.LastValue);
            metricValue.Properties.Add("Max", value.Max);
            metricValue.Properties.Add("MaxUserValue", value.MaxUserValue);
            metricValue.Properties.Add("Mean", value.Mean);
            metricValue.Properties.Add("Median", value.Median);
            metricValue.Properties.Add("Min", value.Min);
            metricValue.Properties.Add("MinUserValue", value.MinUserValue);
            metricValue.Properties.Add("Percentile75", value.Percentile75);
            metricValue.Properties.Add("Percentile95", value.Percentile95);
            metricValue.Properties.Add("Percentile98", value.Percentile98);
            metricValue.Properties.Add("Percentile99", value.Percentile99);
            metricValue.Properties.Add("Percentile999", value.Percentile999);
            metricValue.Properties.Add("SampleSize", value.SampleSize);
            metricValue.Properties.Add("StdDev", value.StdDev);
            metricValue.Properties.Add("Unit", unit.Name);
            _metricStream.OnNext(metricValue);
        }

        protected override void ReportMeter(string name, MeterValue value, Unit unit, TimeUnit rateUnit, MetricTags tags)
        {
            var nameSplit = name.Split('|');
            var metricValue = new MetricValue()
            {
                Name = nameSplit[1],
                Context = nameSplit[0],
                Tags = tags.Tags,
                Type = "Meter",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("Count", value.Count);
            metricValue.Properties.Add("FifteenMinuteRate", value.FifteenMinuteRate);
            metricValue.Properties.Add("FiveMinuteRate", value.FiveMinuteRate);
            metricValue.Properties.Add("MeanRate", value.MeanRate);
            metricValue.Properties.Add("OneMinuteRate", value.OneMinuteRate);
            metricValue.Properties.Add("RateUnit", value.RateUnit);
            foreach (var item in value.Items)
            {
                metricValue.Properties.Add(item.Item + "_Value", item.Value);
                metricValue.Properties.Add(item.Item + "_Percent", item.Percent);
            }
            metricValue.Properties.Add("Unit", unit.Name);
            _metricStream.OnNext(metricValue);
        }

        protected override void ReportTimer(string name, TimerValue value, Unit unit, TimeUnit rateUnit, TimeUnit durationUnit, MetricTags tags)
        {
            var nameSplit = name.Split('|');
            var metricValue = new MetricValue()
            {
                Name = nameSplit[1],
                Context = nameSplit[0],
                Tags = tags.Tags,
                Type = "Timer",
                Timestamp = DateTime.UtcNow
            };
            metricValue.Properties.Add("ActiveSessions", value.ActiveSessions);
            metricValue.Properties.Add("Histogram_Count", value.Histogram.Count);
            metricValue.Properties.Add("Histogram_LastUserValue", value.Histogram.LastUserValue);
            metricValue.Properties.Add("Histogram_LastValue", value.Histogram.LastValue);
            metricValue.Properties.Add("Histogram_Max", value.Histogram.Max);
            metricValue.Properties.Add("Histogram_MaxUserValue", value.Histogram.MaxUserValue);
            metricValue.Properties.Add("Histogram_Mean", value.Histogram.Mean);
            metricValue.Properties.Add("Histogram_Median", value.Histogram.Median);
            metricValue.Properties.Add("Histogram_Min", value.Histogram.Min);
            metricValue.Properties.Add("Histogram_MinUserValue", value.Histogram.MinUserValue);
            metricValue.Properties.Add("Histogram_Percentile75", value.Histogram.Percentile75);
            metricValue.Properties.Add("Histogram_Percentile95", value.Histogram.Percentile95);
            metricValue.Properties.Add("Histogram_Percentile98", value.Histogram.Percentile98);
            metricValue.Properties.Add("Histogram_Percentile99", value.Histogram.Percentile99);
            metricValue.Properties.Add("Histogram_Percentile999", value.Histogram.Percentile999);
            metricValue.Properties.Add("Histogram_SampleSize", value.Histogram.SampleSize);
            metricValue.Properties.Add("Histogram_StdDev", value.Histogram.StdDev);
            metricValue.Properties.Add("Rate_Count", value.Rate.Count);
            metricValue.Properties.Add("Rate_FifteenMinuteRate", value.Rate.FifteenMinuteRate);
            metricValue.Properties.Add("Rate_FiveMinuteRate", value.Rate.FiveMinuteRate);
            metricValue.Properties.Add("Rate_MeanRate", value.Rate.MeanRate);
            metricValue.Properties.Add("Rate_OneMinuteRate", value.Rate.OneMinuteRate);
            metricValue.Properties.Add("Rate_RateUnit", value.Rate.RateUnit);
            metricValue.Properties.Add("Unit", unit.Name);
            metricValue.Properties.Add("DurationUnit", durationUnit.ToString());
            _metricStream.OnNext(metricValue);
        }
    }    
}
