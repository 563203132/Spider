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
        static SpiderEngine _spider;

        static void Main(string[] args)
        {
            _spider = new SpiderEngine();
            _spider.Run();

            //SqlHelper.InitResource();

            Console.ReadLine();
        }

        static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            _spider.Stop();
        }
    }
}
