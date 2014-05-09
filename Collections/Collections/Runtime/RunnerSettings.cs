using Collections.Compiler;

namespace Collections.Runtime
{
    public class RunnerSettings
    {
        public int Iterations { get; set; }
        public RunnerType RunnerType { get; set; }

        public CompilerType CompilerServiceType { get; set; } 
        
    }
}
