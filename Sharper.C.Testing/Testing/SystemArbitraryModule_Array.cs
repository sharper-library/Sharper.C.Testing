using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<A[]> AnyArray<A>(Arbitrary<A> arbA)
    =>
        Arb.From
          ( Gen.ArrayOf(arbA.Generator)
          , Arb.Default.Array<A>().Shrinker
          );

    public static Arbitrary<A[]> NonEmpty<A>(this Arbitrary<A[]> arb)
    =>
        arb.Filter(a => a.Length > 0);
}

}
