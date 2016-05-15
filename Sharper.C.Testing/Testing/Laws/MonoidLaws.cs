using System;
using FsCheck;
using static Sharper.C.Testing.Properties.OperationPropertiesModule;

namespace Sharper.C.Testing.Laws
{
    public sealed class MonoidLaws
    {
        public static Invariant For<A>
          ( Func<A, A, A> plus
          , A zero
          , Func<A, A, bool> eq
          , Arbitrary<A> arbA
          )
        =>  "Monoid Laws".All
              ( HasIdentity(plus, zero, eq, arbA)
              , SemigroupLaws.For(plus, eq, arbA)
              );

        public static Invariant For<A>
          ( Func<A, A, A> plus
          , A zero
          , Arbitrary<A> arbA
          )
          where A : IEquatable<A>
        =>  For(plus, zero, (x, y) => x.Equals(y), arbA);
    }
}
