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
using Coding4Fun.Phone.Controls;

namespace LilyBBS
{
	public partial class MainPage : PhoneApplicationPage
	{
		private const string BoardHeader = "讨论区";
		private const string HotHeader = "各区热点";
		private const string TopTenHeader = "全站十大";

		private App app;
		private bool isTopTenListLoaded = false;
		private bool isHotListLoaded = false;

		private ApplicationBarIconButton RefreshButton;
		
		public MainPage()
		{
			InitializeComponent();
			InitializeApplicationBar();
			app = Application.Current as App;
			SystemTray.SetProgressIndicator(this, app.Indicator);
			BoardListSelector.ItemsSource = BoardManager.Instance;
		}
/*
		private void HelloButton_Click(object sender, RoutedEventArgs e)
		{
			
//			(Application.Current as App).LilyApi.Login(LoginCompleted, "obash", "s3creed");
//			NavigationService.Navigate(new Uri("/Views/SendPostPage.xaml", UriKind.Relative));
			(Application.Current as App).LilyApi.FetchPost(HelloButtonCompleted, "Python", 1323076821, 1240);
//			(Application.Current as App).LilyApi.FetchTopic(HelloButtonCompleted, "D_Computer", 1329204278, 60);
//			(Application.Current as App).LilyApi.FetchPage(FetchPageCompleted, "NJUExpress");
//			(Application.Current as App).LilyApi.FetchBoardList(FetchBoardListCompleted);
//			(Application.Current as App).LilyApi.FetchHotList(HelloButtonCompleted);
		}

		private void HelloButtonCompleted(object sender, BaseEventArgs e)
		{
			object ret = e.Result;
			string s = ret.ToString();
		}
*/
		#region Board

		private void BoardListSelector_Loaded(object sender, RoutedEventArgs e)
		{
			BoardListSelector.SelectedItem = null;
		}

		private void BoardListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Board brd = BoardListSelector.SelectedItem as Board;
			if (brd == null) return;		// clicking group header triggers this event too..
			NavigationService.Navigate(new Uri(string.Format("/Views/BoardPage.xaml?board={0}", brd.Name), UriKind.Relative));
		}
		#endregion

		#region Hot

		private void FetchHotList()
		{
			Utils.ShowIndicator("载入中");
			app.LilyApi.FetchHotList(FetchHotListCompleted);
		}

		private void FetchHotListCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				ShowError(HotList, HotErrorTextBox);
				isHotListLoaded = false;
				return;
			}
			HideError(HotList, HotErrorTextBox);
			isHotListLoaded = true;
			List<HeaderGroup> hotList = e.Result as List<HeaderGroup>;
			HotList.ItemsSource = hotList;
		}

		private void HotList_Loaded(object sender, RoutedEventArgs e)
		{
			HotList.SelectedItem = null;
			if (isHotListLoaded) return;
			FetchHotList();
		}

		private void HotList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = (HotList.SelectedItem as Header);
			if (hdr == null) return;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/TopicPage.xaml?board={0}&Pid={1}&Title={2}",
							hdr.Board, hdr.Pid, hdr.Title),
					UriKind.Relative));
		}

		#endregion

		#region TopTen

		private void FetchTopTenList()
		{
			Utils.ShowIndicator("载入中");
			app.LilyApi.FetchTopTenList(FetchTopTenListCompleted);
		}

		private void FetchTopTenListCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				ShowError(TopTenList, TopTenErrorTextBox);
				isTopTenListLoaded = false;
				return;
			}
			HideError(TopTenList, TopTenErrorTextBox);
			isTopTenListLoaded = true;
			List<Header> topTenList = e.Result as List<Header>;
			TopTenList.ItemsSource = topTenList;
		}

		private void TopTenList_Loaded(object sender, RoutedEventArgs e)
		{
			TopTenList.SelectedItem = null;
			if (isTopTenListLoaded) return;
			FetchTopTenList();
		}

		private void TopTenList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = (TopTenList.SelectedItem as Header);
			if (hdr == null) return;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/TopicPage.xaml?board={0}&Pid={1}&Author={2}&Title={3}",
							hdr.Board, hdr.Pid, hdr.Author, hdr.Title),
					UriKind.Relative));
			// TODO
			//TopTenListBox.SelectedIndex = -1;
		}
		#endregion

		#region ApplicationBar

		private void InitializeApplicationBar()
		{
			RefreshButton = new ApplicationBarIconButton(new Uri("/Images/refresh.png", UriKind.Relative));
			RefreshButton.Text = "刷新";
			RefreshButton.Click += RefreshButton_Click;
			ApplicationBar.Buttons.Add(RefreshButton);
		}

		private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			string pi = (e.AddedItems[0] as PivotItem).Header as string;
			switch (pi)
			{
				case HotHeader:
					ApplicationBar.Mode = ApplicationBarMode.Default;
					if (!ApplicationBar.Buttons.Contains(RefreshButton))
						ApplicationBar.Buttons.Add(RefreshButton);
					break;
				case TopTenHeader:
					ApplicationBar.Mode = ApplicationBarMode.Default;
					if (!ApplicationBar.Buttons.Contains(RefreshButton))
						ApplicationBar.Buttons.Add(RefreshButton);
					break;
				default:
					ApplicationBar.Mode = ApplicationBarMode.Minimized;
					if (ApplicationBar.Buttons.Contains(RefreshButton))
					{
						ApplicationBar.Buttons.Remove(RefreshButton);
					}
					break;
			}
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			string pi = (this.Pivot.SelectedItem as PivotItem).Header as string;
			switch (pi)
			{
				case HotHeader:
					FetchHotList();
					break;
				case TopTenHeader:
					FetchTopTenList();
					break;
				default:
					break;
			}
		}
		#endregion

		#region Settings
		public void SettingsButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
		}
		#endregion

		#region Misc

		private void ShowError(LongListSelector content, TextBlock error)
		{
			content.Visibility = Visibility.Collapsed;
			error.Visibility = Visibility.Visible;
			LilyToast toast = new LilyToast();
			toast.Message = (app.Resources["NetworkErrorMessage"] as NetworkErrorMessage).Message;
			toast.Show();
		}

		private void HideError(LongListSelector content, TextBlock error)
		{
			content.Visibility = Visibility.Visible;
			error.Visibility = Visibility.Collapsed;			
		}

		#endregion
	}
}