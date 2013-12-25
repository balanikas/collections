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

namespace CollectionsSOLID 
{
    public class CollectionBehavior : IBehavior
    {
        Type _objectType;
        Type _collectionType;
        IEnumerable _actions;
        Dictionary<string, MethodInfo> _methods;
        object _collectionInstance;
        

        public CollectionBehavior(Type type, Type collectionType, IEnumerable<string> actions)
        {
            _objectType = type;
            _actions = actions;
            _collectionType = collectionType;
            _methods = new Dictionary<string, MethodInfo>();

            ConstructorInfo ci = null;

            if (_collectionType.IsGenericTypeDefinition)
            {
                if (_collectionType.GetGenericArguments().Length == 1)
                {
                    Type specific = _collectionType.MakeGenericType(_objectType);
                    ci = specific.GetConstructor(Type.EmptyTypes);
                }
                else if (_collectionType.GetGenericArguments().Length == 2)
                {
                    Type specific = _collectionType.MakeGenericType(typeof(Guid), _objectType);
                    ci = specific.GetConstructor(Type.EmptyTypes);
                }

            }
            else
            {
                ci = _collectionType.GetConstructor(Type.EmptyTypes);
            }
            _collectionInstance = ci.Invoke(new object[] { });

            
        }
        public void Update()
        {
            foreach (string method in _actions)
            {

                if (!_methods.ContainsKey(method))
                {
                    _methods.Add(method, _collectionInstance.GetType().GetMethod(method));
                }
                if (!_methods[method].GetParameters().Any())
                {
                    _methods[method].Invoke(_collectionInstance, new object[] { });
                }
                else
                {
                    if (_methods[method].GetParameters().Length == 1)
                    {
                        _methods[method].Invoke(_collectionInstance, new object[] { Activator.CreateInstance(_objectType) });
                    }
                    else if (_methods[method].GetParameters().Length == 2)
                    {
                        _methods[method].Invoke(_collectionInstance, new object[] { Guid.NewGuid(), Activator.CreateInstance(_objectType) });
                    }
                    else
                    {
                        throw new NotImplementedException();
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
            return _collectionType;
        }
    }
}
