using System;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<double> AnyDouble
    =>
        Arb.Default.Float();

    public static Arbitrary<double> Positive(this Arbitrary<double> arb)
    =>
        arb.MapFilter(Math.Abs, _ => true);

    public static Arbitrary<double> NoNaN(this Arbitrary<double> arb)
    =>
        arb.Filter(d => !double.IsNaN(d));

    public static Arbitrary<double> NoInf(this Arbitrary<double> arb)
    =>
        arb.Filter(d => !double.IsInfinity(d));
}

}
