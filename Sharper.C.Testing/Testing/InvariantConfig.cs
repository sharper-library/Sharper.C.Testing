using FsCheck;

namespace Sharper.C.Testing
{
    public sealed class InvariantConfig
      : Configuration
    {
        public new InvariantRunner Runner
        {
            get { return (InvariantRunner) base.Runner; }
            set { base.Runner = value; }
        }

        public new static InvariantConfig Default(InvariantRunner r)
        =>  new InvariantConfig { Runner = r };
    }
}
