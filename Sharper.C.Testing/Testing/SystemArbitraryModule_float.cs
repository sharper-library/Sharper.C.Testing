using System;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<float> AnyFloat
    =>
        Arb.Default.Float32();

    public static Arbitrary<float> Positive(this Arbitrary<float> arb)
    =>
        arb.MapFilter(Math.Abs, _ => true);

    public static Arbitrary<float> NoNaN(this Arbitrary<float> arb)
    =>
        arb.Filter(d => !float.IsNaN(d));

    public static Arbitrary<float> NoInf(this Arbitrary<float> arb)
    =>
        arb.Filter(d => !float.IsInfinity(d));
}

}