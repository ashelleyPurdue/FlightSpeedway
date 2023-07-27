using Godot;

namespace FlightSpeedway
{
    public partial class PauseMenu : Control
    {
        private Control _focusStart => GetNode<Control>("%Continue");

        public override void _Ready()
        {
            Visible = false;
        }

        public override void _Input(InputEvent ev)
        {
            if (InputService.PauseJustPressed(ev))
            {
                if (Visible)
                    Close();
                else
                    Open();
            }
        }

        public void Open()
        {
            if (AnotherMenuAlreadyOpen())
                return;

            Visible = true;
            GetTree().Paused = true;
            _focusStart.GrabFocus();
        }

        public void Close()
        {
            Visible = false;
            GetTree().Paused = false;
        }

        public void Retry()
        {
            Close();
            SignalBus.Instance.EmitLevelReset();
        }

        private bool AnotherMenuAlreadyOpen() => !Visible && GetTree().Paused;
    }
}