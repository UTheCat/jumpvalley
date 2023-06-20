using System;

using Jumpvalley.Music;

namespace Jumpvalley.Player
{
    /// <summary>
    /// This class represents a player who is playing Jumpvalley.
    /// <br/>
    /// The class contains some of the basic components that allow Jumpvalley to function for the player, such as:
    /// <list type="bullet">
    /// <item>Their music player</item>
    /// <item>The Controller instance that allows them to control their character</item>
    /// <item>The Camera instance that allows them to control their camera</item>
    /// </list>
    /// </summary>
    public partial class Player: IDisposable
    {   
        /// <summary>
        /// The player's current music player
        /// </summary>
        public MusicPlayer CurrentMusicPlayer { get; private set; }

        public Player()
        {
            CurrentMusicPlayer = new MusicPlayer();
        }

        public void Dispose()
        {
            CurrentMusicPlayer.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
