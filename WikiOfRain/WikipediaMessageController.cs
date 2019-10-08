using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebHtmlParser;

namespace WikiOfRain
{
    class WikipediaMessageController
    {
        private readonly string url;
        private readonly int messageInterval;
        private CancellationTokenSource cancellationTokenSource;
        private System.Timers.Timer Timer;
        private string[] ParsedPage;

        public WikipediaMessageController(string url, int messageInterval)
        {
            this.url = url;
            this.messageInterval = messageInterval;
        }

        private int index = 0;
        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {   
            if(index < ParsedPage.Length)
            {
                Message.SendColoured(ParsedPage[index], Colours.Pink);
                index++;
            }
            else
            {
                Message.SendColoured("Wiki End.", Colours.Green);
                Stop();
            }
        }

        public async void Start()
        {      
            ParsedPage = (await ReadPage()).ToArray<string>();
            if (!cancellationTokenSource.IsCancellationRequested)
            {
                Timer = new System.Timers.Timer
                {
                    AutoReset = true,
                    Enabled = false,
                    Interval = messageInterval * 1000
                };
                Timer.Elapsed += Timer_Elapsed;
                Timer.Start();
            }            
        }

        public void Stop()
        {
            ParsedPage = null;
            Timer.Dispose();
            cancellationTokenSource.Cancel();
        }

        private async Task<IEnumerable<string>> ReadPage()
        {
            cancellationTokenSource = new CancellationTokenSource();
            var reader = new WebReader(cancellationTokenSource.Token);
            var document = await reader.Read(url);
            var parser = new HtmlDataParser();
            return parser.ConvertToIEnumerable(document);
        }
    }
}
