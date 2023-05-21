using Godot;
//using System;

public partial class App : Node
{
	private Control mainGui = new Control();
	private Label fpsCounter = new Label();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		fpsCounter.Name = "FPSCounter";
		fpsCounter.Text = "FPS: ";
		mainGui.AddChild(fpsCounter);
		this.AddChild(mainGui);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		fpsCounter.Text = "FPS: " + (1 / delta);
	}
}
