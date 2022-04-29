Unfortunately a pre-compiled version of MonoGame.Extended.dll, .Tiled.dll, and Content.Pipeline.dll are currently required. This is due to `TopdoDownShooter\Content\Content.mgcb` requiring the DLLs to compile the content during build.


There are several solutions to this, but for now you just need to keep them manually updated. The DLLs can be found in this folder by default on Windows:

> `%userprofile%\.nuget\packages\monogame.extended.content.pipeline\3.8.0\tools`


1. First you need to run `dotnet restore` to have NuGet download the files. 
2. Copy the DLLs from here into Lib
3. Now you can properly build with `dotnet build`, or run with `dotnet run`

The longterm plan is to update from using [Tiled](https://www.mapeditor.org/) files to [Ogmo](https://ogmoeditor.itch.io/editor) so this is just a workaround for now.