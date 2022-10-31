# Xperience Page Template Utilities

[![NuGet Package](https://img.shields.io/nuget/v/XperienceCommunity.PageTemplateUtilities.svg)](https://www.nuget.org/packages/XperienceCommunity.PageTemplateUtilities)

Utilities to help quickly create and register MVC Page Templates in Kentico Xperience

## Dependencies

This package is compatible with ASP.NET Core 3.1 -> ASP.NET Core 5 and is designed to be used with .NET Core / .NET 5 applications integrated with Kentico Xperience 13.0.

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
       typeof(HomePageTemplateProperties),
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
`Sandbox.HomePage` Page Types. It would need to be changed to `Sandbox.HomePage_Featured` to match the default filter, which follows the pattern of matching Page Templates with a Identifier prefix of `$"{PageTypeClassName}_"`.

```csharp
[assembly: RegisterPageTemplate(
    "Sandbox.HomePage_Default",
    "Home Page (Default)",
    typeof(HomePageTemplateProperties),
    "~/Features/Home/Sandbox.HomePage_Default.cshtml")]

[assembly: RegisterPageTemplate(
    "Sandbox_PageTemplate_HomePage_Featured",
    "Home Page (Featured)",
    typeof(HomePageTemplateProperties),
    "~/Features/Home/Sandbox_HomePage_Featured.cshtml")]

public class HomePageTemplateFilter : PageTypePageTemplateFilter
{
    public override string PageTypeClassName => HomePage.CLASS_NAME;
}
```

The filter can be overridden to allow you to match however you would like. It has a signature of:

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

## Troubleshooting

### InvalidOperationException: The view on path `'~/Views/Shared/PageTypes/<ClassName>.cshtml'` could not be found and there is no page template registered for selected page

#### Live Site

This error is caused by the `CMS_Document.DocumentPageTemplateConfiguration` column not being populated for the Page you are trying to display.

`'~/Views/Shared/PageTypes/<ClassName>.cshtml'` is the default path for Basic ("Route-to-View") [Content Tree Routing](https://docs.xperience.io/developing-websites/implementing-routing/content-tree-based-routing/setting-up-content-tree-based-routing#Settingupcontenttreebasedrouting-BasicSettingupbasicrouting). Xperience uses this View path when no `[RegisterPageRoute]` attributes are found and the Page has no Page Template configuration.

If this error occurs on the Live Site, you need to update this column with a value that looks like the following:

```json
{
  "identifier": "<PageTemplateIdentifier>",
  "properties": {}
}
```

You can expose the `DocumentPageTemplateConfiguration` column in your Page "Content" tab by [adding it as a Page field to the Page's Page Type](https://docs.xperience.io/developing-websites/defining-website-content-structure/managing-page-types/creating-page-types#Creatingpagetypes-Step4–Fields) using a Text Area Form Control. After you add the value (and publish the Page if it's under workflow) you can remove the field from the Page Type.

#### Administration

If this error _only_ appears when trying to view the Page in the Page Builder, it means you have [Page versioning enabled](https://docs.xperience.io/x/9Q_RBg) and the value of the Page's "Document" fields in the `CMS_VersionHistory` table do not have a populated `DocumentPageTemplateConfiguration`. You should follow the steps in [the Live Site instructions](#live-site) and Save/Publish the latest version of the Page with the `DocumentPageTemplateConfiguration` populated.

## Contributing

To build this project, you must have v6.0.300 or higher
of the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) installed.

If you've found a bug or have a feature request, please [open an issue](https://github.com/wiredviews/xperience-page-template-utilities/issues/new) on GitHub.

If you'd like to make a contribution, you can create a [PR on GitHub](https://github.com/wiredviews/xperience-page-template-utilities/compare).

## References

### Kentico Xperience

- [Kentico Xperience Design Patterns: MVC is Dead, Long Live PTVC](https://dev.to/seangwright/kentico-xperience-design-patterns-mvc-is-dead-long-live-ptvc-4635#building-pages-with-page-templates)
- [Filtering Page Templates](https://docs.xperience.io/developing-websites/page-builder-development/developing-page-templates/filtering-page-templates)
