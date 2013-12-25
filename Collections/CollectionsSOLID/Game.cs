using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CollectionsSOLID
{
    public class Game
    {
        IDictionary<string,IRunner> _gObjects;
        bool _isRunning = false;
        public Game()
        {
            _gObjects = new Dictionary<string, IRunner>();

        }
        public void Stop() {
            _isRunning = false;
            Debug.WriteLine("game stopped");
        }
       
        public void Start() 
        {
            Debug.WriteLine("game started");
            _isRunning = true;

            while (_isRunning)
            {
                
                //System.Threading.Thread.Sleep(500);
            }

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
