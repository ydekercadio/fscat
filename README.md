# fscat
.NET Core tool that concats all F# source files of a project into a single source file.

To build, move to your fscat root directory (where you downloaded this project):

```dotnet pack /p:Configuration=Release /p:Platform="Any CPU"```

This will create a nupkg subdirectory, containing the package for the tool.

To install from you build, move to your own project and run:

```dotnet tool update --add-source <path_to_your_fscat_solution>\fscat\nupkg ydk.fsc```

Then, you run the tool, for instance to scan myproj\myproj.fs and output it in unique.fs
```dotnet tool run fscat -- myproj\myproj.fsproj unique.fs```
