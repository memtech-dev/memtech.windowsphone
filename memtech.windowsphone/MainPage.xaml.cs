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
using memtech.windowsphone.ViewModels;

namespace memtech.windowsphone
{
    public partial class MainPage : PhoneApplicationPage
    {        
        public MainPage()
        {
            InitializeComponent();

            // setup event handlers
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);

            // init the view model for this view
            // this viewmodel is static so that we can fetch items from it and not incur network cost again
            // this is mainly for the event view
            App.RootViewModel = new MainPageViewModel();

            // Set the data context of the listbox control to the sample data
            DataContext = App.RootViewModel;
        }

        // Load data for the ViewModel Items
        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            App.RootViewModel.LoadNewsItems();
            App.RootViewModel.LoadUpcomingEvents();
        }

        private void NewsItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = sender as ListBox;
            // If selected index is -1 (no selection) do nothing
            if (listBox.SelectedIndex == -1)
                return;

            var selectedNewsItem = listBox.SelectedItem as NewsItem;            

            // navigate to the internal web viewer
            NavigationService.Navigate(new Uri("/Browser.xaml?uri=" + selectedNewsItem.Uri, UriKind.Relative));

            // remove selection
            listBox.SelectedIndex = -1;            
        }

        private void MenuItem_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listBox = (sender as ListBox);
            // If selected index is -1 (no selection) do nothing
            if (listBox.SelectedIndex == -1)
                return;

            var selectedItem = listBox.SelectedItem as MenuItem;

            NavigationService.Navigate(new Uri(selectedItem.Uri, UriKind.Relative));

            listBox.SelectedIndex = -1;
        }
    }
}