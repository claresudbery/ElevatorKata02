using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ElevatorKata02
{
    public class ObservableLift : IObservable<LiftStatus>, IDisposable, IObserver<ILiftEvent>
    {
        private readonly List<IObserver<LiftStatus>> _observers = new List<IObserver<LiftStatus>>();
        private int _currentFloor;
        private Direction _currentDirection;
        private readonly List<int> _goingUp = new List<int>();
        private readonly List<int> _goingDown = new List<int>();
        private ILiftEventGenerator _liftEventGenerator;

        private const int TopFloor = 30;
        private const int BottomFloor = -10;

        public ObservableLift(
            int startingFloor,
            ILiftEventGenerator liftEventGenerator)
        {
            _currentFloor = startingFloor;
            _currentDirection = Direction.None;

            _liftEventGenerator = liftEventGenerator;
            _liftEventGenerator.LiftSubscribe(this);
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
        }

        public void Call(int destinationFloor)
        {
            Move(destinationFloor);
        }

        private void MoveUpwards()
        {
            _currentDirection = Direction.Up;
            _liftEventGenerator.StartMovingUpwards(_currentFloor, LastUpFloor);
        }

        private void MoveDownwards()
        {
            _currentDirection = Direction.Down;
            _liftEventGenerator.StartMovingDownwards(_currentFloor, LastDownFloor);
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

        public void ArrivedAtFloorOnTheWayUp(int floor)
        {
            // TODO: What if we somehow find ourselves going up past the top floor??

            if (floor == NextUpFloor)
            {
                _currentFloor = floor;
                Stop();
            }
            else
            {
                _currentFloor = floor;
                NotifyObserversOfCurrentStatus();
            }
        }

        public void ArrivedAtFloorOnTheWayDown(int floor)
        {
            // TODO: What if we somehow find ourselves going down past the bottom floor??

            _currentFloor = floor;

            if (floor == NextDownFloor)
            {
                Stop();
            }

            NotifyObserversOfCurrentStatus();
        }

        private void Stop()
        {
            _currentDirection = Direction.None;
            _liftEventGenerator.Stop();
            NotifyObserversOfCurrentStatus();
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

        public void OnNext(ILiftEvent liftEvent)
        {
            liftEvent.OnNext(this);
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public void OnCompleted()
        {
            // Do nothing
        }
    }
}