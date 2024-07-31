using Godot;

namespace JumpvalleyGame.Gui
{
    /// <summary>
    /// Old refresh rate display code that would display the refresh rate to the user using a single <see cref="Label"/>.
    /// </summary>
    public partial class LegacyFramerateCounter : Node
    {
        /// <summary>
        /// The label of the counter to be displayed on the user's screen
        /// </summary>
        public Label CountLabel;

        /// <summary>
        /// Framerate that is considered to be low.
        /// <br/>
        /// The label will display yellow text if the detected FPS is between the value of CriticallyLowFps (exclusive) to the value of this variable (inclusive).
        /// </summary>
        public double LowFps = 40;

        /// <summary>
        /// Framerate that is considered to be critically low
        /// <br/>
        /// The label will display red text if the detected FPS is the value of this variable or lower.
        /// </summary>
        public double CriticallyLowFps = 20;

        public LegacyFramerateCounter(Label initLabel)
        {
            CountLabel = initLabel;
        }

        public LegacyFramerateCounter()
        {
            CountLabel = new Label();
            CountLabel.Name = "FPSCounter";
        }

        /// <summary>
        /// Updates the value of the framerate counter based on the time it took to complete a single frame
        /// </summary>
        /// <param name="delta">The time it took to complete a single frame in seconds</param>
        public void Update(double delta)
        {
            double fps = (1 / delta);
            CountLabel.Text = "FPS: " + fps.ToString("F1");

            Color color;

            if (fps > LowFps)
            {
                color = Color.Color8(0, 255, 0);
            }
            else if (fps > CriticallyLowFps)
            {
                color = Color.Color8(255, 255, 0);
            }
            else
            {
                color = Color.Color8(255, 0, 0);
            }

            CountLabel.RemoveThemeColorOverride("font_color");
            CountLabel.AddThemeColorOverride("font_color", color);
        }

        public override void _Process(double delta)
        {
            Update(delta);
        }
    }
}
