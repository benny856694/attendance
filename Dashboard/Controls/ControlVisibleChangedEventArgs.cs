using System.Windows.Forms;

namespace Dashboard.Controls
{
    public class ControlVisibleChangedEventArgs
    {
        public ControlVisibleChangedEventArgs(Control c, VisibleState oldVisibleState, VisibleState newVisibleState)
        {
            Control = c;
            OldVisibleState = oldVisibleState;
            NewVisibleState = newVisibleState;
        }

        public Control Control { get; }
        public VisibleState OldVisibleState { get; }
        public VisibleState NewVisibleState { get; }
    }
}