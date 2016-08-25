﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WikiClientLibrary.Generators
{
    /// <summary>
    /// Get a list provided by a QueryPage-based special page. (MediaWiki 1.18)
    /// </summary>
    /// <remarks>See https://www.mediawiki.org/wiki/API:Querypage .</remarks>
    public class QueryPageGenerator : PageGenerator<Page>
    {
        public QueryPageGenerator(Site site) : base(site)
        {
        }

        public QueryPageGenerator(Site site, string queryPageName) : base(site)
        {
            QueryPageName = queryPageName;
        }

        /// <summary>
        /// The name of the special page. Note, this is case sensitive.
        /// </summary>
        public string QueryPageName { get; set; }

        /// <summary>
        /// When overridden, fills generator parameters for action=query request.
        /// </summary>
        /// <returns>The dictioanry containing request value pairs.</returns>
        protected override IEnumerable<KeyValuePair<string, object>> GetGeneratorParams()
        {
            if (string.IsNullOrWhiteSpace(QueryPageName))
                throw new InvalidOperationException("Invalid QueryPageName.");
            return new Dictionary<string, object>
            {
                {"generator", "querypage"},
                {"gqppage", QueryPageName},
                {"gqplimit", ActualPagingSize}
            };
        }

        /// <summary>
        /// 返回表示当前对象的字符串。
        /// </summary>
        /// <returns>
        /// 表示当前对象的字符串。
        /// </returns>
        public override string ToString()
        {
            return "QueryPage:" + QueryPageName;
        }
    }
}
