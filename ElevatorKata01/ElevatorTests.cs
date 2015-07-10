using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace ElevatorKata01
{
    [TestFixture]
    public class ElevatorTests : ILiftMonitor
    {
        private List<Floor> _floorsVisited = new List<Floor>();
        private List<LiftStatus> _liftStatuses = new List<LiftStatus>();

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
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Move(ThirdFloor);
            Thread.Sleep(50);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
        }

        [Test]
        public void Test02_When_person_in_lift_enters_a_lower_floor_number_then_lift_starts_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(FirstFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Move(GroundFloor);
            Thread.Sleep(50);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Down));
        }

        [Test]
        public void Test03_When_person_in_lift_enters_a_floor_number_then_lift_notifies_its_current_location()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Move(ThirdFloor);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test04_When_person_in_lift_enters_a_floor_number_then_lift_arrives_at_next_floor_after_one_second()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Move(FirstFloor);

            // Assert
            Thread.Sleep(1050);
            Assert.That(_liftStatuses.Count, Is.EqualTo(2));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(FirstFloor));
        }

        [Test]
        public void Test05_When_lift_arrives_at_new_floor_after_person_in_lift_makes_request_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Move(FirstFloor);

            // Assert
            Thread.Sleep(1050);
            Assert.That(_liftStatuses.Count, Is.EqualTo(2));
            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.None));
        }

        [Test]
        public void Test06_When_person_calls_lift_to_higher_floor_then_lift_starts_moving_upwards()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Call(ThirdFloor);
            Thread.Sleep(50);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Up));
        }

        [Test]
        public void Test07_When_person_calls_lift_to_lower_floor_then_lift_starts_moving_downwards()
        {
            // Arrange
            var theLift = new ObservableLift(FirstFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Call(GroundFloor);
            Thread.Sleep(50);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentDirection, Is.EqualTo(Direction.Down));
        }

        [Test]
        public void Test08_When_person_calls_lift_then_lift_notifies_its_current_location()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Call(ThirdFloor);

            // Assert
            Assert.That(_liftStatuses.Count, Is.EqualTo(1));
            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(GroundFloor));
        }

        [Test]
        public void Test09_When_person_calls_lift_to_new_floor_then_lift_arrives_at_next_floor_after_floor_interval()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Call(FirstFloor);

            // Assert
            Thread.Sleep(TimeConstants.FloorInterval + 50);
            Assert.That(_liftStatuses.Count, Is.EqualTo(2));
            Assert.That(_liftStatuses[1].CurrentFloor, Is.EqualTo(FirstFloor));
        }

        [Test]
        public void Test10_When_lift_arrives_at_new_floor_after_person_outside_lift_calls_it_then_lift_stops_moving()
        {
            // Arrange
            var theLift = new ObservableLift(GroundFloor);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.Call(FirstFloor);

            // Assert
            Thread.Sleep(1050);
            Assert.That(_liftStatuses.Count, Is.EqualTo(2));
            Assert.That(_liftStatuses[1].CurrentDirection, Is.EqualTo(Direction.None));
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
    }
}