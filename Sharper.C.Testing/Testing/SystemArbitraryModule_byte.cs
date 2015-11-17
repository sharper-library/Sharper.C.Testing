using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<byte> AnyByte
    =>
        Arb.Default.Byte();
}

}