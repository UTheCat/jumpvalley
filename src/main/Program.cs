using Godot;
using System;
//using System.Threading;
//using System.Threading.Tasks;

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
        mainGui = (Control) GetNode("Gui");
        spinner = new BoxSpinner((CsgBox3D) GetNode("Map/CSGBox3D"), 1);

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

        Console.WriteLine("play song");
        playlist.Play();
        Console.WriteLine("Song should be fading in");
        Console.WriteLine("Full percent volume in decibels: " + Playlist.VolPercentToDecibels(1));
        Console.WriteLine("Zero percent volume in decibels: " + Playlist.VolPercentToDecibels(0));
        Console.WriteLine("Non-audible percent volume in decibels: " + Playlist.VolPercentToDecibels(Playlist.NonAudiblePercent));

        /*
        new Thread(async () => {
            await Task.Delay(5000);
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
