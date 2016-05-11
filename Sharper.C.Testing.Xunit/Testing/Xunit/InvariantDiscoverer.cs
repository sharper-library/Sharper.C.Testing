using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using FsCheck;
using StdGen = FsCheck.Random.StdGen;

using static Sharper.C.Testing.PropertyModule;

namespace Sharper.C.Testing.Xunit
{

    [AttributeUsage
      ( AttributeTargets.Method | AttributeTargets.Property
      , AllowMultiple=false
      )]
    [XunitTestCaseDiscoverer
      ( "Sharper.C.Testing.Xunit.InvariantDiscoverer"
      , "Sharper.C.Testing.Xunit"
      )]
    public sealed class InvariantAttribute
      : FactAttribute
    {
        public int MaxTest { get; set; } = Config.Default.MaxTest;
        public int MaxFail { get; set; } = Config.Default.MaxFail;
        public int StartSize { get; set; } = Config.Default.StartSize;
        public int EndSize { get; set; } = Config.Default.EndSize;
        public bool Verbose { get; set; } = false;
        public bool QuietOnSuccess { get; set; } = false;

        public StdGen StdGen { get; private set; }

        public string Replay
        {
            get { return $"{StdGen.Item1},{StdGen.Item2}"; }
            set
            {   var x = value.Split(',').Select(int.Parse).ToList();
                StdGen = MkStdGen(x[0], x[1]);
            }
        }
    }

    public sealed class InvariantDiscoverer
      : IXunitTestCaseDiscoverer
    {
        private IMessageSink MessageSink { get; }

        public InvariantDiscoverer(IMessageSink msgSink)
        {   MessageSink = msgSink;
        }

        public IEnumerable<IXunitTestCase> Discover
          ( ITestFrameworkDiscoveryOptions discoveryOptions
          , ITestMethod testMethod
          , IAttributeInfo factAttribute
          )
        {
            var inv = InvariantTestCase.InvariantFromMethod(testMethod);
            return
                inv == null
                ? new[]
                  { new InvariantTestCase
                      ( MessageSink
                      , discoveryOptions.MethodDisplayOrDefault()
                      , testMethod
                      , null
                      )
                  }
                : inv.AsSeq().Select
                    ( i =>
                          new InvariantTestCase
                            ( MessageSink
                            , discoveryOptions.MethodDisplayOrDefault()
                            , testMethod
                            , i
                            )
                    );
        }
    }

    public sealed class InvariantTestCase
      : XunitTestCase
    {
        public InvariantTestCase
          ( IMessageSink diagnosticMessageSink
          , TestMethodDisplay defaultMethodDisplay
          , ITestMethod testMethod
          , Invariant inv
          , object[] testMethodArguments = null
          )
            : base
            ( diagnosticMessageSink
            , defaultMethodDisplay
            , testMethod
            , testMethodArguments
            )
        {
            Inv = inv;
        }

        private Invariant Inv { get; }

        private ConfigF Config
          ( TestOutputHelper output
          )
        {   var attr =
                TestMethod
                .Method
                .GetCustomAttributes(typeof(InvariantAttribute))
                .First();
            var config =
                new InvariantConfig
                { MaxNbOfTest = attr.GetNamedArgument<int>("MaxTest")
                , MaxNbOfFailedTests = attr.GetNamedArgument<int>("MaxFail")
                , StartSize = attr.GetNamedArgument<int>("StartSize")
                , EndSize = attr.GetNamedArgument<int>("EndSize")
                , QuietOnSuccess =
                      attr.GetNamedArgument<bool>("QuietOnSuccess")
                , Every =
                      attr.GetNamedArgument<bool>("Verbose")
                      ? (n, args) =>
                        {   output.WriteLine
                              ( Configuration.Verbose.Every(n, args)
                              );
                            return "";
                        }
                      : Configuration.Quick.Every
                , EveryShrink =
                      attr.GetNamedArgument<bool>("Verbose")
                      ? args =>
                        {   output.WriteLine
                              ( Configuration.Verbose.EveryShrink(args)
                              );
                            return "";
                        }
                      : Configuration.Quick.EveryShrink
                , Replay = attr.GetNamedArgument<StdGen>("StdGen")
                };
            return
                runner =>
                {   config.Runner = runner;
                    return config;
                };
        }

        public override async Task<RunSummary> RunAsync
          ( IMessageSink diagnosticMessageSink
          , IMessageBus bus
          , object[] constructorArguments
          , ExceptionAggregator aggregator
          , CancellationTokenSource cancellationTokenSource
          )
        {   var test = new XunitTest(this, DisplayName);
            var output = new TestOutputHelper();
            var timer = new ExecutionTimer();
            var summary = new RunSummary();
            output.Initialize(bus, test);
            bus.QueueMessage(new TestStarting(test));
            if (Inv == null)
            {   var msg = "Return Type must be Invariant.";
                bus.QueueMessage
                  ( new TestFailed(test, 0, null, new Exception(msg))
                  );
                summary.Aggregate(new RunSummary { Total = 1, Failed = 1 });
                return summary;
            }
            InvariantResult result;
            timer.Aggregate(() => result = Inv.Results(Config(output)).First());
            var xresult = ToXunitResult(test, result, timer.Total);
            bus.QueueMessage(xresult.Item1);
            summary.Aggregate(xresult.Item2);
            return summary;
        }

        public static Invariant InvariantFromMethod(ITestMethod method)
        {   var m = method.Method.ToRuntimeMethod();
            if ( m != null
              && m.IsStatic
              && m.GetParameters().Length == 0
              && m.ReturnType == typeof(Invariant)
              )
            {   var i = m.Invoke(null, new object[] {});
                return i as Invariant;
            }
            return null;
        }

        private static Tuple<IMessageSinkMessage, RunSummary> ToXunitResult
          ( XunitTest test
          , InvariantResult ir
          , decimal time
          )
        =>
            ir.Passed
            ? Tuple.Create
                ( (IMessageSinkMessage)
                  new TestPassed(test, time, null)
                , new RunSummary {  Total = 1, Time = time }
                )

            : ir.Exhausted
            ? Tuple.Create
                ( (IMessageSinkMessage)
                  new TestFailed(test, time, null, new Exception("Exhaustion"))
                , new RunSummary { Total = 1, Failed = 1, Time = time }
                )

            : Tuple.Create
                ( (IMessageSinkMessage)
                  new TestFailed(test, time, null, new Exception(ir.Message))
                , new RunSummary { Total = 1, Failed = 1, Time = time }
                );
    }
}
