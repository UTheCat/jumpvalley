using System;

namespace Jumpvalley.Timing
{
    /// <summary>
    /// For a given number of seconds, this class helps to format that number into the speedrun time format (minutes:seconds.fractional_seconds).
    /// <c>fractional_seconds</c> is the fractional part of ElapsedTime that's to the right of the decimal point.
    /// </summary>
    public partial class SpeedrunTimeFormatter
    {
        private double _elapsedTime = 0;

        /// <summary>
        /// The amount of elapsed time in seconds.
        /// Updating this value will also update the <see cref="Minutes"/> and <see cref="Seconds"/> variables to the corresponding values.
        /// </summary>
        public double ElapsedTime
        {
            get => _elapsedTime;
            set
            {
                _elapsedTime = value;
                Minutes = Math.Floor(value / 60);
                Seconds = value - Minutes * 60;
            }
        }

        /// <summary>
        /// Amount of minutes in the elapsed time.
        /// This number does not include fractional minutes, and so this is equal to the greatest integer less than (ElapsedTime / 60).
        /// </summary>
        public double Minutes { get; private set; }

        /// <summary>
        /// Amount of seconds in the elapsed time.
        /// This number does not include the number of whole minutes.
        /// In other words, it's equal to ElapsedTime - (floor(ElapsedTime / 60) * 60).
        /// </summary>
        public double Seconds { get; private set; }

        /// <summary>
        /// Formats <see cref="ElapsedTime"/> into speedrun time format (minutes:seconds.fractional_seconds).
        /// </summary>
        /// <param name="fractionalSecondsNumDigits">The number of digits displayed in the fractional_seconds part of the speedrun time string.</param>
        /// <returns>
        /// The elapsed time specified in <see cref="ElapsedTime"/> formatted in (minutes:seconds.fractional_seconds).
        /// </returns>
        public string GetSpeedrunFormatTime(int fractionalSecondsNumDigits)
        {
            // The zero that's appended to the left of the seconds count,
            // in case it happens to be less than 10.
            // The formatted time would otherwise be incorrect as the integer part of the seconds count
            // should always be two digits long.
            string appendedZero = "";
            if (Seconds < 10)
            {
                appendedZero = "0";
            }

            return $"{(int)Minutes}:{appendedZero}{Seconds.ToString($"F{fractionalSecondsNumDigits}")}";
        }

        public SpeedrunTimeFormatter() { }
    }
}
