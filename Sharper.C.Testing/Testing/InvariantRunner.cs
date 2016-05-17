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

        public void OnArguments
          ( int ntest
          , FSharpList<object> args
          , FSharpFunc<int, FSharpFunc<FSharpList<object>, string>> every
          )
        =>  every.Invoke(ntest).Invoke(args);

        public void OnFinished(string name, TestResult result)
        =>  results.Add(InvariantResult.Mk(name, result));

        public void OnShrink
          ( FSharpList<object> args
          , FSharpFunc<FSharpList<object>, string> everyShrink
          )
        =>  everyShrink.Invoke(args);

        public void OnStartFixture(Type t)
        =>  Runner.onStartFixtureToString(t);
    }
}
