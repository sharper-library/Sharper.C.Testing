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
        private readonly bool silent;
        private readonly List<InvariantResult> results;

        private InvariantRunner(bool silent, List<InvariantResult> results)
        {   this.silent = silent;
            this.results = results;
        }

        public static InvariantRunner Mk(bool silent = false)
        =>  new InvariantRunner(silent, new List<InvariantResult>());

        public IEnumerable<InvariantResult> Results
        =>  results;

        void IRunner.OnArguments
          ( int ntest
          , FSharpList<object> args
          , FSharpFunc<int, FSharpFunc<FSharpList<object>, string>> every
          )
        {   var msg = every.Invoke(ntest).Invoke(args);
            if (!silent)
            {   Console.Write(msg);
            }
        }

        void IRunner.OnFinished(string name, TestResult result)
        {   results.Add(InvariantResult.Mk(name, result));
            if (!silent)
            {   Console.Write(Runner.onFinishedToString(name, result));
            }
        }

        void IRunner.OnShrink
          ( FSharpList<object> args
          , FSharpFunc<FSharpList<object>, string> everyShrink
          )
        {   var msg = everyShrink.Invoke(args);
            if (!silent)
            {   Console.Write(msg);
            }
        }

        void IRunner.OnStartFixture(Type t)
        {   var msg = Runner.onStartFixtureToString(t);
            if (!silent)
            {   Console.Write(msg);
            }
        }
            
    }
}
