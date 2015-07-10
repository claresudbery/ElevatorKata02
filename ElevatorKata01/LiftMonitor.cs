using System;

namespace ElevatorKata01
{
    public class LiftMonitor : ILiftMonitor
    {
        public void OnNext(LiftStatus value)
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }
    }
}