using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using System.Collections.ObjectModel;
using memtech.windowsphone.ViewModels;

namespace memtech.windowsphone
{
    public partial class TwitterFeed : PhoneApplicationPage
    {
        bool _hasLoaded = false;
        TwitterFeedViewModel _viewModel;
        public TwitterFeed()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(TwitterFeed_Loaded);

            _viewModel = new TwitterFeedViewModel();
            this.DataContext = _viewModel;
        }

        void TwitterFeed_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_hasLoaded)
            {
                _viewModel.LoadCurrentTweets();
                _hasLoaded = true;
            }
        }

        private void TwitterUser_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            var currentTweet = tb.DataContext as Tweet;
            var userProfileUri = string.Format("http://twitter.com/{0}", currentTweet.FromUser);
            NavigationService.Navigate(new Uri("/Browser.xaml?uri=" + userProfileUri, UriKind.Relative));
        }

        private void TwitterTweet_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var tb = sender as TextBlock;
            var currentTweet = tb.DataContext as Tweet;
            var userTweetUri = string.Format("http://twitter.com/{0}/status/{1}", currentTweet.FromUser, currentTweet.IdStr);
            NavigationService.Navigate(new Uri("/Browser.xaml?uri=" + userTweetUri, UriKind.Relative));
        }        
    }


}