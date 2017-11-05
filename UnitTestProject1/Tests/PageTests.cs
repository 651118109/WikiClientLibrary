﻿// Enables the following conditional switch in the project options
// to prevent test cases from making any edits.
//          DRY_RUN

using System;
using System.Linq;
using System.Threading.Tasks;
using WikiClientLibrary;
using WikiClientLibrary.Generators;
using WikiClientLibrary.Pages;
using Xunit;
using Xunit.Abstractions;

namespace UnitTestProject1.Tests
{

    public class PageTests : WikiSiteTestsBase
    {
        private const string SummaryPrefix = "WikiClientLibrary test. ";


        /// <inheritdoc />
        public PageTests(ITestOutputHelper output) : base(output)
        {
            SiteNeedsLogin(Endpoints.WikipediaTest2);
            SiteNeedsLogin(Endpoints.WikiaTest);
            SiteNeedsLogin(Endpoints.WikipediaLzh);
        }

        [Fact]
        public async Task WpTest2PageReadTest1()
        {
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "project:sandbox");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            ShallowTrace(page);
            Assert.True(page.Exists);
            Assert.Equal("Wikipedia:Sandbox", page.Title);
            Assert.Equal(4, page.NamespaceId);
            Assert.Equal("en", page.PageLanguage);
            // Chars vs. Bytes
            Assert.True(page.Content.Length <= page.ContentLength);
            Output.WriteLine(new string('-', 10));
            page = new WikiPage(site, "file:inexistent_file.jpg");
            await page.RefreshAsync();
            ShallowTrace(page);
            Assert.False(page.Exists);
            Assert.Equal("File:Inexistent file.jpg", page.Title);
            Assert.Equal(6, page.NamespaceId);
            Assert.Equal("en", page.PageLanguage);
        }

        [Fact]
        public async Task WpTest2PageReadTest2()
        {
            var site = await WpTest2SiteAsync;
            var search = await site.OpenSearchAsync("A", 10);
            var pages = search.Select(e => new WikiPage(site, e.Title)).ToList();
            await pages.RefreshAsync();
            ShallowTrace(pages);
        }

