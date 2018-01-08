using Abot.Crawler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            //Will use app.config for configuration
            SpiderEngine spider = new SpiderEngine();

            spider.Run();

            Console.ReadLine();
        }
    }
}
