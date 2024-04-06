namespace Jumpvalley.Logging
{
    /// <summary>
    /// Utility class for printing text output to a console or terminal.
    /// </summary>
    public partial class ConsoleLogger
    {
        public enum PrintingInterface
        {
            /// <summary>
            /// Print to console using C#'s Console.WriteLine function
            /// </summary>
            Standard = 0,

            /// <summary>
            /// Print to console using Godot's GD.Print function
            /// </summary>
            Godot = 1,
        }

        /// <summary>
        /// The source of the output. Use this field to specify where the output is coming from.
        /// </summary>
        public string Source;

        /// <summary>
        /// Creates a new instance of <see cref="ConsoleLogger"/> for a given printing "source".
        /// </summary>
        /// <param name="source">Where the output is coming from</param>
        public ConsoleLogger(string source)
        {
            Source = source;
        }
    }
}