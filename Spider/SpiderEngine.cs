using AbotX.Parallel;
using AbotX.Poco;
using log4net.Config;
using System;
using System.Collections.Generic;
using AbotX.Core;
using Abot.Poco;
using System.Net;

namespace Spider
{
    public class SpiderEngine
    {
        public ParallelCrawlerEngine _crawlerEngine;

        public SpiderEngine()
        {
            CrawlConfigurationX config = AbotXConfigurationSectionHandler.LoadFromXml().Convert();
            XmlConfigurator.Configure();//So the logger 

            var siteToCrawlProvider = new SiteToCrawlProvider();
            siteToCrawlProvider.AddSitesToCrawl(new List<SiteToCrawl>
            {
                new SiteToCrawl{ Uri = new Uri("https://www.baidu.com/s?wd=乙肝 症状&pn=0"), SiteBag = "症状" },
                new SiteToCrawl{ Uri = new Uri("https://www.baidu.com/s?wd=乙肝 症状&pn=10"), SiteBag = "症状" },
                new SiteToCrawl{ Uri = new Uri("https://www.baidu.com/s?wd=乙肝 症状&pn=20"), SiteBag = "症状" },
            });

            //Create the crawl engine instance
            var impls = new ParallelImplementationOverride(
                config,
                new ParallelImplementationContainer
                {
                    SiteToCrawlProvider = siteToCrawlProvider
                }
            );

            _crawlerEngine = new ParallelCrawlerEngine(config, impls);

            //Register for site level events
            _crawlerEngine.AllCrawlsCompleted += (sender, eventArgs) =>
            {
                Console.WriteLine("Completed crawling all sites");
            };
            _crawlerEngine.SiteCrawlCompleted += (sender, eventArgs) =>
            {
                Console.WriteLine("Completed crawling site {0}", eventArgs.CrawledSite.SiteToCrawl.Uri);
            };
            _crawlerEngine.CrawlerInstanceCreated += (sender, eventArgs) =>
            {
                eventArgs.Crawler.CrawlBag = eventArgs.SiteToCrawl.SiteBag;
                //Register for crawler level events. These are Abot's events!!!
                eventArgs.Crawler.PageCrawlCompleted += (abotSender, abotEventArgs) =>
                {
                    CrawledPage crawledPage = abotEventArgs.CrawledPage;

                    if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
                        Console.WriteLine("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri);
                    else
                    {
                        if (string.IsNullOrEmpty(crawledPage.Content.Text))
                            Console.WriteLine("Page had no content {0}", crawledPage.Uri.AbsoluteUri);
                        else
                        {
                            Console.WriteLine("Depth: {0} --- Crawl of page succeeded {1}", crawledPage.CrawlDepth, crawledPage.Uri.AbsoluteUri);
                            var item = new CrawledItem()
                            {
                                Name = "123",
                                Url = crawledPage.Uri.AbsoluteUri,
                                Detail = crawledPage.Content.Text
                            };

                            SqlHelper.Store(new System.Collections.Generic.List<CrawledItem>() { item });
                        }
                    }

                    //var htmlAgilityPackDocument = crawledPage.HtmlDocument; //Html Agility Pack parser
                    //var angleSharpHtmlDocument = crawledPage.AngleSharpHtmlDocument; //AngleSharp parser
                };
                eventArgs.Crawler.ShouldCrawlPage((pageToCrawl, crawlContext) =>
                {
                    CrawlDecision decision = new CrawlDecision { Allow = true };

                    var isCrawlDepth1 = pageToCrawl.CrawlDepth == 0 && !pageToCrawl.Uri.AbsoluteUri.Contains("www.baidu.com/s?wd");
                    var isCrawlDepth2 = pageToCrawl.CrawlDepth == 1 && !pageToCrawl.Uri.AbsoluteUri.Contains("www.baidu.com/link");

                    if (isCrawlDepth1 || isCrawlDepth2)
                        return new CrawlDecision { Allow = false, Reason = "Dont want to crawl google pages" };

                    return decision;
                });
            };
        }

        public void Run()
        {
            _crawlerEngine.StartAsync();
        }
    }
}
