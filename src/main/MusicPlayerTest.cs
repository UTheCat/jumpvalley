using System.Collections.Generic;
using Godot;

public partial class MusicPlayerTest : Node
{
    private Song[] songList = {
        new Song(
            "res://addons/music/KORAII/Night_Echo/672358_Night-Echo.mp3",
            "Night Echo",
            "KORAII",
            ""
        ),
        new Song(
            "res://addons/music/ParagonX9/Red_13/464352_ParagonX9___Red_13.mp3",
            "Red 13",
            "Paragon X9",
            ""
        )
    };

    private bool isListeningToInput = false;
    private int songIndex = 0;

    private MusicPlayer _currentMusicPlayer = new MusicPlayer();

    public MusicPlayer CurrentMusicPlayer
    {
        get => _currentMusicPlayer;
        private set
        {
            _currentMusicPlayer = value;
        }
    }

    private void playSongIndex(int index)
    {
        Playlist playlist = new Playlist();
        playlist.Add(songList[index]);
        _currentMusicPlayer.PrimaryPlaylist = playlist;
    }

    public void startTest()
    {
        OS.Alert("Press space to switch to the next song", "Instructions");

        AddChild(_currentMusicPlayer);
        _currentMusicPlayer.IsPlaying = true;
        playSongIndex(songIndex);

        isListeningToInput = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (isListeningToInput && @event is InputEventKey keyEvent && keyEvent.Keycode == Key.Space && keyEvent.Pressed)
        {
            songIndex++;
            if (songIndex >= songList.Length) { songIndex = 0; }

            playSongIndex(songIndex);
        }
    }
}