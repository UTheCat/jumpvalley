using Godot;
//using System;

// refer to this article for naming conventions:
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
public partial class App : Node
{
	private Control mainGui = new Control();
	private Label fpsCounter = new Label();

	private BoxSpinner spinner;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spinner = new BoxSpinner((CsgBox3D) GetNode("Map/CSGBox"), 1);

		fpsCounter.Name = "FPSCounter";
		fpsCounter.Text = "FPS: ";
		mainGui.AddChild(fpsCounter);
		this.AddChild(mainGui);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fpsCounter.Text = "FPS: " + (1 / delta);
		spinner.RotateInFrame(delta);
	}
}
