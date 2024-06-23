using Godot;

namespace Jumpvalley.Levels.Interactives
{
    /// <summary>
    /// Provides some variables and methods that may assist with the development of interactives.
    /// </summary>
    public partial class InteractiveToolkit
    {
        /// <summary>
        /// The string identifier for the number that determines the number of physics updates per second (which is 60 by default)
        /// </summary>
        public static string PROJECT_SETTINGS_PHYSICS_TICKS_PER_SECOND = "physics/common/physics_ticks_per_second";

        /// <summary>
        /// Name of the metadata entry that indicates the class that will be running the interactive.
        /// </summary>
        public static readonly string INTERACTIVE_TYPE_METADATA_NAME = "type";

        /// <summary>
        /// Returns the current number of physics updates per second
        /// </summary>
        /// <returns>The current number of physics updates per second</returns>
        public static int GetPhysicsTicksPerSecond()
        {
            return Engine.MaxFps;
        }
    }
}
