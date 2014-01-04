using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CollectionsSOLID
{
    public class Runtime
    {
        IDictionary<string,IRunner> _gObjects;
        bool _isRunning = false;
        public Runtime()
        {
            _gObjects = new Dictionary<string, IRunner>();

        }
        public void Stop() 
        {
            _isRunning = false;
            
        }
       
        public void Start() 
        {
            
            _isRunning = true;

            while (_isRunning)
            {
            }

            Clear();
        
        }

        public void Clear()
        {
            foreach (var go in _gObjects.Values)
            {
                go.Destroy();
            }
        }

        public void Add(IRunner obj)
        {
            _gObjects.Add(obj.Id,obj);

        }

        public void Remove(string objId)
        {
            IRunner obj;
            if(_gObjects.TryGetValue(objId, out obj))
            {
                _gObjects.Remove(objId);
                obj.Destroy();
            }
                        
        }

        public bool IsRunning()
        {
            return _isRunning;
        }
    }
}
