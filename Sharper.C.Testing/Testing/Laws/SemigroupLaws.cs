using System;
using FsCheck;
using static Sharper.C.Testing.Properties.OperationPropertiesModule;

namespace Sharper.C.Testing.Laws
{
    public static class SemigroupLaws
    {
        public static Invariant For<A>
          ( Func<A, A, A> plus
          , Func<A, A, bool> eq
          , Arbitrary<A> arbA
          )
        => "Semigroup Laws".All(IsAssociative(plus, eq, arbA));

        public static Invariant For<A>
          ( Func<A, A, A> plus
          , Arbitrary<A> arbA
          )
          where A : IEquatable<A>
        => For(plus, (x, y) => x.Equals(y), arbA);
    }
}
