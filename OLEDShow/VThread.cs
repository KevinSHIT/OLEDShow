using System.Threading;

namespace OLEDShow
{
    public class VThread
    {
        private Thread _thread;
        private ThreadStart _act;

        public VThread(ThreadStart act)
        {
            _act = act;
        }

        public void Start(bool abord = true)
        {
            if (abord)
                Stop();

            _thread = new Thread(_act);
            _thread.Start();
        }

        public void Stop()
        {
            try
            {
                _thread.Abort();
            }
            catch { }
        }

    }
}