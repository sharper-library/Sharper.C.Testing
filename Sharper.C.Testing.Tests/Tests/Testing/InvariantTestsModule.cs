using System;
using System.Linq;
using Xunit;
using FsCheck;

using Sharper.C.Testing.Xunit;
using static Sharper.C.Testing.Arbitraries.SystemArbitrariesModule;

namespace Sharper.C.Testing.Tests.Testing
{
    public static class InvariantTestsModule
    {
        public static Arbitrary<Invariant> AnyInvariant1
        =>  Arb.From(GenInvariant1(Gen.Elements(true, false)));

        public static Arbitrary<Invariant> PassInvariant1
        =>  Arb.From(GenInvariant1(Gen.Constant(true)));

        public static Arbitrary<Invariant> FailInvariant1
        =>  Arb.From(GenInvariant1(Gen.Constant(false)));

        public static Arbitrary<Invariant> AnyInvariant
        =>  Arb.From(GenAnyInvariant(Gen.Elements(true, false)));

        public static Arbitrary<Invariant> PassInvariant
        =>  Arb.From(GenAnyInvariant(Gen.Constant(true)));

        [Fact]
        public static void SingleInvariant_ValuePassesEquivalence()
        =>  Prop
            .ForAll(AnyBool, b => b == ConstantInvariant(b).Passes())
            .Label("value <=> Passes()")
            .Run(4);

        [Fact]
        public static void SingleInvariant_ValueCheckEquivalence()
        =>  Prop
            .ForAll(AnyBool, b => b == ConstantInvariant(b).Check())
            .Label("value <=> Check()")
            .Run(4);

        [Fact]
        public static void Invariant_CheckPassEquivalence()
        =>  Prop
            .ForAll(AnyInvariant, i => i.Passes() == i.Check())
            .Label("Passes <=> Check [absent replays]")
            .Run();

        [Fact]
        public static void Invariant_NoFailuresIsPassing()
        =>  Prop
            .ForAll(PassInvariant, i => i.Passes())
            .Label("all true => pass")
            .Run();

        [Fact]
        public static void Invariant_NoFailuresIsCheck()
        =>  Prop
            .ForAll(PassInvariant, i => i.Check())
            .Label("all true => check")
            .Run();

        [Fact]
        public static void Invariant_CheckThrowEquivalence()
        =>  Prop
            .ForAll
              ( AnyInvariant
              , i => i.Check() == !Throws(() => i.CheckThrow())
              )
            .Label("Check <=> CheckThrow")
            .Run();

        [Invariant]
        public static Invariant Invariant_XunitDiscoverySmokeTest()
        =>  "Invariant Xunit discovery smoke test".All
            ( "Sub test A".ForAll(AnyBool, _ => true)
            , "Sub test B".ForAll(AnyBool, _ => true)
            );

        [Invariant(Replay = "1996231529,296153882")]
        public static Invariant Invariant_XunitDiscoverySmokeTest_Replay()
        =>  "Replay smoke test".All(Invariant_XunitDiscoverySmokeTest());

        private static bool Throws(Action a)
        {   try
            {   a();
                return false;
            }
            catch
            {   return true;
            }
        }

        private static Invariant ConstantInvariant(bool pass)
        =>  $"Constant {pass}".ForAll(Arb.From(Gen.Constant(pass)), a => a);

        private static Gen<Invariant> GenInvariant1(Gen<bool> pass)
        =>  from p in pass
            from g in Gen.Constant(ConstantInvariant(p))
            select g;

        private static Gen<Invariant> GenGroup
          ( int depth
          , int s
          , Gen<bool> pass
          )
        =>  Gen.OneOf
              ( s > 0 && depth > 0
                ? new[]
                  { Gen.Resize(s - 20, GenInvariantGroup(depth - 1, pass))
                  , GenInvariant1(pass)
                  }
                : new[] { GenInvariant1(pass) }
              );

        private static Gen<Invariant> GenInvariantGroup
          ( int depth
          , Gen<bool> pass
          )
        =>  from n in Gen.Choose(1, 4)
            from inv in
                Gen.ArrayOf(n, Gen.Sized(s => GenGroup(depth, s, pass)))
            select "Group".All(inv);

        private static Gen<Invariant> GenAnyInvariant(Gen<bool> pass)
        =>  from g in
                Gen.Frequency
                  ( Tuple.Create(1, GenInvariant1(pass))
                  , Tuple.Create(2, GenInvariantGroup(2, pass))
                  )
            select g;

        private static Configuration Config(int? maxTests = null)
        =>  new Configuration
            { MaxNbOfTest = maxTests ?? FsCheck.Config.Default.MaxTest
            };

        private static void Run(this Property p, int? maxTests = null)
        =>  p.QuickCheckThrowOnFailure();
    }
}
