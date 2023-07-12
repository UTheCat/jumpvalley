# Notes
Some notes that may be useful in the development of this project.

## Exporting for release

When exporting a publicly visible build of the project, make sure to turn off the export of debug symbols.
This is because Dotnet debug symbols contain the full file paths of files in the project, which can expose personal information.
