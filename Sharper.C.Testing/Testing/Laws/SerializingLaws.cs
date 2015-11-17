using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static PropertyModule;
using static Properties.FunctionPropertiesModule;

public static class SerializingLaws
{
    public static Invariant For<A>
      ( Action<A, Stream> serialize
      , Func<Stream, A> deserialize
      , Func<A, int> hash
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Serializing Laws"
        .All
          ( IsMonomorphism
              ( SerializeFunc(serialize)
              , DeserializeFunc(deserialize)
              , eq
              , arbA
              )
          , HashingLaws.For(hash, eq, arbA)
          );

    public static Invariant ForDefaultBinary<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For
          ( (a, s) => new BinaryFormatter().Serialize(s, a)
          , s => (A) new BinaryFormatter().Deserialize(s)
          , x => x.GetHashCode()
          , (x, y) => x.Equals(y)
          , arbA
          );

    private static Func<A, Stream> SerializeFunc<A>(Action<A, Stream> serialize)
    =>
        a =>
        {
            var s = new MemoryStream();
            serialize(a, s);
            return s;
        };

    private static Func<Stream, A> DeserializeFunc<A>
      ( Func<Stream, A> deserialize
      )
    =>
        s =>
        {
            using (s)
            {
                s.Seek(0, SeekOrigin.Begin);
                return deserialize(s);
            }
        };
}

}
