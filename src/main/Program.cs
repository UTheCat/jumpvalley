using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

// refer to this article for naming conventions:
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions

/**
<summary>
The game's code entry point
</summary>
*/
public partial class Program : Node
{
    private Control mainGui;
    //private Label fpsCounter = new Label();
    private FramerateCounter framerateCounter = new FramerateCounter();

    private BoxSpinner spinner;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Console.WriteLine("hi");
        mainGui = (Control)GetNode("Gui");
        spinner = new BoxSpinner((CsgBox3D)GetNode("Map/CSGBox3D"), 1);

        AddChild(framerateCounter.CountLabel);

        // music system test
        Song song = new Song(
            "res://addons/a_real_reveal/JCM_JM_0058_02401.mp3",
            "A Real Reveal",
            "Juice Music (published by APM)",
            "Day Lite Island"
        );


        Playlist playlist = new Playlist();
        playlist.Name = "PlaylistDemo";
        playlist.TransitionTime = 2;
        playlist.Add(song);
        AddChild(playlist);

        Console.WriteLine("Connect SongChanged event handler");
        playlist.SongChanged += (sender, args) =>
        {
            Song newSong = args.NewSong;
            if (newSong == null)
            {
                Console.WriteLine("Playlist no longer has a song playing");
            } else {
                Console.WriteLine(
                "The song being played by the playlist has changed:"
                + $"\n Name: {newSong.Name}"
                + $"\n Artist(s): {newSong.Artists}"
                + $"\n Album: {newSong.Album}"
                );
            }
        };

        Console.WriteLine("Current linear volume", playlist.LinearVolume);
        Console.WriteLine("play song");

        playlist.Play();
        Console.WriteLine("Song should be fading in");
        Console.WriteLine("Full percent volume in decibels: " + Playlist.VolPercentToDecibels(1));
        Console.WriteLine("Zero percent volume in decibels: " + Playlist.VolPercentToDecibels(0));
        Console.WriteLine("Non-audible percent volume in decibels: " + Playlist.VolPercentToDecibels(Playlist.NonAudiblePercent));

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
        //fpsCounter.Text = "FPS: " + (1 / delta);
        spinner.RotateInFrame(delta);

        // update frames-per-second counter
        framerateCounter.Update(delta);
    }
}
