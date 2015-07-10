using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ElevatorKata02
{
    /// <summary>
    /// Yeah, ok, I cheated a bit: I got thinking about it while cycling home, and ended up half-coding a whole solution in my head.
    /// The results are noted down here, but none of this code will be used until / unless it is called for to make tests pass.
    /// </summary>
    public class LiftSpares : IObservable<LiftStatus>, IDisposable
    {
        private IObservable<int> _liftEngine = null;
        private IDisposable _liftEngineSubscription = null;
        private List<IObserver<LiftStatus>> _observers = new List<IObserver<LiftStatus>>();

        private int _currentFloor;
        private Direction _currentDirection;
        private readonly List<int> _goingUp = new List<int>();
        private readonly List<int> _goingDown = new List<int>();

        public LiftSpares(
            int startingFloor,
            IObservable<int> internalControlPanel,
            IObservable<LiftCall> externalControlPanel,
            IObservable<ILiftEvent> eventGenerator)
        {
            _currentFloor = startingFloor;
            _currentDirection = Direction.None;

            internalControlPanel.Subscribe
                (
                    Move
                );

            externalControlPanel.Subscribe
                (
                    Call
                );

            throw new NotImplementedException();
        }

        public IDisposable Subscribe(IObserver<LiftStatus> observer)
        {
            _observers.Add(observer);
            return this;
        }

        public void Dispose()
        {
            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        private void NotifyObserversOfCurrentStatus()
        {
            foreach (var observer in _observers)
            {
                observer.OnNext
                    (
                        new LiftStatus
                            {
                                CurrentDirection = _currentDirection,
                                CurrentFloor = _currentFloor
                            }
                    );
            }
        }

        private void ArrivedAtFloorOnTheWayUp(int floor)
        {
            // TODO: What if we somehow find ourselves going up past the top floor??
            if (floor == NextUpFloor)
            {
                Stop();
                Wait(); 
            }
            _currentFloor = floor;
            throw new NotImplementedException();
        }

        private void ArrivedAtFloorOnTheWayDown(int floor)
        {
            // TODO: What if we somehow find ourselves going up past the lowest floor there is??
            throw new NotImplementedException();
        }

        private void Stop()
        {
            NotifyObserversOfCurrentStatus();
            _liftEngineSubscription.Dispose();
            throw new NotImplementedException();
        }

        private void Wait()
        {
            throw new NotImplementedException();
        }

        private void CallUp(int destinationFloor)
        {
            _goingUp.Add(destinationFloor);
            if (NotMoving)
            {
                MoveUpwards();
            }

            throw new NotImplementedException();
        }

        private void CallDown(int destinationFloor)
        {
            _goingDown.Add(destinationFloor);
            if (NotMoving)
            {
                MoveDownwards();
            }

            throw new NotImplementedException();
        }

        public void Call(LiftCall call)
        {
            throw new NotImplementedException();
        }

        public void Move(int destinationFloor)
        {
            if (destinationFloor > _currentFloor)
            {
                _goingUp.Add(destinationFloor);
                MoveUpwards();
            }
            else
            {
                _goingDown.Add(destinationFloor);
                MoveDownwards();
            }
            throw new NotImplementedException();
        }

        private void MoveDownwards()
        {
            NotifyObserversOfCurrentStatus();
            throw new NotImplementedException();
        }

        private void MoveUpwards()
        {
            if (_goingUp.Any())
            {
                _liftEngine = Observable.Generate
                    (
                        0,
                        i => i < LastUpFloor,
                        i => i + 1, // iterator
                        i => i + 1, // actual value? Shouldn't use same val as iterator?
                        i => TimeSpan.FromMilliseconds(TimeConstants.FloorInterval)
                    );

                _currentDirection = Direction.Up;

                _liftEngineSubscription = _liftEngine.Subscribe
                    (
                        ArrivedAtFloorOnTheWayUp
                    );

                NotifyObserversOfCurrentStatus();
            }
            else
            {
                MoveDownwards();
            }

            throw new NotImplementedException();
        }

        public bool NotMoving
        {
            get
            {
                return (_currentDirection == Direction.None);
            }
        }

        public bool NoUpFloors
        {
            get
            {
                return !_goingUp.Any();
            }
        }

        public bool NoDownFloors
        {
            get
            {
                return !_goingDown.Any();
            }
        }

        private void CheckForUpFloors(string itemRequested)
        {
            if (NoUpFloors)
            {
                throw new Exception(itemRequested + " was requested, but GoingUp is empty");
            }
        }

        private void CheckForDownFloors(string itemRequested)
        {
            if (NoDownFloors)
            {
                throw new Exception(itemRequested + " was requested, but GoingDown is empty");
            }
        }

        private int LastUpFloor
        {
            get
            {
                CheckForUpFloors("LastUpFloor");
                return _goingUp.Max();
            }
        }

        public int NextUpFloor
        {
            get
            {
                CheckForUpFloors("NextUpFloor");
                return _goingUp.Where(i => i > _currentFloor).Min();
            }
        }

        private int LastDownFloor
        {
            get
            {
                CheckForDownFloors("LastDownFloor");
                return _goingDown.Min();
            }
        }

        public int NextDownFloor
        {
            get
            {
                CheckForDownFloors("NextDownFloor");
                return _goingDown.Where(i => i < _currentFloor).Max();
            }
        }

        private void StopWaitingUp()
        {
            if (_currentFloor == LastUpFloor)
            {
                MoveDownwards();
            }
            else
            {
                MoveUpwards();
            }
            throw new NotImplementedException();
        }

        private void StopWaitingDown()
        {
            throw new NotImplementedException();
        }
    }
}