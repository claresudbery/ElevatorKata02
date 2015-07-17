namespace ElevatorKata02
{
    public class LiftEngineDownwardsEvent : ILiftEvent
    {
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.ArrivedAtFloorOnTheWayDown(Floor);
        }
    }
}