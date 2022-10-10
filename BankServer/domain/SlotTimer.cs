using Grpc.Core;
using Grpc.Net.Client;
using BankServer.utils;
using System.Timers;
using System.Threading.Channels;
using System.Security.Cryptography.X509Certificates;






namespace BankServer.domain
{

    internal class SlotTimer
    {
        private static System.Timers.Timer _clock;
        IUpdateState _updatable;
        int _slotDuration;
        String _intialTime;


        public SlotTimer(IUpdateState updatable,int slotDuration, string intialTime)
        {
            //Console.WriteLine("Criar o sloTimet");
            _clock = new System.Timers.Timer();
            _updatable = updatable;
            _slotDuration = slotDuration;
            _intialTime = intialTime;

        }

        public void execute()
        {

           // Console.WriteLine("Im in slot: LALALALALALAALAL1111111111111");
            _clock.Elapsed += new ElapsedEventHandler(OnTimedEvent);
            _clock.Interval = _slotDuration;
            _clock.Start();
           


        }

        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {

            //Console.WriteLine("Im in slot: LALALALALALAALAL2222222222222");
            _updatable.update();

            Console.WriteLine("Im in slot: LALALALALALAALAL");

        }
    }

}
