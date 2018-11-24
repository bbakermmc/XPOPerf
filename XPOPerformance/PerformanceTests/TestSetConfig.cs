using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;

namespace ORMBenchmark.PerformanceTests {
    public class TestSetConfig : ManualConfig
    {

        public static long[] RowCounts = new long[] {50,200};//50, 100};//10, 50, 100, 250, 500, 1000, 2500, 5000 };

        public TestSetConfig() {
            var job = Job.Clr
                    .With(Runtime.Clr)
                    .WithLaunchCount(1)
                    .WithWarmupCount(1)
                    .WithMinInvokeCount(1)
                    .WithInvocationCount(1)
                    .WithMaxRelativeError(0.1)
                    .WithUnrollFactor(1);
            job.Run.RunStrategy = BenchmarkDotNet.Engines.RunStrategy.Throughput;
            
            Add(job);
            Set(new TestSetOrderProvider());
            Add(JitOptimizationsValidator.DontFailOnError);
            Add(DefaultConfig.Instance.GetLoggers().ToArray());
            Add(DefaultConfig.Instance.GetExporters().ToArray());
            Add(DefaultConfig.Instance.GetColumnProviders().ToArray());
          //  Set(new FastestToSlowestOrderer());
          //  Set(new DefaultOrderer(SummaryOrderPolicy.Default));
        }

        private class TestSetOrderProvider : IOrderer {

            public bool SeparateLogicalGroups {
                get {
                    return true;
                }
            }

            public IEnumerable<BenchmarkCase> GetExecutionOrder(BenchmarkCase[] benchmarksCase) {
                return benchmarksCase
                       .OrderBy(t => t.Descriptor.WorkloadMethodDisplayInfo.ToString())
                       .ThenBy(t => t.Parameters["RowCount"]);

            }

            public IEnumerable<BenchmarkCase> GetSummaryOrder(BenchmarkCase[] benchmarksCase, Summary summary) =>
                from benchmark in benchmarksCase
                orderby benchmark.Descriptor.WorkloadMethod,
                    benchmark.Parameters["RowCount"],
                    summary[benchmark].ResultStatistics.Mean
                select benchmark;

            public string GetHighlightGroupKey(BenchmarkCase benchmarkCase)
            {
                return benchmarkCase.DisplayInfo.Contains("XPO") ? benchmarkCase.DisplayInfo : null;
            }

            public string GetLogicalGroupKey(IConfig config, BenchmarkCase[] allBenchmarksCases, BenchmarkCase benchmarkCase) {
                return benchmarkCase.Descriptor.WorkloadMethod + "_" + benchmarkCase.Parameters["RowCount"];
            }

            public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(IEnumerable<IGrouping<string, BenchmarkCase>> logicalGroups) {
                return logicalGroups;
            }
            
            
        }
    }
}
