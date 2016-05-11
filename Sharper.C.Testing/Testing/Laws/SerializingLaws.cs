using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FsCheck;

namespace Sharper.C.Testing.Laws
{

using static Properties.FunctionPropertiesModule;

public static class SerializingLaws
{
    public static Invariant For<A, S>
      ( Func<A, S> serialize
      , Func<S, A> deserialize
      , Func<A, int> hash
      , Func<A, A, bool> eq
      , Arbitrary<A> arbA
      )
    =>
        "Serializing Laws"
        .All
          ( IsInjection
              ( serialize
              , deserialize
              , eq
              , arbA
              )
          , HashingLaws.For(hash, eq, arbA)
          );

    public static Invariant ForDefaultBinary<A>(Arbitrary<A> arbA)
      where A : IEquatable<A>
    =>
        For
          ( SerializeFunc<A>((a, s) => new BinaryFormatter().Serialize(s, a))
          , DeserializeFunc(s => (A) new BinaryFormatter().Deserialize(s))
          , x => x.GetHashCode()
          , (x, y) => x.Equals(y)
          , arbA
          );

    private static Func<A, byte[]> SerializeFunc<A>(Action<A, Stream> serialize)
    =>
        a =>
        {
            using (var s = new MemoryStream())
            {
                serialize(a, s);
                return s.ToArray();
            }
        };

    private static Func<byte[], A> DeserializeFunc<A>
      ( Func<Stream, A> deserialize
      )
    =>
        bytes =>
        {
            using (var s = new MemoryStream(bytes))
            {
                s.Seek(0, SeekOrigin.Begin);
                return deserialize(s);
            }
        };
}

}
