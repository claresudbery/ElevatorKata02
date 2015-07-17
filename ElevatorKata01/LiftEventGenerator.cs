using System;
using System.Reactive.Linq;

namespace ElevatorKata02
{
    public class LiftEventGenerator : ILiftEventGenerator, IDisposable
    {
        private IDisposable _liftEngineSubscription = null;
        private ObservableLift _lift = null;

        public IDisposable Subscribe(IObserver<ILiftEvent> observer)
        {
            throw new NotImplementedException();
        }

        public IDisposable LiftSubscribe(ObservableLift observer)
        {
            _lift = observer;
            return this;
        }

        public void StartMovingUpwards(int currentFloor, int lastUpFloor)
        {
            IObservable<ILiftEvent> liftEngine = Observable.Generate
                (
                    UpEvent(currentFloor),
                    liftStatus => liftStatus.Floor <= lastUpFloor,
                    liftStatus => UpEvent(liftStatus.Floor + 1), // iterator
                    liftStatus => UpEvent(liftStatus.Floor + 1), // actual value? Shouldn't use same val as iterator?
                    i => TimeSpan.FromMilliseconds(TimeConstants.FloorInterval)
                );

            _liftEngineSubscription = liftEngine.Subscribe
                (
                    OnNextLiftEvent
                );
        }

        public void StartMovingDownwards(int currentFloor, int lastDownFloor)
        {
            IObservable<ILiftEvent> liftEngine = Observable.Generate
                (
                    DownEvent(currentFloor),
                    liftStatus => liftStatus.Floor >= lastDownFloor,
                    liftStatus => DownEvent(liftStatus.Floor - 1), // iterator
                    liftStatus => DownEvent(liftStatus.Floor - 1), // actual value? Shouldn't use same val as iterator?
                    i => TimeSpan.FromMilliseconds(TimeConstants.FloorInterval)
                );

            _liftEngineSubscription = liftEngine.Subscribe
                (
                    OnNextLiftEvent
                );
        }

        private LiftEngineUpwardsEvent UpEvent(int floor)
        {
            return new LiftEngineUpwardsEvent
                {
                    Floor = floor
                };
        }

        private LiftEngineDownwardsEvent DownEvent(int floor)
        {
            return new LiftEngineDownwardsEvent
                {
                    Floor = floor
                };
        }

        public void OnNextLiftEvent(ILiftEvent liftEvent)
        {
            if (_lift != null)
            {
                liftEvent.OnNext(_lift);
            }
            else
            {
                throw new Exception("Lift event generator received a lift event, but LiftSubscribe was never called, so no lift object has been initialised.");
            }
        }

        public void Stop()
        {
            _liftEngineSubscription.Dispose();
        }

        public void Dispose()
        {
            _lift.OnCompleted();
        }
    }
}