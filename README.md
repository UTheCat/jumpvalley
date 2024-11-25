# Jumpvalley

Jumpvalley is both an app and a library you can use to test and run 3D platformer levels.

Currently, it's a work in progress. Some basic features (such as being able to switch levels within the app) have yet to be implemented. Though if you want to test it, it is playable.

Jumpvalley also features a core API in its repository (this one) that allows developers to code levels with. The core API also features some classes you can use to program a 3D platformer app or game with.

**Important:** While it is currently possible to make a platformer level in Jumpvalley with the Godot Editor, backwards-compatibility is subject to being broken while Jumpvalley is still in major version zero (0.x.x). This is because level formatting and behavior hasn't been set in stone yet. If you want to prevent your level from being broken between minor versions, please wait until Jumpvalley has reached major version one (1.0.0).

## Credits

The ```addons``` folder in this repository's root contains assets made by 3rd-parties. They're super useful in this project because not only do I not have the skill, time, and/or resources to make a lot of these assets, but they also allow me to speed up development and testing using resources that the creators of these assets have allowed others to use.

A list of such assets can be found [here](https://github.com/UTheCat/jumpvalley/blob/main/credits.md).

### Inspirations

Jumpvalley was inspired by several fun platformers that you should check out, including:

- [Celeste](https://www.celestegame.com/)
- [Flood Escape 2](https://www.roblox.com/games/738339342/Flood-Escape-2)
- [Juke's Towers of Hell](https://www.roblox.com/games/8562822414/Jukes-Towers-of-Hell)

## Licensing

This project's source code is licensed under the MIT License. The source code's license can be found [here](https://github.com/UTheCat/jumpvalley/blob/main/LICENSE.md).

Assets under the ```addons``` folder are 3rd-party assets. As such, they are covered by different licenses (especially since they were made by other individuals).

## Running the app

If you're looking to simply run Jumpvalley, you can find the app precompiled in the [releases](https://github.com/UTheCat/jumpvalley/releases) tab.

Jumpvalley currently doesn't have an installer executable. Therefore, downloading the compressed folder corresponding to your device's operating system, and then extracting it, should give you the files needed to run precompiled Jumpvalley.

### For Android users

While there are precompiled versions of Jumpvalley for Android, Android support is *experimental* for these reasons:
- Currently, exporting an app with C# code to an Android APK on Godot 4 is itself an experimental feature
- Currently, you'll need to connect a keyboard and mouse to your Android device in order to move your character and camera.

### For Linux users

The Jumpvalley executable for the Linux version of Jumpvalley is named `jumpvalley` (with no file extension).

On Linux, the operating system itself controls whether or not a file can be executed, regardless of the file's file extension. Therefore, your copy of the Jumpvalley executable for Linux might not be marked as executable.

In order to fix this, open up a terminal session, change the working directory to the directory containing the Jumpvalley executable like this:

`cd [path to the directory with the Jumpvalley executable]`

and type this command:

`chmod +x jumpvalley`

This tells Linux to allow running the Jumpvalley executable.

## Documentation

Jumpvalley's documentation can be found on its [website](https://uthecat.github.io/jumpvalley-docs/) hosted on GitHub.

## Working with this repository

Here are some instructions for working with this project's repository. This assumes that you actually want to do things with Jumpvalley's Git repository.

### Prerequisites

Software you'll need:
- .NET-Enabled Godot v4.3 or later. The latest version of .NET-Enabled Godot 4 is preferred, and can be downloaded from [Godot's official download page](https://godotengine.org/download).
- The [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code](https://code.visualstudio.com/) with the [C# extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) (if you want to work with the project's source code)

### Running Godot and this repository's project file

Open your copy of this repository's ```project.godot``` file in the version of Godot specified in the prerequisites.

In order to run the project, there's a play button near the top-right corner of the window. Click it to run the project.

### Debugging with Visual Studio Code

If you're working in Visual Studio Code, Jumpvalley has a launch configuration named `Debug` that you can use to debug Jumpvalley. This will allow you to see the app's console output.

Just make sure you have an environment variable named `JUMPVALLEY_GODOT_EXECUTABLE` set to the path to the Godot executable as mentioned in the prerequisites, and you should be able to run the launch configuration.

Additionally, if you make any changes to the app's code, you'll have to rebuild the project. This can be done by opening the project in Godot and clicking the hammer icon at the top-right corner of the window. (Note: If you don't see this icon, which should be next to the play button in the Godot window, check to see that you installed .NET properly and that `project.godot` points to the correct C# assembly file.)
