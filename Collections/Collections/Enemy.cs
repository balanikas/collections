using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Collections
{
    class Enemy : GameObject, IDrawable, IMoveable
    {
        private IGuiObject _guiObject;
        internal override void Start()
        {
            _guiObject.SetHeight(30);
            _guiObject.SetWidth(100);
            _guiObject.AddAnimation();
        }

        internal override void Update()
        {
            
        }

        internal override void AddGraphics(IGuiObject graphics)
        {
            _guiObject = graphics;
        }

        internal override IGuiObject GetGraphics()
        {
            return _guiObject;
        }

        internal override void Destroy()
        {
            _guiObject.Destroy();
        }

        internal override bool IsAlive()
        {
            throw new NotImplementedException();
        }

        public void Draw()
        {
            
        }

        public void Move(double x, double y)
        {
            _guiObject.SetPosition(x,y);
        }
    }
}
