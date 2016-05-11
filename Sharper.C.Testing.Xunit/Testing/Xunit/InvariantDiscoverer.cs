using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;
using FsCheck;

namespace Sharper.C.Testing.Xunit
{
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
}
