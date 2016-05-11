using System;
using System.Linq;
using FsCheck;
using Xunit;
using Xunit.Sdk;

using StdGen = FsCheck.Random.StdGen;

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
                StdGen = StdGen.NewStdGen(x[0], x[1]);
            }
        }
    }
}
