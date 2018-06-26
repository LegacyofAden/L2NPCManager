using System;
using System.Threading;
using System.Windows.Threading;

namespace GraphicsLibrary.Workers
{
    public abstract class ProgressWorker
    {
        public delegate void CompleteEvent(bool cancelled, Exception error);
        public delegate void PollEvent();
        public event CompleteEvent OnComplete;

        private Thread thread;
        protected Dispatcher dispatch;
        private Exception error;
        protected volatile bool cancelPending;
        private int progress;
        private object progress_lock;
        private ProgressPoller poller;

        private volatile bool is_active;
        public bool IsActive {get {return is_active;}}


        public ProgressWorker() {
            progress_lock = new object();
            dispatch = Dispatcher.CurrentDispatcher;
        }

        //=============================

        public void Run(PollEvent poll_event) {
            if (poll_event != null) {
                poller = new ProgressPoller(this);
                poller.Start(poll_event);
            }
            //
            cancelPending = false;
            is_active = true;
            thread = new Thread(thread_process);
            thread.Start();
        }

        public void Cancel() {
            cancelPending = true;
        }

        public int GetProgress() {
            lock (progress_lock) {return progress;}
        }

        //-----------------------------

        protected abstract void Process();

        private void thread_process() {
            try {Process();}
            catch (Exception error) {
                this.error = error;
            }
            finally {
                invokeComplete();
                if (poller != null) poller.Stop();
                is_active = false;
            }
        }

        //-----------------------------

        protected void setProgress(int value) {
            lock (progress_lock) {progress = value;}
        }

        private void invokeComplete() {
            if (OnComplete != null) dispatch.BeginInvoke(OnComplete, cancelPending, error);
        }
    }
}