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
        /// Creates a new instance of <see cref="ConsoleLogger"/> for a given printing "source".
        /// </summary>
        public ConsoleLogger()
        {

        }
    }
}