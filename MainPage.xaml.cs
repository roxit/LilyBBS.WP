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
using LilyBBS.API;
using Microsoft.Phone.Info;

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
			Connection api = (Application.Current as App).LilyApi;
/*			if (!api.IsLoggedIn)
			{
				client.Login(LoginCompleted, "obash", "changeme");
			}
*/		}

		private void HelloButton_Click(object sender, RoutedEventArgs e)
		{
			
//			(Application.Current as App).LilyApi.Login(LoginCompleted, "obash", "s3creed");
//			NavigationService.Navigate(new Uri("/Views/SendPostPage.xaml", UriKind.Relative));
//			(Application.Current as App).LilyApi.FetchPost(FetchPostCompleted, 1323076821, "Python", 1240);
//			(Application.Current as App).LilyApi.FetchTopic(FetchTopicCompleted, 1324280901, "D_Computer");
//			(Application.Current as App).LilyApi.FetchTopTenList(FetchTopTenListCompleted);
//			(Application.Current as App).LilyApi.FetchPage(FetchPageCompleted, "NJUExpress");
			(Application.Current as App).LilyApi.FetchBoardList(FetchBoardListCompleted);
		}

		private void LoginCompleted(object sender, BaseEventArgs e)
		{
			string cookie = e.Result as string;
		}

		private void FetchPostCompleted(object sender, BaseEventArgs e)
		{
			Post p = e.Result as Post;

		}

		private void FetchPageCompleted(object sender, BaseEventArgs e)
		{
			LilyBBS.API.Page page = e.Result as LilyBBS.API.Page;
		}

		private void FetchTopTenListCompleted(object sender, BaseEventArgs e)
		{
			List<Header> topTenList = e.Result as List<Header>;
			TopTenListBox.ItemsSource = topTenList;
		}

		private void PivotItem_Loaded(object sender, RoutedEventArgs e)
		{
			string dn = DeviceStatus.DeviceName;
			string dm = DeviceStatus.DeviceManufacturer;

			(Application.Current as App).LilyApi.FetchTopTenList(FetchTopTenListCompleted);
		}

		private void TopTenListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (TopTenListBox.SelectedIndex == -1) return;
			Header header = (TopTenListBox.SelectedItem as Header);
			NavigationService.Navigate(new Uri(
				string.Format("/Views/TopicPage.xaml?Board={0}&Pid={1}&Author={2}&Title={3}", 
					header.Board, header.Pid, header.Author, header.Title), 
				UriKind.Relative));
			TopTenListBox.SelectedIndex = -1;
		}

		private void FetchBoardListCompleted(object sender, BaseEventArgs e)
		{
			//Dictionary<int, Section> BoardList = e.Result as Dictionary<int, Section>;
		}

	}
}