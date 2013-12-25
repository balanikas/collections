using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Collections
{
    interface IGuiObject
    {

        void AddParent(UIElementCollection parent);

        void SetHeight(int value);

        void SetWidth(int value);

        void Init();

        void Destroy();

        void SetColor(Color color);

        void SetPosition(double x, double y);

        void Move(double x, double y);

        Rect GetBounds();

        void AddAnimation();
    }
}
