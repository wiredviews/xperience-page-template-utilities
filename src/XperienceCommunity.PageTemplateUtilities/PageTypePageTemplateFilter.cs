using System;
using System.Collections.Generic;
using System.Linq;
using CMS.DocumentEngine;

namespace Kentico.PageBuilder.Web.Mvc.PageTemplates
{
    /// <summary>
    /// Filter for associating Page Types to Page Templates
    /// </summary>
    public abstract class PageTypePageTemplateFilter : IPageTemplateFilter
    {
        /// <summary>
        /// If true, then the templates returned by <see cref="PageTemplateFilterBy"/> are excluded from the results of <see cref="Filter(IEnumerable{PageTemplateDefinition}, PageTemplateFilterContext)"/>
        /// when the <see cref="PageTypeClassName"/> does not match the Page Type of the context.
        /// Defaults to true.
        /// </summary>
        /// <remarks>
        /// When true, this filter excludes any Page Templates that would be matched from being used with any other Page Type
        /// </remarks>
        public virtual bool ExcludeIfNoMatch { get; } = true;

        /// <summary>
        /// The <see cref="TreeNode.ClassName"/> used to compare to the <see cref="PageTemplateFilterContext.PageType"/>
        /// </summary>
        public abstract string PageTypeClassName { get; }

        /// <summary>
        /// The delegate used to determine if a Page Template matches for the given 
        /// <see cref="PageTemplateDefinition" />, <see cref="PageTemplateFilterContext" />, and <see cref="PageTypeClassName" />.
        /// By default it matches the <see cref="PageTemplateDefinition" /> Identifier against the <see cref="PageTypeClassName" />
        /// as a prefix.
        /// </summary>
        /// <example>
        /// Matches:
        /// PageTypeClassName = "Sandbox.HomePage";
        /// PageTemplateDefinition Identifier = "Sandbox.HomePage_";
        /// </example>
        public virtual Func<PageTemplateDefinition, PageTemplateFilterContext, string, bool> PageTemplateFilterBy { get; } =
            (definition, _ctx, className) => definition.Identifier.StartsWith($"{className}_", StringComparison.OrdinalIgnoreCase);

        /// <inheritdoc cref="IPageTemplateFilter" />
        public IEnumerable<PageTemplateDefinition> Filter(IEnumerable<PageTemplateDefinition> pageTemplates, PageTemplateFilterContext context)
        {
            if (context.PageType.Equals(PageTypeClassName, StringComparison.InvariantCultureIgnoreCase))
            {
                return pageTemplates.Where(d => PageTemplateFilterBy(d, context, PageTypeClassName));
            }

            if (ExcludeIfNoMatch)
            {
                return pageTemplates.Where(d => !PageTemplateFilterBy(d, context, PageTypeClassName));
            }

            return pageTemplates;
        }
    }
}