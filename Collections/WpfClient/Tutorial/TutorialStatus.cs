using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfClient.Tutorial
{
    class TutorialStatus
    {
        private const string FileName = "06B6212A-350C-4622-9A05-471A7F5188DF";

        public static bool HasRun
        {
            get
            {
                if (File.Exists(Path.Combine(Environment.CurrentDirectory, FileName)))
                {
                    return true;
                }
                return false;
            }

            set
            {
                if (value)
                {
                    File.Create(Path.Combine(Environment.CurrentDirectory, FileName));
                }
                else
                {
                    File.Delete(Path.Combine(Environment.CurrentDirectory, FileName));
                }
                
            }
        } 
    }
}
