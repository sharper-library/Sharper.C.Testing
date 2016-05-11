using System;
using System.Linq;
using Microsoft.FSharp.Collections;
using FsCheck;
using FsProp = FsCheck.Prop;

namespace Sharper.C.Testing
{

public static class PropertyModule
{
    public static Property When(bool condition, Action a)
    =>
        PropertyExtensions.When(a, condition);

    public static Property When(bool condition, Func<bool> f)
    =>
        PropertyExtensions.When(f, condition);

    public static Property When(bool condition, Property p)
    =>
        FsProp.given
          ( condition
          , p
          , FsProp.ofTestable
              ( new Result
                  ( Outcome.Rejected
                  , FSharpList<string>.Empty
                  , new FSharpSet<string>(Enumerable.Empty<string>())
                  , FSharpList<object>.Empty
                  )
              )
          );
}

}
