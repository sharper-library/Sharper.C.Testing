using System;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<long> AnyLong
    =>
        Arb.Default.Int64();

    public static Arbitrary<long> Natural(this Arbitrary<long> arb)
    =>
        arb.MapFilter(Math.Abs, n => n > 0);

    public static Arbitrary<long> NonZero(this Arbitrary<long> arb)
    =>
        arb.Filter(n => n != 0);

    public static Arbitrary<long> WithMinMax(this Arbitrary<long> arb)
    =>
        Arb.From
          ( Gen.Frequency
              ( Tuple.Create(1, Gen.Elements(long.MaxValue, long.MinValue))
              , Tuple.Create(10, arb.Generator)
              )
          , arb.Shrinker
          );
}

}