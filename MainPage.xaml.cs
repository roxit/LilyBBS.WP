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

namespace LilyBBS
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			LilyConnection client = (Application.Current as App).LilyApi;
			if (!client.IsLoggedIn)
			{
//				client.Login(LoginCompleted, "obash", "changeme");
			}
		}

		private void LoginCompleted(LilyArgs e)
		{
		}

		private void HelloButton_Click(object sender, RoutedEventArgs e)
		{
//			NavigationService.Navigate(new Uri("/SendPostPage.xaml", UriKind.Relative));
//			(Application.Current as App).LilyClient.FetchPost(FetchPostCompleted, 1323076821, "Python", 1240);
//			(Application.Current as App).LilyClient.FetchTopic(FetchTopicCompleted, 1324280901, "D_Computer");
//			(Application.Current as App).LilyClient.FetchTopTenList(FetchTopTenListCompleted);
			(Application.Current as App).LilyApi.FetchPage(FetchPageCompleted, "NJUExpress");
		}
		private void FetchPostCompleted(LilyArgs e)
		{
			Post p = e.Result as Post;

		}

		private void FetchPageCompleted(LilyArgs e)
		{
			Page page = e.Result as Page;
		}

		private void FetchTopTenListCompleted(LilyArgs e)
		{
			List<Header> topTenList = e.Result as List<Header>;
			TopTenListBox.ItemsSource = topTenList;
		}

		private void PivotItem_Loaded(object sender, RoutedEventArgs e)
		{
			(Application.Current as App).LilyApi.FetchTopTenList(FetchTopTenListCompleted);
		}

		private void TopTenListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (TopTenListBox.SelectedIndex == -1) return;
			Header header = (TopTenListBox.SelectedItem as Header);
			NavigationService.Navigate(new Uri(string.Format("/TopicPage.xaml?Board={0}&Pid={1}&Author={2}&Title={3}", header.Board, header.Pid, header.Author, header.Title), UriKind.Relative));
			TopTenListBox.SelectedIndex = -1;
		}
	}
}