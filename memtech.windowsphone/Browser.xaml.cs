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
using System.Windows.Navigation;

namespace memtech.windowsphone
{
    public partial class Browser : PhoneApplicationPage
    {
        private Uri _currentUri;
        public Browser()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(Browser_Loaded);
            webBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(webBrowser_LoadCompleted);
        }

        void Browser_Loaded(object sender, RoutedEventArgs e)
        {
            webBrowser.Navigate(new Uri(NavigationContext.QueryString["uri"], UriKind.Absolute));
        }

        private void webBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _currentUri = e.Uri;
        }

        private void CopyLink_Click(object sender, EventArgs e)
        {
            // todo: should we copy the link that sent them here or the link the browser ended up as?
            var linkToCopy = _currentUri != null ? _currentUri.AbsolutePath : NavigationContext.QueryString["uri"];
            Clipboard.SetText(linkToCopy);
        }
    }
}