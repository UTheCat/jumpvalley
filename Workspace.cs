using Godot;
using System;

public partial class Workspace : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Console.WriteLine("hi :3");
		Console.WriteLine("meow");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
