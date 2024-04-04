using System;
using System.Diagnostics;

namespace Jumpvalley.Timing
{
    /// <summary>
    /// This class extends the <see cref="Stopwatch"/> (in the standard C# library) so that an initial value for elapsed time can be specified.
    /// This initial value acts as an offset for the Stopwatch's real elapsed time.
    /// </summary>
    public partial class OffsetStopwatch: Stopwatch
    {
        /// <summary>
        /// The elapsed time offset applied to the original <see cref="Elapsed"/> variable in order to return the offsetted elapsed time
        /// in <see cref="OffsetElapsedTime"/>
        /// </summary>
        public TimeSpan TimeOffset;

        /// <summary>
        /// The elapsed time after applying the time offset defined in <see cref="TimeOffset"/>
        /// </summary>
        public TimeSpan OffsetElapsedTime
        {
            get => Elapsed.Add(TimeOffset);
        }

        /// <summary>
        /// Creates a new instance of <see cref="OffsetStopwatch"/>
        /// </summary>
        /// <param name="offset">The time offset to apply</param>
        public OffsetStopwatch(TimeSpan offset)
        {
            TimeOffset = offset;
        }
    }
}
