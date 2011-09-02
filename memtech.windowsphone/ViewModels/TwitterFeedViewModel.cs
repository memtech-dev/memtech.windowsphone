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
using System.Xml.Linq;
using RestSharp;
using System.Collections.Generic;

namespace memtech.windowsphone.ViewModels
{
    public class TwitterFeedViewModel
    {
        public ObservableCollection<Tweet> Tweets { get; set; }

        public TwitterFeedViewModel()
        {
            Tweets = new ObservableCollection<Tweet>();
        }

        internal void LoadCurrentTweets()
        {
            var restClient = new RestClient("http://search.twitter.com");
            
            var restRequest = new RestRequest("search.json");

            restRequest.AddParameter("q", "#memTech");
            restRequest.AddParameter("result_type", "recent");
            restRequest.AddParameter("count", "10");

            restClient.ExecuteAsync<TwitterResponse>(restRequest, (resp) => {
                var twitterData = resp.Data;
                foreach (var tweet in twitterData.Results)
                {
                    // http://twitter.com/#!/JohnFleenor/status/106759409438429185
                    Tweets.Add(tweet);
                }            
            });            
        }
    }

    public class TwitterResponse
    {
        public List<Tweet> Results { get; set; }
    }

    public class Tweet
    {
        public string Text { get; set; }
        public string FromUser { get; set; }
        public string IdStr { get; set; }
    }
}
