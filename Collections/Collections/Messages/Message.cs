﻿namespace Collections.Messages
{
    public enum ObjectState
    {
        Running,
        Finished,
        Unknown
    }

    public class Message
    {
        public int Progress { get; protected set; }
    }
}