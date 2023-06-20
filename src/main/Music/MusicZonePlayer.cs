namespace Jumpvalley.Music
{
    /// <summary>
    /// MusicPlayer that handles the playback of <see cref="Playlist"/>s with music zones in mind.
    /// <br/>
    /// Music zone logic is handled like how it is in Juke's Towers of Hell.
    /// This means that whenever a player enters a music zone, the zone's playlist gets played.
    /// <br/>
    /// In order for music to play when the player is outside a music zone, a primary playlist must be set.
    /// This playlist will get played when outside of the music zones.
    /// </summary>
    public partial class MusicZonePlayer : MusicPlayer
    {
        public MusicZonePlayer() : base() { }


    }
}
