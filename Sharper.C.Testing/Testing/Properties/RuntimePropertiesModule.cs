using System;
using Fuchu;

namespace Sharper.C.Testing.Properties
{

public static class RuntimePropertiesModule
{
    public static Test WithoutOverflow<A>
      ( this string label
      , Func<A> build
      , Action<A> run
      )
    =>  label.All
          ( Test.Case("Build", () => build())
          , Test.Case("Evaluate", () => run(build()))
          );
}

}
