using System.IO;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<MemoryStream> AnyStream
    =>
        Arb
        .Default
        .Array<byte>()
        .Convert
          ( bytes => new MemoryStream(bytes)
          , stream => stream.ToArray()
          );

    public static Arbitrary<Stream> AsStream<S>(this Arbitrary<S> arb)
      where S : Stream
    =>
        arb.Cast<S, Stream>();
}

}
