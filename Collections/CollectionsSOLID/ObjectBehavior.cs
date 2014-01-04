using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CollectionsSOLID;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace CollectionsSOLID
{
    public class ObjectBehavior : IBehavior
    {
        Type _objectType;
        IEnumerable _actions;
        List<MethodInfo> _methods;
        object _objectInstance;

        public ObjectBehavior(Type type, IEnumerable<MethodInfo> actions)
        {
            _objectType = type;
            _actions = actions;
            _methods = new List<MethodInfo>();

            ConstructorInfo ci = null;

            if (_objectType.IsValueType)
            {
                _objectInstance = Activator.CreateInstance(_objectType);
            }
            else
            {
                ci = _objectType.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                {
                    var parameters = new List<object>();

                    //no paramless constructor
                    var constructors = _objectType.GetConstructors();
                    foreach(var c in constructors)
                    {
                        var constructorParameters = c.GetParameters();
                        ci = _objectType.GetConstructor(constructorParameters.Select(p=> p.ParameterType).ToArray());
                        
                        foreach (var item in constructorParameters)
                        {
                            parameters.Add(Utils.RandomizeParamValue(item.ParameterType.Name));
                        }
                        _objectInstance = ci.Invoke(parameters.ToArray());

                    }

                }
                else
                {
                    _objectInstance = ci.Invoke(new object[] { });
                }
                
            }
            
          
            foreach (MethodInfo action in _actions)
            {
                _methods.Add(action);
            }
            

        }
        public bool Update(ILogger logger)
        {
            foreach (MethodInfo method in _actions)
            {
                var parameters = new List<object>();
                foreach (var p in method.GetParameters())
                {
                    var paramValue = Utils.RandomizeParamValue(p.ParameterType.Name);
                    parameters.Add(paramValue);
                }

                try
                {
                    var result = method.Invoke(_objectInstance, parameters.ToArray());
                }
                catch (System.Exception e)
                {
                    var errorMsg = e.InnerException != null ? e.InnerException.Message : e.Message;
                    logger.Error(errorMsg);
                    return false;
                }
                
            }

            return true;
        }

        public bool UpdateAndLog(ILogger logger)
        {
            foreach (MethodInfo method in _actions)
            {
                var parameters = new List<object>();
                foreach (var p in method.GetParameters())
                {
                    var paramValue = Utils.RandomizeParamValue(p.ParameterType.Name);
                    parameters.Add(paramValue);
                }

                object result;
                try
                {
                    result = method.Invoke(_objectInstance, parameters.ToArray());
                    var methodToWrite = "called: " + method.ToString();
                    var paramsToWrite = "with params: " + String.Join(",", parameters.ToArray());
                    var returnValueToWrite = "returned: " + result;
                    var message = methodToWrite + Environment.NewLine + paramsToWrite + Environment.NewLine + returnValueToWrite + Environment.NewLine;
                    logger.Info(message);
                }
                catch (System.Exception e)
                {
                    var errorMsg = e.InnerException != null ? e.InnerException.Message : e.Message;
                    logger.Error(errorMsg);
                    return false;
                }

                
            }

            return true;
        }

        public Type GetObjectType()
        {
            return _objectType;
        }

        public Type GetCollectionType()
        {
            return _objectType;
        }
    }
}
