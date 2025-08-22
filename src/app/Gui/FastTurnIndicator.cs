using Godot;
using System;
using UTheCat.Jumpvalley.App.Players.Movement;

namespace UTheCat.Jumpvalley.App.Gui
{
    /// <summary>
    /// Indicator that's shown when the user <i>explicitly</i> has fast-turn enabled and hidden otherwise.
    /// <br/><br/>
    /// An example of the user explicitly toggling fast-turn is when the user presses a button to enable or disable fast turn.
    /// By contrast, in the context of this class, entering/exiting first-person doesn't count as explicitly toggling fast-turn.
    /// </summary>
    public partial class FastTurnIndicator : IDisposable
    {
        private FastTurnControl fastTurnControl;
        private Control actualControl;

        public FastTurnIndicator(FastTurnControl initFastTurnControl, Control initActualControl)
        {
            if (initFastTurnControl == null) throw new ArgumentNullException(nameof(initFastTurnControl), $"Attempted to pass a {nameof(FastTurnControl)} that doesn't exist.");
            if (initActualControl == null) throw new ArgumentNullException(nameof(initActualControl), $"Attempted to pass a {nameof(Control)} that doesn't exist.");

            fastTurnControl = initFastTurnControl;
            actualControl = initActualControl;

            ToggleIndicatorVisibility(initFastTurnControl.UserEnabledFastTurn);
            fastTurnControl.UserToggledFastTurn += HandleUserToggledFastTurn;
        }

        private void ToggleIndicatorVisibility(bool isVisible)
        {
            actualControl.Visible = isVisible;
        }

        private void HandleUserToggledFastTurn(object _o, bool enabled)
        {
            ToggleIndicatorVisibility(enabled);
        }

        public void Dispose()
        {
            fastTurnControl.UserToggledFastTurn -= HandleUserToggledFastTurn;
        }
    }
}
