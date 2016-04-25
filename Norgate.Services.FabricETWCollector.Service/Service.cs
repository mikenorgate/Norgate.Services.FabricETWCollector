using System;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Diagnostics.Tracing.Session;
using System.Reflection;
using Microsoft.Diagnostics.Tracing;
using Metrics;
using Norgate.Services.FabricETWCollector.Drains.InfluxDb;
using Norgate.Services.FabricETWCollector.Core;
using System.IO;
using Newtonsoft.Json;
using System.Reactive.Subjects;
using Metrics.Json;

namespace Norgate.Services.FabricETWCollector.Service
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class Service : StatelessService
    {
        private IObservable<TraceEvent> _eventStream;
        private IObservable<MetricValue> _metricStream;
        private IEnvironment _environment;

        public Service(StatelessServiceContext context)
            : base(context)
        {
            var environment = new ServiceFabricEnvironment();
            environment.InstanceId = context.InstanceId;
            environment.PartitionId = context.PartitionId;
            environment.ServiceTypeName = context.ServiceTypeName;
            environment.ApplicationName = context.CodePackageActivationContext.ApplicationName;
            _environment = environment;
        }

        protected async override Task RunAsync(CancellationToken cancellationToken)
        {
            if (TraceEventSession.IsElevated() != true)
            {
                return;
            }

            var configPath = Path.Combine(Context.CodePackageActivationContext.GetConfigurationPackageObject("Config").Path, "Config.json");
            var settings = JsonConvert.DeserializeObject<ServiceConfig>(File.ReadAllText(configPath));

            using (var session = new TraceEventSession(Assembly.GetEntryAssembly().FullName))
            {
                _eventStream = session.Source.Dynamic.Observe(null);

                EnableConfiguredPerfCounters(settings.PerfCounters);
                EnabledConfiguredProviders(session, settings.EventProviders);

                RegisterDrains(settings.Drains);

                Task.Factory.StartNew(() => { session.Source.Process(); });
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
                }
                session.Stop();
            }
        }

        private void RegisterDrains(DrainConfig config)
        {
            if (config.InfluxDb != null)
            {
                var influxDrain = new InfluxDbDrain(config.InfluxDb, _environment);
                _eventStream.Subscribe(influxDrain);
                _metricStream.Subscribe(influxDrain);
            }
        }

        private void EnabledConfiguredProviders(TraceEventSession session, EventProviderConfig[] providers)
        {
            foreach (var provider in providers)
            {
                session.EnableProvider(provider.Name, provider.Level);
            }
        }

        private void EnableConfiguredPerfCounters(PerfCounterConfig config)
        {
            foreach (var counter in config.PerfCounters)
            {
                Metric.PerformanceCounter(counter.Name, counter.CounterCategory, counter.CounterName, counter.CounterInstance, counter.Unit);
            }

            Metric.Config
                .WithSystemCounters()
                .WithReporting(r =>
                {
                    var report = new ObservableMetricReport();
                    r.WithReport(report, config.CollectionInterval);
                    _metricStream = report.MetricSream;
                })
                .WithJsonDeserialzier(j=>JsonConvert.DeserializeObject<JsonMetricsContext>(j));

            if (config.RemoteMetrics != null)
            {
                foreach (var remote in config.RemoteMetrics)
                {
                    Metric.Config.RegisterRemote(remote.Name, new Uri(remote.Url), remote.CollectionInterval);
                }
            }
        }
    }
}
