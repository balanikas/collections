using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Collections.Utilities
{
    public class Timing
    {
      
        public static TimeSpan MeasureExecutionTime<T>(Action action)
        {
            var watch = new Stopwatch();
            watch.Start();
            action();
            watch.Stop();
            return watch.Elapsed;
        }

        public static TimeSpan MeasureExecutionTime<T>(Action<T> action, T obj)
        {
            var watch = new Stopwatch();
            watch.Start();
            action.Invoke(obj);
            watch.Stop();
            return watch.Elapsed;
        }
    }
}