using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace ElevatorKata02
{
    [TestFixture]
    public class ElevatorTests : ILiftMonitor, ILiftEventGenerator, IDisposable
    {
        private List<Floor> _floorsVisited = new List<Floor>();
        private List<LiftStatus> _liftStatuses = new List<LiftStatus>();
        private List<LiftStatus> _liftEngineEvents = new List<LiftStatus>(); 
        IObserver<ILiftEvent> _currentObserver;

        private const int FloorIsIrrelevant = -1;
        private const int GroundFloor = 0;
        private const int FirstFloor = 1;
        private const int SecondFloor = 2;
        private const int ThirdFloor = 3;
        private const int FourthFloor = 4;
        private const int FifthFloor = 5;
        private const int SixthFloor = 6;

        [Test]
        public void Test01_When_person_in_lift_enters_a_higher_floor_number_then_lift_starts_moving_upwards()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test02_When_person_in_lift_enters_a_lower_floor_number_then_lift_starts_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(ThirdFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = FirstFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Down));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(ThirdFloor));
        }

        [Test]
        public void Test01_When_person_in_lift_enters_a_floor_number_then_lift_notifies_direction_and_location_for_every_floor_it_passes()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(4));

            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));

            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(FirstFloor));

            Assert.That(_liftStatuses[2].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[2].CurrentFloor, Is.EqualTo(SecondFloor));

            Assert.That(_liftStatuses[3].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftStatuses[3].CurrentFloor, Is.EqualTo(ThirdFloor));
        }

        [Test]
        public void Test01_When_person_in_lift_enters_a_higher_floor_number_then_lift_engine_is_asked_to_move_upwards_and_then_stopped_when_it_reaches_destination()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.EqualTo(2));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(GroundFloor));

            Assert.That(_liftEngineEvents[1].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[1].CurrentFloor, Is.EqualTo(FloorIsIrrelevant));
        }

        [Test]
        public void Test03_When_person_in_lift_enters_a_floor_number_then_lift_notifies_its_current_location()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.GreaterThan(0));

            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test04_When_person_in_lift_enters_a_floor_number_then_lift_goes_to_that_floor()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FourthFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(5));

            Assert.That(_liftStatuses[4].CurrentFloor, Is.EqualTo(FourthFloor));
        }

        [Test]
        public void Test05_When_lift_arrives_at_new_floor_after_person_in_lift_makes_request_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(1));

            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentFloor, Is.EqualTo(FloorIsIrrelevant));
        }

        [Test]
        public void Test01_When_person_calls_lift_to_higher_floor_number_then_lift_starts_moving_upwards()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test02_When_person_calls_lift_to_lower_floor_number_then_lift_starts_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(ThirdFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = FirstFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Down));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(ThirdFloor));
        }

        [Test]
        public void Test01_When_person_calls_lift_to_floor_number_then_lift_notifies_direction_and_location_for_every_floor_it_passes()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(4));

            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));

            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(FirstFloor));

            Assert.That(_liftStatuses[2].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[2].CurrentFloor, Is.EqualTo(SecondFloor));

            Assert.That(_liftStatuses[3].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftStatuses[3].CurrentFloor, Is.EqualTo(ThirdFloor));
        }

        [Test]
        public void Test01_When_person_calls_lift_to_higher_floor_number_then_lift_engine_is_asked_to_move_upwards_and_then_stopped_when_it_reaches_destination()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.EqualTo(2));

            Assert.That(_liftEngineEvents[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(GroundFloor));

            Assert.That(_liftEngineEvents[1].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[1].CurrentFloor, Is.EqualTo(FloorIsIrrelevant));
        }

        [Test]
        public void Test03_When_person_calls_lift_to_floor_number_then_lift_notifies_its_current_location()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.GreaterThan(0));

            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test04_When_person_calls_lift_to_floor_number_then_lift_goes_to_that_floor()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FourthFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(5));

            Assert.That(_liftStatuses[4].CurrentFloor, Is.EqualTo(FourthFloor));
        }

        [Test]
        public void Test05_When_lift_arrives_at_new_floor_after_person_calls_lift_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(1));

            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentFloor, Is.EqualTo(FloorIsIrrelevant));
        }

        //[Test]
        //public void Given_lift_is_on_groundfloor_and_personA_is_on_the_firstfloor_and_personB_is_on_the_second_floor_and_both_people_want_to_ascend_When_lift_is_called_then_it_will_fetch_personA_first()
        //{
        //    // Arrange
        //    var theLift = new ObservableLift();
        //    theLift.Subscribe(this);
        //    theLift.CurrentFloor = Floor.Ground;

        //    // Act
        //    theLift.Move(Floor.Second, Floor.Third);
        //    theLift.Move(Floor.First, Floor.Third);

        //    // Assert
        //    Assert.That(_floorsVisited[0], Is.EqualTo(Floor.Ground));
        //    Assert.That(_floorsVisited[1], Is.EqualTo(Floor.First));
        //    Assert.That(_floorsVisited[2], Is.EqualTo(Floor.Second));
        //    Assert.That(_floorsVisited[3], Is.EqualTo(Floor.Third));

        //    // Clean up
        //    theLift.Dispose();
        //}

        public void OnNext(LiftStatus currentLiftStatus)
        {
            _liftStatuses.Add(currentLiftStatus);
        }

        public void OnError(Exception error)
        {
            // Do nothing
        }

        public void OnCompleted()
        {
            // Do nothing
        }

        public IDisposable Subscribe(IObserver<ILiftEvent> observer)
        {
            throw new NotImplementedException();
        }

        public IDisposable LiftSubscribe(ObservableLift observer)
        {
            _currentObserver = observer;
            return this;
        }

        public void StartMovingUpwards(int currentFloor, int lastUpFloor)
        {
            AddLiftEvent(Direction.Up, currentFloor);
        }

        public void StartMovingDownwards(int currentFloor, int lastDownFloor)
        {
            AddLiftEvent(Direction.Down, currentFloor);
        }

        public void Stop()
        {
            AddLiftEvent(Direction.None, FloorIsIrrelevant);
        }

        private void AddLiftEvent(Direction whichDirection, int currentFloor)
        {
            _liftEngineEvents.Add(
                new LiftStatus
                {
                    CurrentDirection = whichDirection,
                    CurrentFloor = currentFloor
                });
        }

        public void Dispose()
        {
            _currentObserver.OnCompleted();
        }
    }
}