using FsCheck;

namespace Sharper.C.Testing
{

public static partial class SystemArbitraryModule
{
    public static Arbitrary<bool> AnyBool
    =>
        Arb.Default.Bool();
}

}