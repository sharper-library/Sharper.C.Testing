using System.Linq;
using FsCheck;

namespace Sharper.C.Testing.Arbitraries
{

public static partial class SystemArbitrariesModule
{
    public static Arbitrary<char> AnyChar
    =>
        Arb.From
          ( Gen.Choose(char.MinValue, '\xFFFF').Select(n => (char)n)
          , AsciiChar.Shrinker
          );

    public static Arbitrary<char> AsciiChar
    =>
        Arb.Default.Char();

    public static Arbitrary<char> BoundedChar(char min, char max)
    =>
        Arb.From
          ( Gen.Choose(min, max).Select(n => (char)n)
          , c => c == min ? Enumerable.Empty<char>() : new[] {min}
          );

    public static Arbitrary<char> AlphaNumChar
    =>
        Arb.From
          ( Gen.OneOf
              ( BoundedChar('0', '9').Generator
              , BoundedChar('a', 'z').Generator
              , BoundedChar('A', 'Z').Generator
              )
          , c => c == '0' ? Enumerable.Empty<char>() : new[] {'0'}
          );
}

}
