using Grpc.Core;
using Grpc.Net.Client;
using BankServer.utils;
using System.Timers;
using System.Threading.Channels;
using System.Security.Cryptography.X509Certificates;

namespace BankServer.domain
{

    public class SlotTimer {
        private static System.Timers.Timer _clock;
        IUpdateState _updatable;
        uint _slotDuration;
        String _intialTime;


        public SlotTimer(IUpdateState updatable, uint slotDuration, string intialTime) {
            //Console.WriteLine("Criar o sloTimet");
            _clock = new System.Timers.Timer();
            _updatable = updatable;
            _slotDuration = slotDuration;
            _intialTime = intialTime;

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
