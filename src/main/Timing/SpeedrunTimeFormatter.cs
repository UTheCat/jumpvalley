using System;

namespace Jumpvalley.Time
{
    /// <summary>
    /// For a given number of seconds, this class helps to format that number into the speedrun time format (minutes:seconds.milliseconds).
    /// </summary>
    public partial class SpeedrunTimeFormatter
    {
        private double _elapsedTime = 0;

        /// <summary>
        /// The amount of elapsed time in seconds.
        /// Updating this value will also update the <see cref="Minutes"/>, <see cref="Seconds"/>, and <see cref="Milliseconds"/> variables to the corresponding values.
        /// </summary>
        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                Minutes = Math.Floor(value / 60);
                Seconds = (int)Math.Floor(value - Minutes * 60);
                Milliseconds = (int)Math.Floor((value - Minutes * 60 - Seconds * 60) * 1000);
            }
        }

        /// <summary>
        /// Amount of minutes in the elapsed time.
        /// This number does not include fractional minutes, and so this is equal to the greatest integer less than (ElapsedTime / 60).
        /// </summary>
        public double Minutes { get; private set; }

        /// <summary>
        /// Amount of seconds in the elapsed time.
        /// This number does not include the number of whole minutes or the number of milliseconds.
        /// </summary>
        public int Seconds { get; private set; }

        /// <summary>
        /// Amount of milliseconds in the elapsed time.
        /// This number does not include the number of whole minutes or the whole number of seconds that have passed within the elapsed time,
        /// and so this number cannot be greater than 999.
        /// </summary>
        public int Milliseconds { get; private set; }

        /// <returns>
        /// The elapsed time specified in <see cref="ElapsedTime"/> formatted in (minutes:seconds.milliseconds).
        /// </returns>
        public string GetSpeedrunFormatTime()
        {
            return $"{Minutes}:{Seconds.ToString("D2")}.{Milliseconds}";
        }

        public SpeedrunTimeFormatter() { }
    }
}
