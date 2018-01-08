using Abot.Crawler;
using Abot.Poco;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public class WebSpider
    {
        private PoliteWebCrawler _crawler;

        public WebSpider()
        {
            _crawler = new PoliteWebCrawler();

            _crawler.PageCrawlStartingAsync += crawler_ProcessPageCrawlStarting;
            _crawler.PageCrawlCompletedAsync += crawler_ProcessPageCrawlCompleted;
            _crawler.PageCrawlDisallowedAsync += crawler_PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += crawler_PageLinksCrawlDisallowed;

            _crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
            {
                CrawlDecision decision = new CrawlDecision { Allow = true };

                var isCrawlDepth1 = pageToCrawl.CrawlDepth == 0 && !pageToCrawl.Uri.AbsoluteUri.Contains("www.baidu.com/s?wd");
                var isCrawlDepth2 = pageToCrawl.CrawlDepth == 1 && !pageToCrawl.Uri.AbsoluteUri.Contains("www.baidu.com/link");

                if (isCrawlDepth1 || isCrawlDepth2)
                    return new CrawlDecision { Allow = false, Reason = "Dont want to crawl google pages" };

                return decision;
            });
        }

        public void Run()
        {
            _crawler.Crawl(new Uri("https://www.baidu.com/s?wd=乙肝 症状"));
        }

        void crawler_ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            //PageToCrawl pageToCrawl = e.PageToCrawl;
            //Console.WriteLine("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri);
        }

        void crawler_ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
            else
                Console.WriteLine("Depth: {0} --- Crawl of page succeeded {1}", crawledPage.CrawlDepth, crawledPage.Uri.AbsoluteUri);

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
                Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);

            var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
            var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser
        }

        void crawler_PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            //CrawledPage crawledPage = e.CrawledPage;
            //Console.WriteLine("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason);
        }

        void crawler_PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            //PageToCrawl pageToCrawl = e.PageToCrawl;
            //Console.WriteLine("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason);
        }
    }
}
