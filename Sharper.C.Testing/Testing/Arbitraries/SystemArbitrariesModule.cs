using System;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<B> Cast<A, B>(this Arbitrary<A> arb)
      where A : B
    =>
        arb.Convert(a => (B)a, b => (A)b);

    /// <summary>
    /// Derive arbitrary instance using reflection. Only for tuples and enums.
    /// </summary>
    public static Arbitrary<A> Derive<A>()
    =>
        Arb.Default.Derive<A>();

    public static Arbitrary<B> MapGenerator<A, B>
      ( this Arbitrary<A> arbA
      , Func<A, B> f
      )
    =>
        Arb.From(arbA.Generator.Select(f));

    public static Arbitrary<A> NoShrink<A>(this Arbitrary<A> arb)
    =>
        Arb.From(arb.Generator);
}

}