        [Fact]
        public async Task WpTest2PageReadRedirectTest()
        {
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "Foo");
            await page.RefreshAsync();
            Assert.True(page.IsRedirect);
            var target = await page.GetRedirectTargetAsync();
            ShallowTrace(target);
            Assert.Equal("Foo24", target.Title);
            Assert.True(target.RedirectPath.SequenceEqual(new[] { "Foo", "Foo2", "Foo23" }));
        }

        [Fact]
        public async Task WpTest2PageGeoCoordinateTest()
        {
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "France");
            await page.RefreshAsync(PageQueryOptions.FetchGeoCoordinate);
            ShallowTrace(page);
            Assert.False(page.PrimaryCoordinate.IsEmpty);
            Assert.Equal(47, page.PrimaryCoordinate.Latitude, 12);
            Assert.Equal(2, page.PrimaryCoordinate.Longitude, 12);
            Assert.Equal(GeoCoordinate.Earth, page.PrimaryCoordinate.Globe);
        }

        [Fact]
        public async Task WpLzhPageExtractTest()
        {
            var site = await WpLzhSiteAsync;
            var page = new WikiPage(site, "莎拉伯恩哈特");
            await page.RefreshAsync(PageQueryOptions.FetchExtract);
            ShallowTrace(page);
            Assert.Equal("莎拉·伯恩哈特，一八四四年生，法國巴黎人也。", page.Extract);
        }

        [Fact]
        public async Task WpLzhPageReadDisambigTest()
        {
            var site = await WpLzhSiteAsync;
            var page = new WikiPage(site, "莎拉伯恩哈特");
            await page.RefreshAsync();
            Assert.True(await page.IsDisambiguationAsync());
        }

        [Fact]
        public async Task WpLzhFetchRevisionsTest()
        {
            var site = await WpLzhSiteAsync;
            var revIds = new[] { 248199, 248197, 255289 };
            var pageTitles = new[] { "清", "清", "香草" };
            var rev = await Revision.FetchRevisionsAsync(site, revIds).ToList();
            ShallowTrace(rev);
            Assert.Equal(revIds, rev.Select(r => r.Id));
            Assert.Equal(pageTitles, rev.Select(r => r.Page.Title));
            // Asserts that pages with the same title shares the same reference
            // Or an Exception will raise.
            var pageDict = rev.Select(r => r.Page).Distinct().ToDictionary(p => p.Title);
        }

        [Fact]
        public async Task WpLzhFetchFileTest()
        {
            var site = await WpLzhSiteAsync;
            var file = new FilePage(site, "File:Empress Suiko.jpg");
            await file.RefreshAsync();
            ShallowTrace(file);
            //Assert.True(file.Exists);   //It's on WikiMedia!
            Assert.Equal(58865, file.LastFileRevision.Size);
            Assert.Equal("7aa12c613c156dd125212d85a072b250625ae39f", file.LastFileRevision.Sha1.ToLowerInvariant());
        }

        [Fact]
        public async Task WikiaPageReadTest()
        {
            var site = await WikiaTestSiteAsync;
            var page = new WikiPage(site, "Project:Sandbox");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            Assert.Equal("Mediawiki 1.19 test Wiki:Sandbox", page.Title);
            Assert.Equal(4, page.NamespaceId);
            ShallowTrace(page);
        }

        [Fact]
        public async Task WikiaPageReadDisambigTest()
        {
            var site = await WikiaTestSiteAsync;
            var page = new WikiPage(site, "Test (Disambiguation)");
            await page.RefreshAsync();
            Assert.True(await page.IsDisambiguationAsync());
        }

        [Fact]
        public async Task WpLzhRedirectedPageReadTest()
        {
            var site = await WpLzhSiteAsync;
            var page = new WikiPage(site, "project:sandbox");
            await page.RefreshAsync(PageQueryOptions.ResolveRedirects);
            Assert.Equal("維基大典:沙盒", page.Title);
            Assert.Equal(4, page.NamespaceId);
            ShallowTrace(page);
        }

        [SkippableFact]
        public async Task WpTest2PageWriteTest1()
        {
            AssertModify();
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "project:sandbox");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            page.Content += "\n\nTest from WikiClientLibrary.";
            Output.WriteLine(page.Content);
            await page.UpdateContentAsync(SummaryPrefix + "Edit sandbox page.");
        }

        [SkippableFact]
        public async Task WpTest2PageWriteTest2()
        {
            AssertModify();
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "Test page");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            Assert.True(page.Protections.Any(), "To perform this test, the working page should be protected.");
            page.Content += "\n\nTest from WikiClientLibrary.";
            await Assert.ThrowsAsync<UnauthorizedOperationException>(() =>
                page.UpdateContentAsync(SummaryPrefix + "Attempt to edit a protected page."));
        }

        [SkippableFact]
        public async Task WpTest2PageWriteTest3()
        {
            AssertModify();
            var site = await WpTest2SiteAsync;
            var page = new WikiPage(site, "Special:RecentChanges");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            Assert.True(page.IsSpecialPage);
            page.Content += "\n\nTest from WikiClientLibrary.";
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                page.UpdateContentAsync(SummaryPrefix + "Attempt to edit a special page."));
        }

        [SkippableFact]
        public async Task WpTest2BulkPurgeTest()
        {
            AssertModify();
            var site = await WpTest2SiteAsync;
            // Usually 500 is the limit for normal users.
            var pages = await new AllPagesGenerator(site) { PaginationSize = 300 }.EnumPagesAsync().Take(300).ToList();
            var badPage = new WikiPage(site, "Inexistent page title");
            pages.Insert(pages.Count / 2, badPage);
            Output.WriteLine("Attempt to purge: ");
            ShallowTrace(pages, 1);
            // Do a normal purge. It may take a while.
            var failedPages = await pages.PurgeAsync();
            Output.WriteLine("Failed pages: ");
            ShallowTrace(failedPages, 1);
            Assert.Equal(1, failedPages.Count);
            Assert.Same(badPage, failedPages.Single());
        }

        [SkippableFact]
        public async Task WpTest2PagePurgeTest()
        {
            AssertModify();
            var site = await WpTest2SiteAsync;
            // We do not need to login.
            var page = new WikiPage(site, "project:sandbox");
            var result = await page.PurgeAsync(PagePurgeOptions.ForceLinkUpdate | PagePurgeOptions.ForceRecursiveLinkUpdate);
            Assert.True(result);
            // Now an ArgumentException should be thrown from Page.ctor.
            //page = new Page(site, "special:");
            //result = AwaitSync(page.PurgeAsync());
            //Assert.False(result);
            page = new WikiPage(site, "the page should be inexistent");
            result = await page.PurgeAsync();
            Assert.False(result);
        }

        [SkippableFact]
        public async Task WikiaPageWriteTest1()
        {
            AssertModify();
            var site = await WikiaTestSiteAsync;
            Utility.AssertLoggedIn(site);
            var page = new WikiPage(site, "project:sandbox");
            await page.RefreshAsync(PageQueryOptions.FetchContent);
            page.Content += "\n\nTest from WikiClientLibrary.";
            Output.WriteLine(page.Content);
            await page.UpdateContentAsync(SummaryPrefix + "Edit sandbox page.");
        }
    }
}
