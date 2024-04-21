# Jumpvalley

This is a 3D-platformer made with the [Godot Engine](https://godotengine.org) based on the hardcore platformer, [Juke's Towers of Hell](https://www.roblox.com/games/8562822414/Jukes-Towers-of-Hell). Yes, this means you will not need to be connected to the internet while playing an obstacle course that runs on Jumpvalley!

Currently, it's a work in progress. A lot of basic features, like being able to load other obstacle courses (other than the ones present when you first load the game), as well as various level mechanics such as jump pads, have yet to be implemented.

Though if you want to test it, it is playable. There's currently a small obstacle course you can try out. For the most part, the control scheme is the same as the one in Juke's Towers of Hell (with the main exception being the ability to ladder-boost as done in [Flood Escape 2](https://www.roblox.com/games/738339342/Flood-Escape-2)).

**Important:** While it is currently possible to make an obstacle course in Jumpvalley with the Godot Editor, backwards-compatibility is subject to being broken while Jumpvalley is still in major version zero (0.x.x). This is because level behavior hasn't been set in stone yet. If you want to prevent your level from being broken between minor versions, please wait until Jumpvalley has reached major version one (1.0.0).

## Credits

The ```addons``` folder contains assets made by 3rd-parties. They're super useful in this project because not only do I not have the skill, time, and/or resources to make a lot of these assets, but they also allow me to speed up development and testing using resources that the creators of these assets have allowed others to use.

A list of such assets can be found [here](https://github.com/UTheDev/jumpvalley/blob/main/credits.md).

## Licensing

This project's source code is licensed under the MIT License. The source code's license can be found [here](https://github.com/UTheDev/jumpvalley/blob/main/LICENSE.md).

Assets under the ```addons``` folder are covered by different licenses (especially since they were made by other individuals).

## Running the game

If you're looking to simply run Jumpvalley, you can find the game precompiled in the [releases](https://github.com/UTheDev/jumpvalley/releases) tab.

Jumpvalley currently doesn't have an installer executable. Therefore, downloading the compressed folder corresponding to your device's operating system, and then extracting it, should give you the files needed to run precompiled Jumpvalley.

### For Android users

While there are precompiled versions of Jumpvalley for Android, Android support is *experimental* for these reasons:
- Currently, exporting your game to an Android APK on Godot is itself an experimental feature
- You'll need to connect a keyboard and mouse to your Android device in order to move your character and camera.

### For Linux users

The Jumpvalley executable for the Linux version of Jumpvalley is named `jumpvalley` (with no file extension).

On Linux, the operating system itself controls whether or not a file can be executed, regardless of the file's file extension. Therefore, your copy of the  Jumpvalley executable for Linux might not be marked as executable.

In order to fix this, open up a terminal session, change the working directory to the directory containing the Jumpvalley executable like this:

`cd [path to the directory with the Jumpvalley executable]`

and type this command:

`chmod a+x jumpvalley`

This tells the operating system to let itself execute the file (in this case, the file is the Jumpvalley executable). You should then be able to run the Jumpvalley executable.

## Documentation

Jumpvalley's documentation can be found on its [website](https://uthedev.github.io/jumpvalley_docs/index.html) hosted on GitHub.

## Working with this repository

Here are some instructions for working with this project's repository. This assumes that you actually want to do things with Jumpvalley's Git repository.

### Prerequisites

Software you'll need:
- .NET-Enabled Godot v4.2.2 or later. The latest version of .NET-Enabled Godot 4 is preferred, and can be downloaded from [Godot's official download page](https://godotengine.org/download).
- An installation of the [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Visual Studio Code with the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) (if you want to work with the project's source code)

### Running Godot and this repository's project file

Open your copy of this repository's ```project.godot``` file in the version of Godot specified in the prerequisites.

In order to run the project, there's a play button near the top-right corner of the window. Click it to run the project.

### Debugging with Visual Studio Code

If you're working in Visual Studio Code, Jumpvalley has a launch configuration named `Debug` that you can use to debug Jumpvalley. This will allow you to see the game's console output.

Just make sure you have an environment variable named `JUMPVALLEY_GODOT_EXECUTABLE` set to the path to the Godot executable as mentioned in the prerequisites, and you should be able to run the launch configuration.
