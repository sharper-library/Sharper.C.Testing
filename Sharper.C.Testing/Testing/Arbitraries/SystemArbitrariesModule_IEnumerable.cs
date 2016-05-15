using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<IEnumerable<A>> AnySeq<A>(Arbitrary<A> arbA)
    =>
        Arb.From
          ( Gen.ListOf(arbA.Generator)
          , Arb.Default.FsList<A>().Shrinker
          )
        .Convert(fsList => fsList.AsEnumerable(), ListModule.OfSeq);
}

}
