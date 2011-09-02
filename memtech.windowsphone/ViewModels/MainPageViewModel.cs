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
using System.Xml.Linq;
using System.Collections.Specialized;
using System.ComponentModel;

namespace memtech.windowsphone.ViewModels
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<NewsItem> NewsItems { get; set; }
        public ObservableCollection<UpcomingEvent> UpcomingEvents { get; set; }
        public ObservableCollection<MenuItem> MenuItems { get; set; }

        private Visibility _showNoNewsMessage = Visibility.Visible;
        public Visibility ShowNoNewsMessage
        {
            get
            {
                return _showNoNewsMessage;
            }
            private set
            {
                _showNoNewsMessage = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ShowNoNewsMessage"));
            }
        }

        private Visibility _showNoEventsMessage = Visibility.Visible;
        public Visibility ShowNoEventsMessage
        {
            get
            {
                return _showNoEventsMessage;
            }
            private set
            {
                _showNoEventsMessage = value;
                this.OnPropertyChanged(new PropertyChangedEventArgs("ShowNoEventsMessage"));
            }
        }

        public MainPageViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>();            
            MenuItems.Add(new MenuItem { DisplayName = "#memTech", Uri = "/TwitterFeed.xaml" });

            NewsItems = new ObservableCollection<NewsItem>();
            UpcomingEvents = new ObservableCollection<UpcomingEvent>();

            NewsItems.CollectionChanged += new NotifyCollectionChangedEventHandler(NewsItems_CollectionChanged);
            UpcomingEvents.CollectionChanged += new NotifyCollectionChangedEventHandler(UpcomingEvents_CollectionChanged);
        }

        void UpcomingEvents_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShowNoEventsMessage = UpcomingEvents.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        void NewsItems_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ShowNoNewsMessage = NewsItems.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        internal void LoadUpcomingEvents()
        {
            var webClient = new WebClient();

            webClient.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs e)
            {
                if (e.Error != null)
                {
                    MessageBox.Show("An error occurred downloading the news :(");
                    return;
                }

                using (var stream = e.Result)
                {
                    var doc = XDocument.Load(stream);
                    var futureEvents = from entry in doc.Root.Elements(XName.Get("entry", "http://www.w3.org/2005/Atom"))
                                       let startDate = DateTime.Parse(entry.Element(XName.Get("when", "http://schemas.google.com/g/2005")).Attribute("startTime").Value)
                                       where startDate > DateTime.Now && (startDate < DateTime.Today.AddDays(14))
                                       select new UpcomingEvent
                                       {
                                           Name = entry.Element(XName.Get("title", "http://www.w3.org/2005/Atom")).Value,
                                           Description = entry.Element(XName.Get("content", "http://www.w3.org/2005/Atom")).Value,
                                           Location = entry.Element(XName.Get("where", "http://schemas.google.com/g/2005")).HasAttributes ? "Somewhere" : "",
                                           Date = startDate
                                       };

                    foreach (var futureEvent in futureEvents.OrderBy(x => x.Date))
                        UpcomingEvents.Add(futureEvent);
                }
            };

            webClient.OpenReadAsync(new Uri("https://www.google.com/calendar/feeds/en.usa%23holiday%40group.v.calendar.google.com/public/full"));
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

                using (var stream = e.Result)
                {
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
                }
            };

            // todo: hopefully get rid of this RSS crap and make the memtech-service the backend instead...
            // it should allow an admin of some sort to add RSS links to aggregate news
            // it should also allow us to push our own custom news down (maybe with push notificatons?)
            webClient.OpenReadAsync(new Uri("http://www.memphisdev.com/feeds/posts/default"));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, e);
        }
    }

    public class MenuItem
    {
        public string DisplayName { get; set; }
        public string Uri { get; set; }
    }

    public class NewsItem
    {
        public string Headline { get; set; }
        public string Uri { get; set; }
        // todo: make this a string that is like "3 hours ago;5 days ago" etc.
        public DateTime Date { get; set; }
    }

    public class UpcomingEvent
    {
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
    }
}
