using Grpc.Core;
using Grpc.Net.Client;
using BankServer.utils;
using System.Timers;
using System.Threading.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Globalization;

namespace BankServer.domain
{

    public class SlotTimer {
        private static System.Timers.Timer? _clock;
        IUpdatable _updatable;
        uint _slotDuration;
        


        public SlotTimer(IUpdatable updatable, uint slotDuration, string initialTime) {
            //Console.WriteLine("Criar o sloTimet");
            DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var span = dateTime - DateTime.Now;
            _slotDuration = slotDuration;
            _clock = new System.Timers.Timer() { Interval = _slotDuration, AutoReset = false };
            _updatable = updatable;
            

        }
        /* AFonso pintarolas*/
        public void Execute() {
            if (_clock == null) Environment.Exit(-1);
            _clock.Elapsed += new ElapsedEventHandler(onTimedEvent);
            _clock.Start();
        }

        private void onTimedEvent(object? source, ElapsedEventArgs e) {
            _updatable.Update();
        }
    }

}
