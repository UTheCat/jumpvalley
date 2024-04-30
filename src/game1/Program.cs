using Godot;
using System;

namespace Jumpvalley.Game
{
	/// <summary>
	/// Entry point for the game's code
	/// </summary>
	public partial class Program : Node
	{
		private JumpvalleyPlayer player;

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Console.WriteLine("hi :3");
			Console.WriteLine($"Now running: {ProjectSettings.GetSettingWithOverride("application/config/name").As<string>()} v{ProjectSettings.GetSettingWithOverride("application/config/version").As<string>()}");

			player = new JumpvalleyPlayer(GetTree(), this);
			player.Start();

			Console.WriteLine("Game has started successfully.");
		}

		// This root node will be removed from the tree once the program exits
		public override void _ExitTree()
		{
			Console.WriteLine("Now exiting...");

			player.Stop();
			player.Dispose();
			player = null;

			Console.WriteLine("Goodbye!");
		}
	}
}
