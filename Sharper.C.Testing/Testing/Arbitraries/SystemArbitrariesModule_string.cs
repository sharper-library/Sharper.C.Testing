using System.Linq;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<string> AnyString
    =>
        Arb.From
          ( Gen.ArrayOf(AnyChar.Generator).Select(cs => new string(cs))
          , AsciiString.Shrinker
          );

    public static Arbitrary<string> AsciiString
    =>
        Arb
        .Default
        .NonNull<string>()
        .Convert(x => x.Get, NonNull<string>.NewNonNull);

    public static Arbitrary<string> AlphaNumString
    =>
        Arb.From
          ( Gen
            .ArrayOf(AlphaNumChar.Generator)
            .Select(cs => new string(cs))
          , s => s == "0" ? Enumerable.Empty<string>() : new[] {"0"}
          );

    public static Arbitrary<string> NonEmpty(this Arbitrary<string> arb)
    =>
        arb.Filter(s => !string.IsNullOrEmpty(s));

    public static Arbitrary<string> NoNullChars(this Arbitrary<string> arb)
    =>
        arb.MapFilter(s => s.Replace("\0", "0"), _ => true);
}

}
