namespace Jumpvalley.Players.Movement
{
    /// <summary>
    /// Event arguments for <see cref="BaseMover.BodyStateChanged"/>.
    /// In this class's documentation, "character" refers to the CharacterBody3D whose movement is being handled by its <see cref="BaseMover"/>
    /// </summary>
    public partial class BodyStateChangedArgs
    {
        /// <summary>
        /// The character's previous body state
        /// </summary>
        public readonly BaseMover.BodyState OldState;

        /// <summary>
        /// The character's new body state
        /// </summary>
        public readonly BaseMover.BodyState NewState;

        /// <summary>
        /// Creates a new instance of <see cref="BodyStateChangedArgs"/> to indicate that a character's body state has changed.
        /// </summary>
        /// <param name="oldState">The character's previous body state</param>
        /// <param name="newState">The character's new body state</param>
        public BodyStateChangedArgs(BaseMover.BodyState oldState, BaseMover.BodyState newState)
        {
            OldState = oldState;
            NewState = newState;
        }
    }
}
