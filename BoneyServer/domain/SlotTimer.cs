using Grpc.Core;
using Grpc.Net.Client;
using System.Timers;
using System.Threading.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;
using BoneyServer.domain;

namespace BoneyServer.utils
{

    public class SlotTimer
    {
        private static System.Timers.Timer? _clock;
        IUpdatable _updatable;
        uint _slotDuration;



        public SlotTimer(IUpdatable updatable, uint slotDuration, string initialTime)
        {
            //Console.WriteLine("Criar o sloTimet");
            DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var span = dateTime - DateTime.Now;
            //if (span.TotalMilliseconds < 0) throw new Exception("The starting time in configuration file must be after the current time.");           DECOMENT WHEN NOT DEBUGGING!!!!!
            _clock = new System.Timers.Timer() { Interval = 1/*span.TotalMilliseconds*/, AutoReset = false };
            _updatable = updatable;
            _slotDuration = slotDuration;
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

            

        }

        public void Execute()
        {
            if (_clock == null) Environment.Exit(-1);
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
                action();
            }
        }

        
    }

}