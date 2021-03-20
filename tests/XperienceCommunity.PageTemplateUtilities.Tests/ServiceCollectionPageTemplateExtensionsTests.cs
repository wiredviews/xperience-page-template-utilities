using Kentico.PageBuilder.Web.Mvc.PageTemplates;
using FluentAssertions;
using Xunit;
using Kentico.PageBuilder.Web.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace XperienceCommunity.PageTemplateUtilities.Tests
{
    public class DefaultPageTemplateFilterTests
    {
        [Fact]
        public void AddPageTemplateFilters_Will_Register()
        {
            var services = new ServiceCollection();

            services.AddPageTemplateFilters(Assembly.GetExecutingAssembly());

            var filters = PageBuilderFilters.PageTemplates;

            filters.Should().NotBeNull().And.HaveCount(2);

            filters.Any(f => f.GetType().IsAssignableTo(typeof(NonDefaultPageTemplateFilter))).Should().BeTrue();
            filters.Any(f => f.GetType().IsAssignableTo(typeof(HomePageTemplateFilter))).Should().BeTrue();
        }
    }

    public class HomePageTemplateFilter : PageTypePageTemplateFilter
    {
        public override string PageTypeClassName => "Sandbox.HomePage";
    }

    public class NonDefaultPageTemplateFilter : IPageTemplateFilter
    {
        public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context) =>
            pageTemplates;
    }
}
