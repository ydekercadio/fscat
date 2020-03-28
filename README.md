# fscat
.NET Core tool that concats all F# source files of a project into a single source file.

# Install
## From NuGet
The easy way to install the tool is to get it from NuGet.

Move to your project's directory (the one containing your sln file), and run:

```dotnet new tool-manifest```

```dotnet tool install ydk.fscat```

## Build and install
Alternatively, you may wish to download the sources, and build from them.

To build, move to your fscat root directory (where you downloaded this project):

```dotnet pack /p:Configuration=Release /p:Platform="Any CPU"```

This will create a nupkg subdirectory, containing the package for the tool.

To install, move to your own project and run:

```dotnet new tool-manifest```

```dotnet tool install --add-source <path_to_your_fscat_solution>\fscat\nupkg ydk.fscat```

# Run
Once installed, you can run the tool from ```dotnet tool run```.

For instance, to scan myproj\myproj.fs and output its sources concatenated in unique.fs, run:

```dotnet tool run fscat -- myproj\myproj.fsproj unique.fs```
