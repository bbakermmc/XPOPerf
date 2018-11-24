
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Configuration;
using BenchmarkDotNet.Running;
using Microsoft.EntityFrameworkCore;
using ORMBenchmark.Models.EFCore;
using ORMBenchmark.PerformanceTests;
using ILogger = NLog.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace XPOPerformance
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
        
        static void Main(string[] args) {
            var servicesProvider = BuildDi();
            
            for(int i = 0; i < args.Length; i++) {
                if(args[i].ToLower() == "-count") {
                    var counts = new List<long>();
                    for(i++; i < args.Length; i++) {
                        int count;
                        if(Int32.TryParse(args[i], out count)) {
                            counts.Add(count);
                        } else {
                            i--;
                            break;
                        }
                    }
                    if(counts.Count > 0) {
                        TestSetConfig.RowCounts = counts.ToArray();
                    }
                }
            }
            var summary = BenchmarkRunner.Run<PerformanceTestSet>(new TestSetConfig());
           // string resultsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BenchmarkDotNet.Artifacts", "results", "ORMBenchmark.PerformanceTests.PerformanceTestSet-report.html");
            //System.Diagnostics.Process.Start(resultsPath);
            NLog.LogManager.Shutdown(); 
        }
        
        private static IServiceProvider BuildDi()
        {
            var services = new ServiceCollection();

            
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Trace));
            
     
            var serviceProvider = services.BuildServiceProvider();

            var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

            //configure NLog
            loggerFactory.AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties =true });
            NLog.LogManager.LoadConfiguration("nlog.config");

            return serviceProvider;
        }
        
    }

  
}