using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<bool> AnyBool
    =>
        Arb.Default.Bool();
}

}
