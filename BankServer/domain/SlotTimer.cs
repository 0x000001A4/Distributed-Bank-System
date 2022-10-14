using System.Timers;
using System.Globalization;
using BankServer.domain;

namespace BankServer.utils
{

    public class SlotTimer
    {
        private static System.Timers.Timer? _clock;
        IUpdatable _updatable;
        uint _slotDuration;



        public SlotTimer(IUpdatable updatable, uint slotDuration, string initialTime)
        {
            DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var span = dateTime - DateTime.Now;
            //if (span.TotalMilliseconds < 0) throw new Exception("The starting time in configuration file must be after the current time.");           DECOMENT WHEN NOT DEBUGGING!!!!!
            _clock = new System.Timers.Timer() { Interval = 1/*span.TotalMilliseconds*/, AutoReset = false };
            _updatable = updatable;
            _slotDuration = slotDuration;
        }

        public void Execute()
        {
            if (_clock == null) throw new Exception();
            _clock.Elapsed += new ElapsedEventHandler(onTimedEvent);
            _clock.Start();
        }

        private void onTimedEvent(object? source, ElapsedEventArgs e)
        {
            Task res = RunInBackground(TimeSpan.FromMilliseconds(_slotDuration), () => _updatable.Update());
        }

        async Task RunInBackground(TimeSpan timeSpan, Action action)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            while (await periodicTimer.WaitForNextTickAsync())
            {
                Logger.LogEvent("Tick");
                action();
            }
        }


    }
}