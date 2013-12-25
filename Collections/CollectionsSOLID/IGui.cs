using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollectionsSOLID
{
    public interface IGui
    {
        string Id { get; set; }
        void Init();
        void Update(Message message);
        void Destroy();
    }
}
