using System;
using System.Reflection;
using System.Windows.Controls;
using System.ComponentModel;
using System.Diagnostics;

namespace Collections
{
    class Missile : GameObject
    {
        private IMissileBehavior m_missileBehavior;
        private IGuiObject m_guiObject;
        private BackgroundWorker m_bw = new BackgroundWorker();
        private Stopwatch m_watch;
        private int _progress = 0;
        private bool _isAlive = false;
        public Missile(IMissileBehavior behavior, IGuiObject guiObject)
        {

            m_guiObject = guiObject;
            m_missileBehavior = behavior;
            
            m_bw.WorkerReportsProgress = true;
            m_bw.WorkerSupportsCancellation = true;
            m_bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            m_bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            m_bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        internal override void Start()
        {
            m_guiObject.Init();
            _isAlive = true;
            if (m_bw.IsBusy != true)
            {
                m_bw.RunWorkerAsync();
                m_watch = new Stopwatch();
                m_watch.Start();
            }
           
        }

        internal override void Update()
        {
            m_guiObject.Move(1, _progress);
        }

        internal override void AddGraphics(IGuiObject graphics)
        {
            throw new NotImplementedException();
        }

        internal override IGuiObject GetGraphics()
        {
            return m_guiObject;
        }

        internal override void Destroy()
        {
            m_guiObject.Destroy();
        }

        internal override bool IsAlive()
        {

            return _isAlive;
        }

        public void Stop()
        {

        }


        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

           

            Type generic = m_missileBehavior.GetWeaponType();
            Type specific = generic.MakeGenericType(m_missileBehavior.GetAmmoType());
            ConstructorInfo ci = specific.GetConstructor(Type.EmptyTypes);
            object o = ci.Invoke(new object[] {});
            
            for (int i = 1; (i <= 1000000); i++)
            {
                if ((worker.CancellationPending == true))
                {
                    e.Cancel = true;
                    break;
                }
                else
                {
                    m_missileBehavior.Update(o);
                    if (i % 10000 == 0)
                    {
                        worker.ReportProgress((i / 10000));
                    }

                }
            }
        }

        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true))
            {
                m_guiObject.Destroy();
                
            }

            else if (e.Error != null)
            {
                m_guiObject.Destroy();
            }

            else
            {
                m_guiObject.Destroy();
                m_watch.Stop();
                Debug.WriteLine("ms: " + m_watch.ElapsedMilliseconds);
            }

            _isAlive = false;
        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            _progress = e.ProgressPercentage;
            //m_guiObject.Move(1, e.ProgressPercentage);
        }
    }
}
