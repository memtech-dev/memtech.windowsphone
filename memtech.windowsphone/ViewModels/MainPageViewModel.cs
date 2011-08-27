using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ServiceModel.Syndication;
using System.Xml;
using System.IO;
using System.Linq;

namespace memtech.windowsphone.ViewModels
{
    public class MainPageViewModel
    {
        public ObservableCollection<NewsItem> NewsItems { get; set; }

        public MainPageViewModel()
        {            
            NewsItems = new ObservableCollection<NewsItem>();
        }

        internal void LoadNewsItems()
        {
            var webClient = new WebClient();
            
            webClient.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    MessageBox.Show("An error occurred downloading the news :(");
                    return;
                }

                Stream stream = e.Result;
                var feedXml = XmlReader.Create(stream);
                var feed = SyndicationFeed.Load(feedXml);
                foreach (var feedItem in feed.Items)
                {
                    NewsItems.Add(new NewsItem
                    {
                        Headline = feedItem.Title.Text,
                        Date = feedItem.PublishDate.Date,       
                        // todo: fix this in a better way
                        Uri = feedItem.Links.Single(x => x.RelationshipType == "alternate").Uri.AbsoluteUri
                    });
                }
            };

            // todo: hopefully get rid of this RSS crap and make the memtech-service the backend instead...
            // it should allow an admin of some sort to add RSS links to aggregate news
            // it should also allow us to push our own custom news down (maybe with push notificatons?)
            webClient.OpenReadAsync(new Uri("http://www.memphisdev.com/feeds/posts/default"));            
        }
    }

    public class NewsItem
    {
        public string Headline { get; set; }    
        public string Uri { get; set; }    
        // todo: make this a string that is like "3 hours ago;5 days ago" etc.
        public DateTime Date { get; set; }
    }
}
