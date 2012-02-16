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
using Microsoft.Phone.Shell;

namespace LilyBBS
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			InitializeComponent();
			var app = Application.Current as App;
			SystemTray.SetProgressIndicator(this, app.Indicator);
			BoardListSelector.ItemsSource = BoardManager.Instance;
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
//			(Application.Current as App).LilyApi.FetchTopic(HelloButtonCompleted, "D_Computer", 1329204278, 60);
//			(Application.Current as App).LilyApi.FetchPage(FetchPageCompleted, "NJUExpress");
//			(Application.Current as App).LilyApi.FetchBoardList(FetchBoardListCompleted);
			(Application.Current as App).LilyApi.FetchHotList(HelloButtonCompleted);
		}

		private void HelloButtonCompleted(object sender, BaseEventArgs e)
		{
			var t = e.Result as List<List<Header>>;
		}

		#region Board
		private void BoardListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (BoardListSelector.SelectedItem == null) return;		// clicking group header triggers this event too..
			Board brd = BoardListSelector.SelectedItem as Board;
			NavigationService.Navigate(new Uri(string.Format("/Views/BoardPage.xaml?Board={0}", brd.Name), UriKind.Relative));
		}
		#endregion

		#region Hot

		private void HotList_Loaded(object sender, RoutedEventArgs e)
		{
			var app = Application.Current as App;
			app.Indicator.IsVisible = true;
			app.LilyApi.FetchHotList(FetchHotListCompleted);
		}

		private void FetchHotListCompleted(object sender, BaseEventArgs e)
		{
			var app = Application.Current as App;
			app.Indicator.IsVisible = false;
			List<HeaderGroup> hotList = e.Result as List<HeaderGroup>;
			HotList.ItemsSource = hotList;
		}

		private void HotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = (HotList.SelectedItem as Header);
			if (hdr == null) return;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/TopicPage.xaml?Board={0}&Pid={1}&Title={2}",
							hdr.Board, hdr.Pid, hdr.Title),
					UriKind.Relative));
		}

		#endregion

		#region TopTen
		private void TopTenList_Loaded(object sender, RoutedEventArgs e)
		{
			var app = Application.Current as App;
			app.Indicator.IsVisible = true;
			app.LilyApi.FetchTopTenList(FetchTopTenListCompleted);
		}

		private void FetchTopTenListCompleted(object sender, BaseEventArgs e)
		{
			var app = Application.Current as App;
			app.Indicator.IsVisible = false;
			List<Header> topTenList = e.Result as List<Header>;
			TopTenList.ItemsSource = topTenList;
		}

		private void TopTenList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = (TopTenList.SelectedItem as Header);
			if (hdr == null) return;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/TopicPage.xaml?Board={0}&Pid={1}&Author={2}&Title={3}",
							hdr.Board, hdr.Pid, hdr.Author, hdr.Title),
					UriKind.Relative));
			// TODO
			//TopTenListBox.SelectedIndex = -1;
		}
		#endregion

		private void Pivot_LoadingPivotItem(object sender, PivotItemEventArgs e)
		{
			return;
			int i;
			if (e.Item.Header == "全站十大")
			{
				ApplicationBar.IsVisible = true;
			}
			else if (e.Item.Header == "讨论区")
			{
				ApplicationBar.IsVisible = false;
			}
			else
			{
				ApplicationBar.IsVisible = false;
			}
		}
	}
}