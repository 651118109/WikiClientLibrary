using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Infrastructures;
using WikiClientLibrary.Sites;
using WikiClientLibrary.Pages.Queries;
using WikiClientLibrary.Pages.Queries.Properties;

namespace WikiClientLibrary.Pages
{
    // Factory methods
    partial class WikiPage
    {

        /// <summary>
        /// Creates a list of <see cref="WikiPage"/> based on JSON query result.
        /// </summary>
        /// <param name="site">A <see cref="Site"/> object.</param>
        /// <param name="queryNode">The <c>qurey</c> node value object of JSON result.</param>
        /// <param name="options"></param>
        /// <returns>Retrieved pages.</returns>
        internal static IList<WikiPage> FromJsonQueryResult(WikiSite site, JObject queryNode, IWikiPageQueryProvider options)
        {
            if (site == null) throw new ArgumentNullException(nameof(site));
            if (queryNode == null) throw new ArgumentNullException(nameof(queryNode));
            var pages = (JObject)queryNode["pages"];
            if (pages == null) return EmptyPages;
            // If query.xxx.index exists, sort the pages by the given index.
            // This is specifically used with SearchGenerator, to keep the search result in order.
            // For other generators, this property simply does not exist.
            // See https://www.mediawiki.org/wiki/API_talk:Query#On_the_order_of_titles_taken_out_of_generator .
            return pages.Properties().OrderBy(page => (int?)page.Value["index"])
                .Select(page =>
                {
                    var newInst = new WikiPage(site, 0);
                    MediaWikiHelper.PopulatePageFromJson(newInst, (JObject)page.Value, options);
                    return newInst;
                }).ToList();
        }

    }
}
