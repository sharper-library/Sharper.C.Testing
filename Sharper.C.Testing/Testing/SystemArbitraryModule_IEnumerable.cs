using System.Collections.Generic;
using System.Linq;
using Microsoft.FSharp.Collections;
using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
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