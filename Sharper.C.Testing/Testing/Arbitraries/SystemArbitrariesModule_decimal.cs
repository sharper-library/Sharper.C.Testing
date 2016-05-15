using System;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<decimal> AnyDecimal
    =>
        Arb.Default.Decimal();

    public static Arbitrary<decimal> Positive(this Arbitrary<decimal> arb)
    =>
        arb.MapFilter(Math.Abs, _ => true);
}

}
