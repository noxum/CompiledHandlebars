Compiled Handlebars for C#
==========================

Compiling Handlebars-Templates into native C# code for performant and type-safe serverside HTML rendering.

### Project Status
This project is currently in a beta phase.

### Features
+ Get your type-errors at design-time
+ Resolving partial and helper calls at design-time
+ Trivial invocation: TemplateName.Render(viewModel);
+ Minimal dependencies: resulting code just depends on System, System. Net and System.Text 


#### Performance
A performance comparison was made against the microbenchmarks from the handlebars.js repository:
https://github.com/wycats/handlebars.js/tree/master/bench

| Microbenchmark | handlebars.js | CompiledHandlebars | Speedup |
|----------------|---------------|--------------------|---------|
|arrayeach|391 ops/ms|3039 ops/ms|7.77|
|complex|120 ops/ms|1180 ops/ms|9.83|
|data|295 ops/ms|1333 ops/ms|4.51|
|depth1|228 ops/ms|3693 ops/ms|16.20|
|depth2|63 ops/ms|1515 ops/ms|24.04|
|partial-recursion|125 ops/ms|1895 ops/ms|15.16|
|partial|211 ops/ms|905 ops/ms|4.29|
|paths|2060 ops/ms|4646 ops/ms|2.25|
|string|5563 ops/ms|13964 ops/ms|2.51|
|variables|1991 ops/ms|4027 ops/ms|2.02|


### Usage

#### Visual Studio Integration
Just install the vsix package from the CustomTool project and restart Visual Studio. Add a new Handlebars template to your solution (ending on .hbs) and then add "HandlebarsCompiler" to the file's CustomTool property. 
The compiler will be invoked every time the Handlebar template is saved and will create a {templatename}.hbs.cs file containing the generated code.

#### Basic Usage
Every Handlebars-Template needs a type which it renders. 
As an example a class named "PersonModel" will serve:
```CSharp
namespace ViewModels
{
  public class PersonModel
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int Age { get; set; }
    public List<string> EMailAddresses { get; set; }
  }
}
```

Now, we must communicate the type to the HandlebarsCompiler. At the beginning of each Handlebars-Template there needs to be a socalled ModelToken:
```Handlebars
{{model NamespacePath.ClassName}}
```
For our example that would be:
```Handlebars
{{model ViewModels.PersonModel}}
```
Note, that the type inside the ModelToken can also be a base class or interface of the actual ViewModel that is passed to the template.


The rest of the Handlebars-Templates follows standard Handlebars syntax and semantics.
Here is the full example:
```Handlebars
{{model ViewModels.PersonModel}}
<h1>Hello {{FirstName}} {{LastName}}</h1>
<p>You are {{Age}} years old and you have
{{#if EMailAddresses}}
  following email addresses:
  <ul>
  {{#each EMailAddresses}}
    <li>{{this}}</li>
  {{/each}}
  </ul>
{{else}}
  no email addresses.
{{/if}}</p>
```

#### Partials
Partials do not need to be registered. Just be sure to compile the partial before the Handlebars-Template that uses that partial. For everything else standard handlebars.js logic applies.

#### Helper Functions
CompiledHandlebars allow the usage of helper functions. These must be static, return a string and be annotated by an attribute. Parameters are checked if they match at compile-time so overloading is possible. Other than that, they have no restrictions an can be placed anywhere in your codebase.

```CSharp
[CompiledHandlebarsHelperMethod]
pulic static string FullName(PersonModel model)
{
  return string.Concat(model.FirstName, " ", model.LastName);
}
```

#### Layout Functionality
CompiledHandlebars offers differing functionality regarding layouts:
Any HandlabarsTemplate can be rendered inside a HandlebarsLayout. HandlebarsLayouts differ from normal HandlebarsTemplates due to a special {{body}} token:
```Handlebars
{{model ViewModels.IPageModel}}
<!DOCTYPE html>
<head>
  <title>{{Title}}</title>
  {{#if Keywords}}<meta name="keywords" content="{{Keywords}}">{{/if}}
</head>
<body>
  {{body}}
</body>
</html>
```

To render a Handlebars-Template in that layout, use the {{layout}} Token right after the {{model}} token:
```Handlebars
{{model ViewModels.TitlePageModel}}
{{layout MainLayout}}
<h1>{{Headline}}</h1>
<p>{{Content}}</p>
```

The result of that template is equal to the following template:
```Handlebars
{{model ViewModels.TitlePageModel}}
<!DOCTYPE html>
<head>
<title>{{Title}}</title>
  {{#if Keywords}}<meta name="keywords" content="{{Keywords}}">{{/if}}
</head>
<body>
  <h1>{{Headline}}</h1>
  <p>{{Content}}</p>
</body>
</html>
```





## Compatability to Handlebars.js
### Special Syntax
Because of the different approach to the Handlebars.js version (e.g. statically typed Handlebars-Templates), the compiler needs to know for which type your Handlebars-Template is meant.
This information is communicated to the compiler by a special ModelToken. Its syntax is straightforward:
```Handlebars
{{model NamespacePath.ClassName}}
```
For example:
```Handlebars
{{model HandlebarsTest.TestViewModel}}
```

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

## Building
Following prerequisites are needed in order to be able to build the solution:
+ Visual Studio 2015
+ Visual Studio 2015 Extensibility Tools
