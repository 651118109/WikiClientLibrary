﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using WikiClientLibrary.Client;
using WikiClientLibrary.Generators.Primitive;
using WikiClientLibrary.Pages;
using WikiClientLibrary.Sites;

namespace WikiClientLibrary.Generators
{

    /// <summary>
    /// Represents a generator (or iterator) of <see cref="Revision"/> on a specific page.
    /// </summary>
    public class RevisionsGenerator : WikiPagePropertyGenerator<Revision, WikiPage>
    {

        /// <inheritdoc />
        public RevisionsGenerator(WikiSite site) : base(site)
        {
        }

        public RevisionsGenerator(WikiSite site, string pageTitle) : base(site)
        {
            PageTitle = pageTitle;
        }

        /// <inheritdoc />
        public override string PropertyName => "revisions";

        /// <inheritdoc />
        public override IEnumerable<KeyValuePair<string, object>> EnumListParameters()
        {
            return new Dictionary<string, object>
            {
                {"rvlimit", PaginationSize},
                {"rvdir", TimeAscending ? "newer" : "older"},
                {"rvstart", StartTime},
                {"rvend", EndTime},
                {"rvstartid", StartRevisionId},
                {"rvendid", EndRevisionId},
                {"rvuser", UserName},
                {"rvexcludeuser", ExcludedUserName},
                {"rvprop", RequestHelper.GetRvProp(RevisionOptions)},
            };
        }

        /// <inheritdoc />
        protected override Revision ItemFromJson(JToken json, JObject jpage)
        {
            var rev = json.ToObject<Revision>(Utility.WikiJsonSerializer);
            rev.Page = new WikiPageStub((int)jpage["pageid"], (string)jpage["title"], (int)jpage["ns"]);
            return rev;
        }

        /// <summary>
        /// Whether to list revisions in an ascending order of time.
        /// </summary>
        /// <value><c>true</c>, if oldest revisions are listed first; or <c>false</c>, if newest revisions are listed first.</value>
        /// <remarks>
        /// Any specified <see cref="StartTime"/> value must be later than any specified <see cref="EndTime"/> value.
        /// This requirement is reversed if <see cref="TimeAscending"/> is <c>true</c>.
        /// </remarks>
        public bool TimeAscending { get; set; } = false;

        /// <summary>
        /// The timestamp to start listing from.
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// The timestamp to end listing at.
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// Revision ID to start listing from.
        /// </summary>
        public int? StartRevisionId { get; set; }

        /// <summary>
        /// Revision ID to stop listing at. 
        /// </summary>
        public int? EndRevisionId { get; set; }

        /// <summary>
        /// Only list revisions made by this user.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Do not list revisions made by this user.
        /// </summary>
        public string ExcludedUserName { get; set; }

        /// <summary>
        /// Gets/sets the page query options for <see cref="WikiPagePropertyList{T}.EnumItemsAsync"/>
        /// </summary>
        public PageQueryOptions RevisionOptions { get; set; }

        /// <inheritdoc />
        /// <summary>Infrastructure. Not intended to be used directly in your code.
        /// Asynchronously enumerates the pages from generator.</summary>
        /// <remarks>
        /// Using <c>revisions</c> as generator is not supported until MediaWiki 1.25.
        /// Usually this generator will only returns the title specified in
        /// <see cref="WikiPagePropertyList{T}.PageTitle"/> or <see cref="WikiPagePropertyList{T}.PageId"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IAsyncEnumerable<WikiPage> EnumPagesAsync()
        {
            return base.EnumPagesAsync();
        }

        /// <inheritdoc />
        /// <summary>Infrastructure. Not intended to be used directly in your code.
        /// Asynchronously enumerates the pages from generator.</summary>
        /// <remarks>
        /// Using <c>revisions</c> as generator is not supported until MediaWiki 1.25.
        /// Usually this generator will only returns the title specified in
        /// <see cref="WikiPagePropertyList{T}.PageTitle"/> or <see cref="WikiPagePropertyList{T}.PageId"/>.
        /// </remarks>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override IAsyncEnumerable<WikiPage> EnumPagesAsync(PageQueryOptions options)
        {
            return base.EnumPagesAsync(options);
        }
    }
}
