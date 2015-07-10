namespace ElevatorKata01
{
    public class Lift
    {
        private Floor _currentFloor;

        public Lift(Floor initialFloor)
        {
            _currentFloor = initialFloor;
        }

        public Floor Call(Floor source)
        {
            return Move(source);
        }

        public Floor Move(Floor destination)
        {
            _currentFloor = destination;

            return _currentFloor;
        }
    }
}