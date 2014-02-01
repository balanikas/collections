﻿using System;

namespace Collections
{
    public enum ObjectType
    {
        BackgroundWorkerBased,
        ParallelTaskBased
    }

    public static class RunnerFactory
    {
        public static IRunner Get(ObjectType type, IBehavior behavior, IGui gui, ILogger logger, int loopCount = 1000000)
        {
            switch (type)
            {
                case ObjectType.BackgroundWorkerBased:
                    return new BWBasedRunner(behavior, gui, logger, loopCount);
                case ObjectType.ParallelTaskBased:
                    return new TplBasedRunner(behavior, gui, logger, loopCount);
                default:
                    throw new NotImplementedException();
            }
        }
    }
}