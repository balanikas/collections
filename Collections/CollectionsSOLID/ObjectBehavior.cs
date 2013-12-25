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
        public void Update()
        {
            foreach (MethodInfo method in _actions)
            {


                if (!method.GetParameters().Any())
                {
                    method.Invoke(_objectInstance, new object[] { });
                }
                else
                {
                    if (method.GetParameters().Length > 0)
                    {
                       
                        var parameters = new List<object>();
                        foreach (var p in method.GetParameters())
                        {
                            var paramValue = Utils.RandomizeParamValue(p.ParameterType.Name);
                            parameters.Add(paramValue);
                            //Debug.WriteLine(" {0} ",paramValue);
                        }
                        try
                        {
                            var result = method.Invoke(_objectInstance, parameters.ToArray());
                        }
                        catch 
                        {
                            
                            
                        }
                        

                        //Debug.WriteLine("output: {0} ", result);
                    }
                   

                }

            }
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
