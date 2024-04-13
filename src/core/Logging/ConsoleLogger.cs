using System;
using Godot;

namespace Jumpvalley.Logging
{
    /// <summary>
    /// Utility class for printing text output to a console or terminal.
    /// </summary>
    public partial class ConsoleLogger
    {
        /// <summary>
        /// Enumerator for specifying which API to use for printing to the console.
        /// </summary>
        public enum PrintingApi
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
        /// The API to use for printing to the console.
        /// </summary>
        public PrintingApi Api;

        /// <summary>
        /// Creates a new instance of <see cref="ConsoleLogger"/> for a given printing "source".
        /// </summary>
        /// <param name="source">Where the output is coming from</param>
        public ConsoleLogger(string source = "", PrintingApi api = PrintingApi.Standard)
        {
            Source = source;
            Api = api;
        }

        /// <summary>
        /// Prints a message to the console with a timestamp.
        /// </summary>
        /// <param name="message"></param>
        public void Print(string message)
        {
            PrintingApi api = Api;
            TimeSpan currentTime = TimeSpan.FromMicroseconds(Time.GetTicksUsec());

            // Time displayed here is formatted based on the information in this article:
            // https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-timespan-format-strings
            string actualMessage = $"[{Source}][{string.Format("{0:g}", currentTime)}]: {message}";

            switch (api)
            {
                case PrintingApi.Standard:
                    Console.WriteLine(actualMessage);
                    break;
                case PrintingApi.Godot:
                    GD.Print(actualMessage);
                    break;
            }
        }
    }
}
