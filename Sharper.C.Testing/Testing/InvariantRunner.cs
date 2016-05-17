using System;
using System.Collections.Generic;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using FsCheck;

namespace Sharper.C.Testing
{
    public struct InvariantRunner
      : IRunner
    {
        private readonly List<InvariantResult> results;

        private InvariantRunner(List<InvariantResult> results)
        {   this.results = results;
        }

        public static InvariantRunner Mk()
        =>  new InvariantRunner(new List<InvariantResult>());

        public IEnumerable<InvariantResult> Results
        =>  results;

        void IRunner.OnArguments
          ( int ntest
          , FSharpList<object> args
          , FSharpFunc<int, FSharpFunc<FSharpList<object>, string>> every
          )
        =>  every.Invoke(ntest).Invoke(args);

        void IRunner.OnFinished(string name, TestResult result)
        =>  results.Add(InvariantResult.Mk(name, result));

        void IRunner.OnShrink
          ( FSharpList<object> args
          , FSharpFunc<FSharpList<object>, string> everyShrink
          )
        =>  everyShrink.Invoke(args);

        void IRunner.OnStartFixture(Type t)
        =>  Runner.onStartFixtureToString(t);
    }
}
