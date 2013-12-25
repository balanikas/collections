using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    class MissileBehavior : IMissileBehavior
    {
        private Type m_ammoType;
        private Type m_collectionType;
        public IList Actions;
        private Dictionary<string, MethodInfo> m_actions;
        public MissileBehavior(Type type, Type collectionType, List<string> actions )
        {
            m_ammoType = type;
            Actions = actions;
            m_collectionType = collectionType;
            m_actions = new Dictionary<string, MethodInfo>();
            
        }
        public void Update(object o)
        {
            foreach (string action in Actions)
            {

                if (!m_actions.ContainsKey(action))
                {
                    m_actions.Add(action, o.GetType().GetMethod(action));
                }
                if (!m_actions[action].GetParameters().Any())
                {
                    m_actions[action].Invoke(o, new object[] { });
                }
                else
                {
                    m_actions[action].Invoke(o, new object[] { Activator.CreateInstance(m_ammoType) });
                }
                
                


            }
        }

        public Type GetAmmoType()
        {
            return m_ammoType;
        }

        public Type GetWeaponType()
        {
            return m_collectionType;
        }
    }

    //class MissileBehavior<T> : IMissileBehavior
    //{
    //    public string Id;
    //    public IList Actions;


    //    private Type m_collectionType;
    //    private readonly Func<object, string> m_updateDelegate;
        
    //    private Dictionary<string, MethodInfo> m_actions; 
    //    private MethodInfo m_m1 = null;
    //    private object m_ammo = null;


    //    public MissileBehavior(Type collectionType)
    //    {
    //        m_collectionType = collectionType;

    //        m_actions = new Dictionary<string, MethodInfo>();
    //        if (m_ammo == null)
    //        {
    //            m_ammo = Activator.CreateInstance<T>();
    //        }
    //        m_updateDelegate = delegate(object o)
    //        {
                
    //            foreach (string action in Actions)
    //            {
                    
    //                if (!m_actions.ContainsKey(action))
    //                {
    //                    m_actions.Add(action, o.GetType().GetMethod(action));
    //                }
    //                if (!m_actions[action].GetParameters().Any())
    //                {
    //                    m_actions[action].Invoke(o, new object[] {  });
    //                }
    //                else
    //                {
    //                    m_actions[action].Invoke(o, new object[] { m_ammo });
    //                }
                    

    //            }
                
    //            return "";
    //        }; 
    //    }

    //    public void Update(object o)
    //    {
    //        m_updateDelegate(o);
    //    }

    //    public Type GetAmmoType()
    //    {
    //        return typeof (T);
    //    }

    //    public Type GetWeaponType()
    //    {
    //        return m_collectionType;
    //    }
    //}

    internal interface IMissileBehavior
    {
        void Update(object o);
        Type GetAmmoType();
        Type GetWeaponType();
    }
}
