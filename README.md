# Xperience Page Template Utilities

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.PageTemplateUtilities.svg)](https://www.nuget.org/packages/XperienceCommunity.PageTemplateUtilities)

Utilities to help quickly create and register MVC Page Templates in Kentico Xperience

## Dependencies

This package is compatible with ASP.NET Core 3.1 -> ASP.NET Core 5 and is designed to be used with .NET Core / .NET 5 Console applications integrated with Kentico Xperience 13.0.

## How to Use?

1. First, install the NuGet package in your ASP.NET Core project

   ```bash
   dotnet add package XperienceCommunity.PageTemplateUtilities
   ```

1. Create an implementation of `PageTypePageTemplateFilter` for a given Page Type and register some Page Templates

   ```csharp
   [assembly: RegisterPageTemplate(
       "Sandbox.HomePage_Default",
       "Home Page (Default)",
       typeof(BasicContentPageTemplateProperties),
       "~/Features/Home/Sandbox.HomePage_Default.cshtml")]

   public class HomePageTemplateFilter : PageTypePageTemplateFilter
   {
        public override string PageTypeClassName => "Sandbox.HomePage";
   }
   ```

1. Register all `IPageTemplateFilter` implementations with the `IServiceCollection`

   ```csharp
   public class Startup
   {
       public void ConfigureServices(IServiceCollection services)
       {
           services.AddPageTemplateFilters(Assembly.GetExecutingAssembly());
       }
   }
   ```

## How Does It Work?

The `PageTypePageTemplateFilter` is simplest if your Page Templates follow some conventions, but allows for full customization.

In the example below, `Home Page (Default)` will match the filter and be allowed for selection for `Sandbox.HomePage`
Page Types, but `Home Page (Featured)` does not have an Identifier (`Sandbox_PageTemplate_HomePage_Featured`)
that matches the prefix of `Sandbox.HomePage_`, so it will not be displayed for selection for
`Sandbox.HomePage` Page Types. It would need to be changed to `Sandbox.HomePage_Featured` to match the default filter.

```csharp
[assembly: RegisterPageTemplate(
    "Sandbox.HomePage_Default",
    "Home Page (Default)",
    typeof(BasicContentPageTemplateProperties),
    "~/Features/Home/Sandbox.HomePage_Default.cshtml")]

[assembly: RegisterPageTemplate(
    "Sandbox_PageTemplate_HomePage_Featured",
    "Home Page (Featured)",
    typeof(BasicContentPageTemplateProperties),
    "~/Features/Home/Sandbox_HomePage_Featured.cshtml")]

public class HomePageTemplateFilter : PageTypePageTemplateFilter
{
    public override string PageTypeClassName => HomePage.CLASS_NAME;
}
```

The filter can be override to allow you to match however you would like and has a signature of:

```csharp
Func<PageTemplateDefinition, PageTemplateFilterContext, string, bool> PageTemplateFilterBy
```

The customization below would match an Identifier like `_Sandbox.HomePage-Default`:

```csharp
public class HomePageTemplateFilter : PageTypePageTemplateFilter
{
    public override string PageTypeClassName => HomePage.CLASS_NAME;

    public override Func<PageTemplateDefinition, PageTemplateFilterContext, string, bool> PageTemplateFilterBy { get; } =
        (definition, context, className) => definition.Identifier.StartsWith($"_{className}-", StringComparison.OrdinalIgnoreCase);
}
```

We could even skip using the `className` entirely and match against the `definition` or `context` with some hardcoded values,
but at that point it's probably best to implement the `IPageTemplateFilter` directly.

## References

### Kentico Xperience

- [Filtering Page Templates](https://docs.xperience.io/developing-websites/page-builder-development/developing-page-templates/filtering-page-templates)
