using FsCheck;

namespace Sharper.C.Testing
{
    public struct InvariantResult
    {
        public string Label { get; }

        private TestResult Result { get; }

        private InvariantResult(string label, TestResult result)
        {   Label = label;
            Result = result;
        }

        public string Message
        =>  Runner.onFinishedToString(Label, Result);

        public bool Passed
        =>  Result.IsTrue;

        public bool Failed
        =>  !Passed;

        public bool Exhausted
        =>  Result.IsExhausted;

        public static InvariantResult Mk
          ( string label
          , TestResult result
          )
        =>  new InvariantResult(label, result);
    }
}
