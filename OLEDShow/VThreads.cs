using System.Threading;

namespace OLEDShow
{
    public class VThreads
    {
        private Thread _thread;
        private ThreadStart _act;

        public VThreads(ThreadStart act)
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