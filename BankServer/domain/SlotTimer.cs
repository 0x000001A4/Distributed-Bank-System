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
        private static System.Timers.Timer _clock;
        IUpdateState _updatable;
        uint _slotDuration;
        


        public SlotTimer(IUpdateState updatable, uint slotDuration, string initialTime) {
            //Console.WriteLine("Criar o sloTimet");
            DateTime dateTime = DateTime.ParseExact(initialTime, "HH:mm:ss",
                                        CultureInfo.InvariantCulture);
            var span = dateTime - DateTime.Now;
            _clock = new System.Timers.Timer() { Interval = _slotDuration, AutoReset = false };
            _updatable = updatable;
            _slotDuration = slotDuration;
            

        }

        public void execute() {
            _clock.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _clock.Interval = _slotDuration;
            _clock.Start();
        }

        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            _updatable.update();
        }
    }

}
