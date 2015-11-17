using System;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<int> AnyInt
    =>
        Arb.Default.Int32();

    public static Arbitrary<int> Natural(this Arbitrary<int> arb)
    =>
        arb.MapFilter(Math.Abs, n => n > 0);

    public static Arbitrary<int> NonZero(this Arbitrary<int> arb)
    =>
        arb.Filter(n => n != 0);

    public static Arbitrary<int> WithMinMax(this Arbitrary<int> arb)
    =>
        Arb.From
          ( Gen.Frequency
              ( Tuple.Create(1, Gen.Elements(int.MaxValue, int.MinValue))
              , Tuple.Create(10, arb.Generator)
              )
          , arb.Shrinker
          );

    public static Arbitrary<int> BoundedInt(int min, int max)
    =>
        Arb.From(Gen.Choose(min, max), AnyInt.Shrinker);
}

}