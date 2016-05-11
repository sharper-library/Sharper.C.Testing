using System;

namespace Sharper.C.Testing.Properties
{

public static class RuntimePropertiesModule
{
    public static Invariant WithoutOverflow<A>
      ( this string label
      , Func<A> build
      , Action<A> run
      )
    =>  label.All
          ( "Build".ForUnit(() => new Action(() => build()))
          , "Evaluate".ForUnit(() => new Action(() => run(build())))
          );
}

}
