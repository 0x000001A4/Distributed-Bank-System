using System.Diagnostics;

namespace BankClient.utils
{
    public class TimeoutTimer
    {
        private static int TIMEOUT = 5000;
        private int _maxWaiting;
        private Stopwatch _stopwatch;
        public TimeoutTimer(int timeMillis = -1)
        {
            _maxWaiting = timeMillis > 0 ? timeMillis : TIMEOUT;
            _stopwatch = new Stopwatch();
        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public bool TimedOut()
        {
            if (_stopwatch.ElapsedMilliseconds > _maxWaiting) {
                _stopwatch.Stop();
                return true; 
            }
            return false;
        }

        public static void SetTimeout(int millis)
        {
            Logger.LogEvent($"Timeout set to: {millis}");
            TIMEOUT = millis;
        }
    }
}
