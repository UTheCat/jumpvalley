/// <summary>
/// Event arguments for <see cref="Playlist.SongChanged"/> and <see cref="MusicPlayer.SongChanged"/>
/// </summary>
public class SongChangedArgs : System.EventArgs
{
    public Song NewSong { get; private set; }

    public SongChangedArgs(Song s)
    {
        NewSong = s;
    }
}