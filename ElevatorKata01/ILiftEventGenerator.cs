using System;

namespace ElevatorKata02
{
    public interface ILiftEventGenerator : IObservable<ILiftEvent>
    {
        void StartMovingUpwards(int currentFloor, int lastUpFloor);
        void StartMovingDownwards(int currentFloor, int lastDownFloor);
        void Stop();
        IDisposable LiftSubscribe(ObservableLift observer);
    }
}