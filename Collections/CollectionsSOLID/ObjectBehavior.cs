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
        
        List<MethodInfo> _methods;
        object _objectInstance;
       
        ThreadSafeRandom _randomizer;

        public ObjectBehavior(Type type, IEnumerable<MethodInfo> methods)
        {
            _objectType = type;
            var areMethodsValid = Utils.MethodsUseSupportedTypes(methods);
            if(!areMethodsValid)
            {
                throw new ArgumentException("method(s) contains unsupported types");
            }
             _methods = methods.ToList();
            _objectInstance = CreateInstanceFromType(_objectType);
            if(_objectInstance == null)
            {
                var staticMethods = _methods.Where(m => !m.IsStatic);
                if (staticMethods.Any())
                {
                    throw new ArgumentException("cannot invoke non-static method of static class");
                }
            }
           

        }

      

        private object CreateInstanceFromType(Type type)
        {
            object obj = null;

            if (type.IsValueType)
            {
                obj = Activator.CreateInstance(type);
            }
            else
            {
                ConstructorInfo ci = null;
                ci = type.GetConstructor(Type.EmptyTypes);
                if (ci == null)
                {
                    if (!type.IsSealed && type.IsAbstract)
                    {
                        //abstract class
                        throw new Exception("abstract classes are not allowed");
                    }
                    if(type.IsSealed && type.IsAbstract)
                    {
                       //static class
                        return null;
                    }
                    throw new Exception("only types with empty constructors are allowed");
                    //todo: this doesnt work because lets say a type has many ctors with params,
                    //which one to choose? lets limit now to only allow empty constructors...until a better solution
                    //var parameters = new List<object>();


                    //var constructors = type.GetConstructors();
                    //foreach (var c in constructors)
                    //{
                    //    var constructorParameters = c.GetParameters();
                    //    ci = type.GetConstructor(constructorParameters.Select(p => p.ParameterType).ToArray());

                    //    foreach (var item in constructorParameters)
                    //    {
                    //        parameters.Add(Randomizer.RandomizeParamValue(item.ParameterType.Name));
                    //    }
                    //    obj = ci.Invoke(parameters.ToArray());

                    //}

                }
                else
                {
                    //constructor is paramless
                    obj = ci.Invoke(new object[] { });
                }

            }
            return obj;
        }

        private ThreadSafeRandom Randomizer
        {
            get
            {
                if(_randomizer == null)
                {
                    _randomizer = new ThreadSafeRandom();
                }
                return _randomizer;
            }
        }
    
        public bool Update(ILogger logger)
        {
            foreach (MethodInfo method in _methods)
            {

                var parameters = new List<object>();
                foreach (var p in method.GetParameters())
                {
                    var paramValue = Randomizer.RandomizeParamValue(p.ParameterType.Name);
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
            foreach (MethodInfo method in _methods)
            {
                var parameters = new List<object>();
                foreach (var p in method.GetParameters())
                {
                    var paramValue = Randomizer.RandomizeParamValue(p.ParameterType.Name);
                    parameters.Add(paramValue);
                }

                object result;
                try
                {
                    result = method.Invoke(_objectInstance, parameters.ToArray());
                    var methodToWrite = "CALLED: " + method.ToString();
                    var paramsToWrite = "ARGS: " + String.Join(",", parameters.ToArray());
                    var returnValueToWrite = "RETURNED: " + result;
                    var message = methodToWrite +"," + paramsToWrite + "," + returnValueToWrite;
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
