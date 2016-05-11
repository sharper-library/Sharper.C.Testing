using System;
using FsCheck;

namespace Sharper.C.Testing
{
    public struct Testable
    {
        private Testable(Property p)
        {   Prop = p;
        }

        public Property Prop { get; }

        public static implicit operator Testable(Property p)
        =>  new Testable(p);

        public static implicit operator Testable(bool b)
        =>  new Testable(b.ToProperty());

        public static implicit operator Testable(Action a)
        =>  new Testable(a.ToProperty());

        public static implicit operator Testable(Invariant i)
        =>  new Testable(i.LabeledProperty);
    }
}
