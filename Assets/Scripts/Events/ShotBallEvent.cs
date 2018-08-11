public partial class Events
{
    public delegate void ShotBallAction();

    public static event ShotBallAction ShotBallEvent;

    public partial class SendEvent
    {
        public static void ShotBall()
        {
            if (ShotBallEvent != null)
            {
                ShotBallEvent();
            }
        }
    }
}