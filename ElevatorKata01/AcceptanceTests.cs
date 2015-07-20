using System;
using System.Collections.Generic;
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
        public void Playing_with_test_scheduler()
        {
            var sched = new TestScheduler();
            var subject = sched.CreateColdObservable(
                sched.OnNextAt(100, "m"), // Provides "m" at 100 ms
                sched.OnNextAt(200, "o"), // Provides "o" at 200 ms
                sched.OnNextAt(300, "r"), // Provides "r" at 300 ms
                sched.OnNextAt(400, "k")  // Provides "k" at 400 ms
            );

            string seenValue = null;
            subject.Subscribe(value => seenValue = value);

            sched.AdvanceByMs(100);
            Assert.AreEqual("m", seenValue);

            sched.AdvanceByMs(100);
            Assert.AreEqual("o", seenValue);

            sched.AdvanceByMs(100);
            Assert.AreEqual("r", seenValue);

            sched.AdvanceByMs(100);
            Assert.AreEqual("k", seenValue);
        }

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
            testScheduler.AdvanceBy(1000000);

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
            testScheduler.AdvanceBy(1000000);

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