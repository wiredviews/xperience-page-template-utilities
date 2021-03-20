using System;
using System.Linq;
using System.Reflection;
using Kentico.PageBuilder.Web.Mvc;
using Kentico.PageBuilder.Web.Mvc.PageTemplates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionPageTemplateExtensions
    {
        /// <summary>
        /// Adds all existing Page Template Filters to the <see cref="PageBuilderFilters.PageTemplates"/> collection via reflection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembiles"></param>
        /// <returns></returns>
        /// <example>
        /// services.AddPageTemplateFilters(Assembly.GetExecutingAssembly())
        /// </example>
        public static IServiceCollection AddPageTemplateFilters(this IServiceCollection services, params Assembly[] assembiles)
        {
            var types = assembiles.SelectMany(a => a.GetExportedTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface && typeof(IPageTemplateFilter).IsAssignableFrom(t)));

            foreach (var t in types)
            {
                object? instance = Activator.CreateInstance(t);

                if (instance is IPageTemplateFilter filter)
                {
                    PageBuilderFilters.PageTemplates.Add(filter);
                }
            }

            return services;
        }
    }
}