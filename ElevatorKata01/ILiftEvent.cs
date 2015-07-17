namespace ElevatorKata02
{
    public interface ILiftEvent
    {
        int Floor { get; set; }

        void OnNext(ObservableLift lift);
    }
}