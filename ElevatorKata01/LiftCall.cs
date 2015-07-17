namespace ElevatorKata02
{
    public class LiftCall : ILiftEvent
    {
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.Call(Floor);
        }
    }
}