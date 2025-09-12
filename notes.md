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

## Debugging

Due to Godot quirks, if you want the Visual Studio Code debugger to stop at exceptions while debugging Jumpvalley, you will need to enable "All Exceptions" under Run and Debug > Breakpoints.
