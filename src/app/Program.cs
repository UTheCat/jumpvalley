using Godot;
using System;

namespace JumpvalleyApp
{
	/// <summary>
	/// Entry point for the game's code
	/// </summary>
	public partial class Program : Node
	{
		private JumpvalleyPlayer player;

		public Program()
		{
			RenderingServer.SetDefaultClearColor(new Color(0, 0, 0, 1));
		}

		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{
			Console.WriteLine("hi :3");

			string nowRunningText = "Now running: ";
			string projectName = ProjectSettings.GetSettingWithOverride("application/config/name").As<string>();
			if (string.IsNullOrEmpty(projectName))
			{
				// This is the folder title used in Godot's default user data folder
				// on desktop platforms for a project that has no name
				nowRunningText += "[unnamed project]";
			}
			else
			{
				nowRunningText += projectName;
			}

			string projectVersion = ProjectSettings.GetSettingWithOverride("application/config/version").As<string>();
			if (!string.IsNullOrEmpty(projectVersion))
			{
				nowRunningText += $" {projectVersion}";
			}
			
			Console.WriteLine(nowRunningText);

			player = new JumpvalleyPlayer(GetTree(), this);
			AddChild(player);
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
