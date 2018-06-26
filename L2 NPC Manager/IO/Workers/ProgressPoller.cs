using System.Windows.Threading;

namespace L2NPCManager.IO.Workers
{
    public class ProgressPoller
    {
        public delegate void TickEvent();
        public event TickEvent OnTick;

        private ProgressWorker.PollEvent poll_event;
        private Dispatcher dispatch;
        private ProgressWorker worker;
        private bool is_running;


        public ProgressPoller(ProgressWorker worker) {
            this.worker = worker;
            dispatch = Dispatcher.CurrentDispatcher;
            OnTick += ProgressPoller_OnTick;
        }

        //=============================

        public void Start(ProgressWorker.PollEvent poll_event) {
            this.poll_event = poll_event;
            is_running = true;
            schedule_tick();
        }

        public void Stop() {
            is_running = false;
        }

        //-----------------------------

        private void ProgressPoller_OnTick() {
            if (poll_event != null) poll_event.Invoke();
            if (is_running) schedule_tick();
        }

        private void schedule_tick() {
            dispatch.BeginInvoke(OnTick, DispatcherPriority.Background);
        }
    }
}