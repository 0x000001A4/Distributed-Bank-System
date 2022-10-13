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
            //DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
            //                            CultureInfo.InvariantCulture);
            //var span = dateTime - DateTime.Now;
            //if (span.TotalMilliseconds < 0) throw new Exception("The starting time in configuration file must be after the current time.");
            _clock = new System.Timers.Timer() { Interval = slotDuration/*span.TotalMilliseconds*/, AutoReset = false };
            _updatable = updatable;
            _slotDuration = slotDuration;


        }

        public void Execute()
        {
            if (_clock == null) Environment.Exit(-1);
            _clock.Elapsed += new ElapsedEventHandler(onTimedEvent);
            _clock.Interval = _slotDuration;
            _clock.Start();
        }

        private void onTimedEvent(object? source, ElapsedEventArgs e)
        {
            _updatable.Update();
        }
    }

}