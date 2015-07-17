namespace ElevatorKata02
{
    public class LiftEngineUpwardsEvent : ILiftEvent
    {
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.ArrivedAtFloorOnTheWayUp(Floor);
        }
    }
}