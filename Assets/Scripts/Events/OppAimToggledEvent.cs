public partial class Events
{
    public delegate void OppAimToggledAction(bool on);

    public static event OppAimToggledAction OppAimToggledEvent;

    public partial class SendEvent
    {
        public static void OppAimToggled(bool on)
        {
            if (OppAimToggledEvent != null)
            {
                OppAimToggledEvent(on);
            }
        }
    }
}