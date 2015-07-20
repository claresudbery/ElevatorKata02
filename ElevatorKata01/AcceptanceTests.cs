using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using NUnit.Framework;
using ReactiveUI.Testing;

namespace ElevatorKata02
{
    [TestFixture]
    public class AcceptanceTests : ILiftMonitor
    {
        private List<LiftStatus> _liftStatuses = new List<LiftStatus>();

        [Test]
        public void When_person_in_lift_enters_a_floor_number_then_lift_notifies_its_current_location()
        {
            // Arrange
            var testScheduler = new TestScheduler();
            var theLiftEngine = new LiftEventGenerator(testScheduler);
            var theLift = new ObservableLift(LiftConstants.GroundFloor, theLiftEngine);
            _liftStatuses.Clear();
            theLift.Subscribe(this);
            
            // Act
            theLift.OnNext(new LiftMoveRequest { Floor = LiftConstants.FirstFloor });
            testScheduler.Start();
            testScheduler.AdvanceBy(5);

            // Assert
            Assert.That(_liftStatuses.Count, Is.GreaterThan(0));

            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));
        }

        [Test]
        public void When_person_in_lift_calls_lift_to_a_floor_then_lift_notifies_its_current_location()
        {
            // Arrange
            var testScheduler = new TestScheduler();
            var theLiftEngine = new LiftEventGenerator(testScheduler);
            var theLift = new ObservableLift(LiftConstants.GroundFloor, theLiftEngine);
            _liftStatuses.Clear();
            theLift.Subscribe(this);

            // Act
            theLift.OnNext(new LiftCall { Floor = LiftConstants.FirstFloor });
            testScheduler.Start();
            testScheduler.AdvanceBy(5);

            // Assert
            Assert.That(_liftStatuses.Count, Is.GreaterThan(0));

            Assert.That(_liftStatuses[0].CurrentFloor, Is.EqualTo(LiftConstants.GroundFloor));
        }

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