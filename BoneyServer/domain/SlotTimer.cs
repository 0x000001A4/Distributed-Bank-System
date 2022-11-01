using System.Timers;
using System.Globalization;
using BoneyServer.domain;

namespace BoneyServer.utils
{

    public class SlotTimer
    {
        private static System.Timers.Timer? _clock;
        IUpdatable _updatable;
        uint _slotDuration;
        uint _maxTicks;



        public SlotTimer(IUpdatable updatable, uint slotDuration, string initialTime, uint maxTicks)
        {
            DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var span = dateTime - DateTime.Now;
            //if (span.TotalMilliseconds < 0) throw new Exception("The starting time in configuration file must be after the current time.");     //      DECOMENT WHEN NOT DEBUGGING!!!!!
            _clock = new System.Timers.Timer() { Interval = 1/*span.TotalMilliseconds*/, AutoReset = false };
            _updatable = updatable;
            _slotDuration = slotDuration;
            _maxTicks = maxTicks;
        }

        public void Execute()
        {
            if (_clock == null) throw new Exception();
            _clock.Elapsed += new ElapsedEventHandler(onTimedEvent);
            _clock.Start();
        }

        private void onTimedEvent(object? source, ElapsedEventArgs e)
        {
            Task res = RunInBackground(TimeSpan.FromMilliseconds(_slotDuration), _updatable);
        }

        async Task<bool> RunInBackground(TimeSpan timeSpan, IUpdatable updatable)
        {
            var periodicTimer = new PeriodicTimer(timeSpan);
            uint currentTick = 1;
            while (await periodicTimer.WaitForNextTickAsync())
            {
                Logger.LogEvent("current tick: " + currentTick);
                if (currentTick >= _maxTicks)
                {
                    Logger.LogEvent("Reached maximum ticks");
                    updatable.Stop();
                    return true;
                }
                Logger.LogEvent("Tick");
                updatable.Update(currentTick);
                currentTick++;
            }
            return true;
        }


    }
}