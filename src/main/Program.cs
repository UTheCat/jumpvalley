using Godot;
using System;

using Jumpvalley.Players;

// refer to this article for naming conventions:
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

/**
<summary>
The game's code entry point
</summary>
*/
namespace Jumpvalley
{
    public partial class Program : Node
    {
        private Control mainGui;
        //private Label fpsCounter = new Label();

        private BoxSpinner spinner;

        public Player player;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Console.WriteLine("hi :3");
            spinner = new BoxSpinner((CsgBox3D)GetNode("Map/CSGBox3D"), 1);

            player = new Player(GetTree(), this);
            player.Start();

            /*
            mainGui = (Control)GetNode("Gui");
            spinner = new BoxSpinner((CsgBox3D)GetNode("Map/CSGBox3D"), 1);

            AddChild(framerateCounter.CountLabel);

            Console.WriteLine("Run MusicPlaylistTest");
            MusicPlayerTest musicPlayerTest = new MusicPlayerTest();
            AddChild(musicPlayerTest);

            Label bottomBarDesc = (Label)GetNode("Gui/BottomBar/Description");

            Console.WriteLine("Connect SongChanged event handler to musicPlayer");
            musicPlayerTest.CurrentMusicPlayer.SongChanged += (sender, args) =>
            {
                Song newSong = args.NewSong;
                if (newSong == null)
                {
                    Console.WriteLine("musicPlayer no longer has a song playing");
                    bottomBarDesc.Text = "No music playing";
                }
                else
                {
                    Console.WriteLine(
                    "The song being played by musicPlayer has changed:"
                    + $"\n Name: {newSong.Name}"
                    + $"\n Artist(s): {newSong.Artists}"
                    + $"\n Album: {newSong.Album}"
                    );
                    bottomBarDesc.Text = $"MUSIC\n{newSong.Artists} - {newSong.Name}";
                }
            };

            Console.WriteLine("Try playing music");
            musicPlayerTest.startTest();

            Console.WriteLine("Run MethodTweenTest");
            MethodTweenTest methodTweenTest = new MethodTweenTest(0.4f, 0.8f);
            AddChild(methodTweenTest);
            */

            // music system test
            /*
            Song song = new Song(
                "res://addons/music/KORAII/Night_Echo/672358_Night-Echo.mp3",
                "Night Echo",
                "KORAII",
                ""
            );
            */

            /*
            Playlist playlist = new Playlist();
            playlist.Name = "PlaylistDemo";
            playlist.TransitionTime = 2;
            playlist.Add(song);
            AddChild(playlist);

            Console.WriteLine("test music player");
            MusicPlayer musicPlayer = new MusicPlayer();
            */

            /*
            Console.WriteLine("Set primary playlist");
            musicPlayer.PrimaryPlaylist = playlist;

            Console.WriteLine("Try playing music");
            musicPlayer.IsPlaying = true;
            */

            /*
            Console.WriteLine("Current linear volume", playlist.LinearVolume);
            Console.WriteLine("play song");

            playlist.Play();
            Console.WriteLine("Song should be fading in");
            Console.WriteLine("Full percent volume in decibels: " + Playlist.VolPercentToDecibels(1));
            Console.WriteLine("Zero percent volume in decibels: " + Playlist.VolPercentToDecibels(0));
            Console.WriteLine("Non-audible percent volume in decibels: " + Playlist.VolPercentToDecibels(Playlist.NonAudibleVolume));
            */

            /*
            new Thread(async () => {
                Console.WriteLine("Song will get stopped in 5 seconds");
                await Task.Delay(5000);
                Console.WriteLine("Stopping song");
                playlist.Stop();
            }).Start();
            */

            //fpsCounter.Name = "FPSCounter";
            //fpsCounter.Text = "FPS: ";
            //mainGui.AddChild(fpsCounter);
            //this.AddChild(mainGui);
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            spinner.RotateInFrame(delta);
        }

        // This root node will be removed from the tree once the program exits
        public override void _ExitTree()
        {
            player.Dispose();
            player = null;
        }
    }

}