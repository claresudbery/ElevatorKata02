namespace ElevatorKata02
{
    public interface ILiftEvent
    {
        //Direction Direction { get; set; }
        int Floor { get; set; }

        void OnNext(ObservableLift lift);
    }

    public class LiftEngineUpwardsEvent : ILiftEvent
    {
        //public Direction Direction { get; set; }
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.ArrivedAtFloorOnTheWayUp(Floor);
        }
    }

    public class LiftEngineDownwardsEvent : ILiftEvent
    {
        //public Direction Direction { get; set; }
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.ArrivedAtFloorOnTheWayDown(Floor);
        }
    }

    public class LiftMoveRequest : ILiftEvent
    {
        //public Direction Direction { get; set; }
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.Move(Floor);
        }
    }

    public class LiftCall : ILiftEvent
    {
        //public Direction Direction { get; set; }
        public int Floor { get; set; }

        public void OnNext(ObservableLift lift)
        {
            lift.Call(Floor);
        }
    }
}