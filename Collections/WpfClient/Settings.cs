using System;
using System.Collections.Generic;
using Collections.Compiler;
using Collections.Runtime;

namespace WpfClient
{
    public enum DrawTypes
    {
        Circle,
        Rectangle
    }

    public enum Setting
    {
        ExploreModeIterationCount,
        PlayModeIterationCount,
        CompilerInterval,
        RunnerInterval,
        DrawAs,
        ThreadingType,
        CompilerServiceType,
    }


    public class Key<T>
    {
        public Setting Type { get; private set; }

        public Key(Setting type)
        {
            Type = type;
        }
    }

   
    internal sealed class Settings
    {

        public class Keys
        {
            public static readonly Key<int> ExploreModeIterationCount = new Key<int>(Setting.ExploreModeIterationCount);
            public static readonly Key<int> PlayModeIterationCount = new Key<int>(Setting.PlayModeIterationCount);
            public static readonly Key<int> CompilerInterval = new Key<int>(Setting.CompilerInterval);
            public static readonly Key<int> RunnerInterval = new Key<int>(Setting.RunnerInterval);
            public static readonly Key<DrawTypes> DrawAs = new Key<DrawTypes>(Setting.DrawAs);
            public static readonly Key<RunnerType> ThreadingType = new Key<RunnerType>(Setting.ThreadingType);
            public static readonly Key<CompilerType> CompilerServiceType = new Key<CompilerType>(Setting.CompilerServiceType);
        }  

        public T Get<T>(Key<T> key)
        {
            return (T)_settings[key.Type];
        }

        public void Set<T>(Key<T> key, T value)
        {
            _settings[key.Type] = value;
            TriggerNotification(key.Type, value);
        }

        private static volatile Settings _instance;
        private static object _syncRoot = new object();


        private Dictionary<Setting, object> _settings;

        private Settings()
        {
            _settings = new Dictionary<Setting, object>();
            _settings.Add(Setting.ExploreModeIterationCount, 1);
            _settings.Add(Setting.PlayModeIterationCount, 1);
            _settings.Add(Setting.CompilerInterval, 1000);
            _settings.Add(Setting.RunnerInterval, 1000);
            _settings.Add(Setting.DrawAs, DrawTypes.Circle);
            _settings.Add(Setting.ThreadingType, RunnerType.BackgroundWorkerBased);
            _settings.Add(Setting.CompilerServiceType, CompilerType.Default);

           
        }

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_syncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new Settings();
                        }
                    }
                }
                return _instance;
            }
        }

        //public void UpdateSetting(Setting key, object value)
        //{
        //    //if (_settings[key] != value)
        //    //{
        //    //    if (value.GetType() == _settings[key].GetType())
        //    //    {
        //    //        _settings[key] = value;
        //    //        TriggerNotification(key, value);
        //    //    }
                
        //    //}
           
        //}

        //public T GetSetting<T>(Setting key) where T : struct
        //{
        //    return (T) _settings[key];
        //}

        public event EventHandler<SettingsUpdatedEventArgs> OnSettingsUpdated;
        private void TriggerNotification(Setting key, object value)
        {
            var handler = OnSettingsUpdated;
            if (handler != null)
            {
                handler(this, new SettingsUpdatedEventArgs(key, value));
            }
        }
    }

    public class SettingsUpdatedEventArgs : EventArgs
    {
        public Setting Type { get; private set; }
        public object Value { get; private set; }

        public SettingsUpdatedEventArgs(Setting type, object value)
        {
            Type = type;
            Value = value;
        }
    }

  
}