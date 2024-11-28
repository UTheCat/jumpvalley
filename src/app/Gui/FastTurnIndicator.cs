using Godot;
using System;

using Jumpvalley.Players.Movement;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Indicator that's shown when the user has fast-turn enabled
    /// and hidden otherwise.
    /// </summary>
    public partial class FastTurnIndicator
    {
        private BaseMover mover;
        private Control actualControl;

        public FastTurnIndicator(BaseMover initMover, Control initActualControl)
        {
            if (initMover == null) throw new ArgumentNullException(nameof(initMover), $"Attempted to pass a {nameof(BaseMover)} that doesn't exist.");
            if (initActualControl == null) throw new ArgumentNullException(nameof(initActualControl), $"Attempted to pass a {nameof(Control)} that doesn't exist.");

            mover = initMover;
            actualControl = initActualControl;

            mover.OnFastTurnToggled += HandleFastTurnToggled;
        }

        private void HandleFastTurnToggled(object _o, bool enabled)
        {
            actualControl.Visible = enabled;
        }
    }
}
