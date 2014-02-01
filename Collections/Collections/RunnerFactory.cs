using System;

namespace Collections
{
   

    public static class RunnerFactory
    {
        public static IRunner Get(IBehavior behavior, IGui gui, ILogger logger,RunnerSettings settings)
        {
            switch (settings.RunnerType)
            {
                case RunnerType.BackgroundWorkerBased:
                    return new BWBasedRunner(behavior, gui, logger, settings);
                case RunnerType.ParallelTaskBased:
                    return new TplBasedRunner(behavior, gui, logger, settings);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}