## Publishing
To publish, run the dotnet command:

`dotnet publish -c Release`


## Scene Lifecycle
```
InitializeBase();
LoadContent();
Start();
	Update();
	Draw()
```