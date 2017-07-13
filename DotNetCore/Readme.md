# A brief example of how to use the HandlebarsCompiler in a project that targets the .net core framework

Follow these simple steps:

1. Put the HandlebarsCompiler.exe in your PATH
2. Add the following to your .csproj file. Adding a PreBuild-Event is possible, too, but Macros are not supported there (https://github.com/dotnet/sdk/issues/677).
Pay attention to the `-c` flag that tells the compiler to not add compiled files to the project file.
```
<Target Name="PreBuild" BeforeTargets="PreBuildEvent">
	<Exec Command="HandlebarsCompiler.exe -c $(ProjectPath)" />
</Target>
```
3. Create a Handlebars-Template (any file with '.hbs' extension) and make sure its `BuildAction` is set to `None`

As soon as you build the project, the compiler will compile all Handlbars-Templates to their C#-counterpart.