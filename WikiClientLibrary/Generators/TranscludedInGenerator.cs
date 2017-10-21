﻿using System;
using System.Collections.Generic;
using System.Text;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace WikiClientLibrary.Generators
{
    /// <summary>
    /// Generates all the pages that transclude the specified title.
    /// </summary>
    /// <seealso cref="BacklinksGenerator"/>
    public class TranscludedInGenerator : WikiPageGenerator<WikiPage>
    {

        public TranscludedInGenerator(WikiSite site) : base(site)
        {
        }

        public TranscludedInGenerator(WikiSite site, string targetTitle) : base(site)
        {
            TargetTitle = targetTitle;
        }

        /// <summary>
        /// List pages transcluding this title. The title does not need to exist.
        /// </summary>
        public string TargetTitle { get; set; }

        /// <summary>
        /// Only list pages in these namespaces.
        /// </summary>
        /// <value>Selected ids of namespace, or <c>null</c> if all the namespaces are selected.</value>
        public IEnumerable<int> NamespaceIds { get; set; }

        /// <summary>
        /// How to filter redirects in the results.
        /// </summary>
        public PropertyFilterOption RedirectsFilter { get; set; }

        /// <inheritdoc />
        public override string ListName => "embeddedin";

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> EnumListParameters()
        {
            return new Dictionary<string, object>
            {
                {"eititle", TargetTitle},
                {"einamespace", NamespaceIds == null ? null : string.Join("|", NamespaceIds)},
                {"eifilterredir", RedirectsFilter.ToString("redirects", "nonredirects")},
                {"eilimit", PaginationSize}
            };
        }
    }
}
