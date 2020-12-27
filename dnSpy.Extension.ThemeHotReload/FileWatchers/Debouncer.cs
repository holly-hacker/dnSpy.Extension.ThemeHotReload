using System;
using System.Diagnostics;
using System.Timers;

namespace HoLLy.dnSpyExtension.ThemeHotReload.FileWatchers
{
    public class Debouncer
    {
        private readonly TimeSpan _interval;
        private Timer? _timer;
        private Action? _action;

        public Debouncer(TimeSpan interval)
        {
            _interval = interval;
        }

        public void Trigger(Action a)
        {
            if (_timer is not null)
                ResetTimer(_timer);

            _action = a;

            _timer = new Timer(_interval.TotalMilliseconds) {AutoReset = false};
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs e)
        {
            Debug.Assert(_action is not null);
            _action!();
            ResetTimer((Timer) sender);
        }

        private void ResetTimer(Timer timer)
        {
            timer.Elapsed -= TimerOnElapsed;
            timer.Close();
        }
    }
}