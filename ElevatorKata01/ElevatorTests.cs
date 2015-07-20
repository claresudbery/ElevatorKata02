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
        private List<LiftEngineEvent> _liftEngineEvents = new List<LiftEngineEvent>(); 
        IObserver<ILiftEvent> _currentObserver;

        [Test]
        public void When_person_in_lift_enters_a_higher_floor_number_then_lift_is_told_to_start_moving_upwards()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));
        }

        [Test]
        public void When_person_in_lift_enters_a_lower_floor_number_then_lift_is_told_to_start_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.ThirdFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.FirstFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Down));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.ThirdFloor));
        }

        [Test]
        public void When_lift_moves_past_floors_then_lift_notifies_direction_and_location_for_every_floor_it_passes()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(4));

            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));

            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(LiftConstants.FirstFloor));

            Assert.That(_liftStatuses[2].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[2].CurrentFloor, Is.EqualTo(LiftConstants.SecondFloor));

            Assert.That(_liftStatuses[3].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftStatuses[3].CurrentFloor, Is.EqualTo(LiftConstants.ThirdFloor));
        }

        [Test]
        public void When_person_in_lift_enters_a_higher_floor_number_then_lift_engine_is_asked_to_move_upwards_and_then_stopped_when_it_reaches_destination()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.EqualTo(2));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));

            Assert.That(_liftEngineEvents[1].Direction, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[1].CurrentFloor, Is.EqualTo(LiftConstants.FloorIsIrrelevant));
        }

        [Test]
        public void When_person_in_lift_enters_a_floor_number_then_lift_is_told_to_move_to_that_floor()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].LastFloor, Is.EqualTo(LiftConstants.FourthFloor));
        }

        [Test]
        public void When_lift_arrives_at_new_floor_after_person_in_lift_makes_request_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftMoveRequest { Floor = LiftConstants.FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(1));

            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].Direction, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentFloor, Is.EqualTo(LiftConstants.FloorIsIrrelevant));
        }

        [Test]
        public void When_person_calls_lift_to_higher_floor_number_then_lift_is_told_to_start_moving_upwards()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));
        }

        [Test]
        public void When_person_calls_lift_to_lower_floor_number_then_lift_is_told_to_start_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.ThirdFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.FirstFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Down));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.ThirdFloor));
        }

        [Test]
        public void When_person_calls_lift_to_floor_number_then_lift_notifies_direction_and_location_for_every_floor_it_passes()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.GroundFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.FirstFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.SecondFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(4));

            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));

            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(LiftConstants.FirstFloor));

            Assert.That(_liftStatuses[2].CurrentDirection, Is.EqualTo(Direction.Up));
            Assert.That(_liftStatuses[2].CurrentFloor, Is.EqualTo(LiftConstants.SecondFloor));

            Assert.That(_liftStatuses[3].CurrentDirection, Is.EqualTo(Direction.None));
            Assert.That(_liftStatuses[3].CurrentFloor, Is.EqualTo(LiftConstants.ThirdFloor));
        }

        [Test]
        public void When_person_calls_lift_to_higher_floor_number_then_lift_engine_is_asked_to_move_upwards_and_then_stopped_when_it_reaches_destination()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.ThirdFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.ThirdFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.EqualTo(2));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));

            Assert.That(_liftEngineEvents[1].Direction, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[1].CurrentFloor, Is.EqualTo(LiftConstants.FloorIsIrrelevant));
        }

        [Test]
        public void When_person_calls_lift_to_floor_number_then_lift_is_moved_to_that_floor()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(0));

            Assert.That(_liftEngineEvents[0].Direction, Is.EqualTo(Direction.Up));
            Assert.That(_liftEngineEvents[0].LastFloor, Is.EqualTo(LiftConstants.FourthFloor));
        }

        [Test]
        public void When_lift_arrives_at_new_floor_after_person_calls_lift_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(LiftConstants.GroundFloor, this);
            _liftEngineEvents.Clear();
            theLift.Subscribe(this);

            // Act
            _currentObserver.OnNext(new LiftCall { Floor = LiftConstants.FourthFloor });
            _currentObserver.OnNext(new LiftEngineUpwardsEvent { Floor = LiftConstants.FourthFloor });

            // Assert
            Assert.That(_liftEngineEvents.Count, Is.GreaterThan(1));

            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].Direction, Is.EqualTo(Direction.None));
            Assert.That(_liftEngineEvents[_liftEngineEvents.Count - 1].CurrentFloor, Is.EqualTo(LiftConstants.FloorIsIrrelevant));
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
            AddLiftEvent(Direction.Up, currentFloor, lastUpFloor);
        }

        public void StartMovingDownwards(int currentFloor, int lastDownFloor)
        {
            AddLiftEvent(Direction.Down, currentFloor, lastDownFloor);
        }

        public void Stop()
        {
            AddLiftEvent(Direction.None, LiftConstants.FloorIsIrrelevant, LiftConstants.FloorIsIrrelevant);
        }

        private void AddLiftEvent(
            Direction whichDirection,
            int currentFloor,
            int lastFloor)
        {
            _liftEngineEvents.Add(
                new LiftEngineEvent
                {
                    Direction = whichDirection,
                    CurrentFloor = currentFloor,
                    LastFloor = lastFloor
                });
        }

        public void Dispose()
        {
            _currentObserver.OnCompleted();
        }
    }
}