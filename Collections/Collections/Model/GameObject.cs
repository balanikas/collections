using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Collections
{
    
    internal abstract class GameObject
    {

        internal abstract void Start();

        internal abstract void Update();

        internal abstract void AddGraphics(IGuiObject graphics);
       

        internal abstract IGuiObject GetGraphics();

        internal abstract void Destroy();

        internal abstract bool IsAlive();

       
  
    }
}
