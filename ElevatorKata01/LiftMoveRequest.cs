namespace ElevatorKata02
{
    public class LiftMoveRequest : ILiftEvent
    {
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.Move(Floor);
        }
    }
}