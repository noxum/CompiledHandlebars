# A brief example of how to use the HandlebarsCompiler in a project that targets the .net framework

Follow these simple steps:

1. Put the HandlebarsCompiler.exe in your PATH
2. Add a pre-build event that calls the compiler
In this case: `HandlebarsCompiler.exe $(ProjectPath)`
3. Create a Handlebars-Template (any file with '.hbs' extension) and make sure its `BuildAction` is set to `None`

As soon as you build the project, the compiler will compile all Handlbars-Templates to their C#-counterpart and add them to the project.
Possible compiler errors will show up in the `Output` of the build process.