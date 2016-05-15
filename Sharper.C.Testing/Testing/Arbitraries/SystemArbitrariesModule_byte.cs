using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<byte> AnyByte
    =>
        Arb.Default.Byte();
}

}
