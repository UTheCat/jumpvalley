# Notes

Some notes that may be useful in the development of this project.

## Exporting for release

When exporting a publicly visible build of the project, make sure to turn off the export of debug symbols.
This is because Dotnet debug symbols contain the full file paths of files in the project, which can expose personal information.

## Resources useful in the making of this project

GDScript basics:
https://docs.godotengine.org/en/stable/tutorials/scripting/gdscript/gdscript_basics.html

Documentation comments:
https://docs.godotengine.org/en/stable/tutorials/scripting/gdscript/gdscript_documentation_comments.html

GDScript style guide:
https://docs.godotengine.org/en/stable/tutorials/scripting/gdscript/gdscript_styleguide.html#classes-and-nodes

Icon changing:
https://docs.godotengine.org/en/stable/getting_started/workflow/export/changing_application_icon_for_windows.html

Meshes and Textures:
https://www.youtube.com/watch?v=K7a4hDRxYu8

3D Asset Library:
https://polyhaven.com/

GUI Design in Godot:
https://docs.godotengine.org/en/stable/tutorials/ui/gui_skinning.html

## Misc

### Debugging

```GD.Print()``` seems to be wack at the moment.

Therefore, to see console output:
Use the standard ```Console.WriteLine()```, and export the project to an exe with debugging symbols enabled. Then, run the resulting "console.exe" (preferably with a terminal)

### Physics

- Player's (their character's) gravity (as in gravitational acceleration) seems to be the same as a box with a mass of 0.01kg as of Jumpvalley v0.3.0
    - Changing physics refresh rate from 60 hz to 120 hz and vice versa does not seem to make a difference in the gravitational acceleration of the box or the player's character. Increasing the physics refresh rate from 60 hz or 120 hz to 240 hz does not make a difference either.
    - However, increasing the physics refresh rate from 60 hz to 120 hz has made the box easier to push. Increasing the refresh rate from 120 hz to 240 hz seems to have made physics in Jumpvalley more stable as well.
- In Eternal Towers of Hell, the physics refresh rate is 240 hz.
