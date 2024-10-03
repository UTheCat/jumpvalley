using Godot;
using Jumpvalley.Timing;

namespace JumpvalleyApp.Gui
{
    /// <summary>
    /// Class that handles the LevelTimer GUI which displays how long a player has been playing a level.
    /// </summary>
    public partial class LevelTimer
    {
        /// <summary>
        /// The <see cref="SpeedrunTimeFormatter"/> that holds the amount of elapsed time that the level timer will display.
        /// </summary>
        private SpeedrunTimeFormatter timeCounter;

        private double _elapsedTime;
        private Label timerText;
        private Label timerTextMs;

        /// <summary>
        /// The amount of elapsed time (in seconds) that will be shown by the level timer
        /// </summary>
        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                timeCounter.ElapsedTime = value;
                
                if (timerText != null && timerTextMs != null)
                {
                    double tcSeconds = timeCounter.Seconds;

                    string appendedZero = "";
                    if (tcSeconds < 10)
                    {
                        appendedZero = "0";
                    }

                    // Seconds is displayed with 3-decimal-digit accuracy
                    // (aka, number of milliseconds is displayed to the right of the decimal point)
                    string[] tcSecondsString = tcSeconds.ToString("F3").Split(".");

                    // just in case
                    if (tcSecondsString.Length < 2) return;

                    timerText.Text = $"{timeCounter.Minutes}:{appendedZero}{tcSecondsString[0]}";
                    timerTextMs.Text = $".{tcSecondsString[1]}";
                }
            }
        }

        /// <summary>
        /// Constructs a new instance of this level timer operator class
        /// </summary>
        /// <param name="levelTimerNode">The actual Godot node of the level timer where the elapsed time will be displayed</param>
        public LevelTimer(Node levelTimerNode)
        {
            timeCounter = new SpeedrunTimeFormatter();

            timerText = levelTimerNode.GetNode<Label>("TimerText");
            timerTextMs = levelTimerNode.GetNode<Label>("TimerTextMs");
        }


    }
}
