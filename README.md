# Compiled Handlebars for C#
A compiler that compiles Handlebars-Templates into native C# code for performant and type-safe serverside HTML rendering.

### Project Status
This project is currently in an alpha phase. Most of the features planned for the release are implemented but not yet thoroughly tested.

### Features
+ Get your type-errors at design-time
+ Resolving partial and helper calls at design-time
+ Trivial invocation: TemplateName.Render(viewModel);
+ Minimal dependencies: resulting code just depends on System, System. Net and System.Text 

#### Performance
The compiler is not yet finished and performance improvements are yet to come but first tests already show a significant improvement over the JavaScript implementation of Handlebars:\
Tests from https://github.com/gotascii/js-templates-benchmark
+ Mustache -> Simple: ~55ms; Loop ~128ms
+ Handlebars.js -> Simple: ~8ms; Loop ~75ms
+ CompiledHandlebars -> Simple: ~6ms; Loop ~11ms


## Compatability to Handlebars.js
### Special Syntax
Because of the different approach to the Handlebars.js version (e.g. statically typed Handlebars-Templates), the compiler needs to know for which type your Handlebars-Template is meant.\
This information is communicated to the compiler by a special ModelToken. Its syntax is straightforward: \
{{model NamespacePath.ClassName}}\
For example:\
{{model HandlebarsTest.TestViewModel}}

This token needs to be at the beginning of every Handlebars-Template.

#### Implemented Subset
+ Html Encoding/Escaping
+ Builtin Block Helpers
+ Partials
+ Helper Functions
+ Whitespace Control
 
#### Not Implemented:
+ Mustache Blocks
+ Block Helpers
+ Subexpressions


## Usage
There are two basic ways to use the Handlebars Compiler in your project:
#### Visual Studio Integration
Just install the vsix package from the CustomTool project and restart Visual Studio. Add a new Handlebars template to your solution (ending on .hbs) and then add "HandlebarsCompiler" to the file's CustomTool property. \
The compiler will be invoked every time the Handlebar template is saved and will create a {templatename}.hbs.cs file containing the generated code.
#### Command Line Interface
TODO

## Building
Following prerequisites are needed in order to be able to build the solution:
+ Visual Studio 2015
+ Visual Studio 2015 Extensibility Tools