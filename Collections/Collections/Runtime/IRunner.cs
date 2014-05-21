﻿using Collections.Messages;

namespace Collections.Runtime
{
    public interface IRunner
    {
        string Id { get; }
        void Start();
        void Destroy();
        bool IsAlive();
        MethodExecutionResultAggregation GetCurrentState();

        void AddUiListener(IGui listener);
        void RemoveUiListener(IGui listener);
    }
}