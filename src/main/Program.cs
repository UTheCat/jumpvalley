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

		//private MeshSpinner spinner;

		public Player player;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Console.WriteLine("hi :3");

			player = new Player(GetTree(), this);
			player.Start();
		}

		// This root node will be removed from the tree once the program exits
		public override void _ExitTree()
		{
			Console.WriteLine("Now exiting...");

			player.Dispose();
			player = null;

			Console.WriteLine("Goodbye!");
		}
	}

}
