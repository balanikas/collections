using Collections.Compiler;

namespace Collections.Runtime
{
    public struct RunnerSettings
    {
        public readonly int Iterations;
        public readonly RunnerType RunnerType;
        public readonly CompilerType CompilerServiceType;

        public RunnerSettings(int iterations, RunnerType runnerType, CompilerType compilerType)
        {
            Iterations = iterations;
            RunnerType = runnerType;
            CompilerServiceType = compilerType;
        }
    }
}
