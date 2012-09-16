using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using LilyBBS.Models;
using LilyBBS.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows.Navigation;
using LilyBBS.Misc;

namespace LilyBBS.Views
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
			AllBoardListSelector.ItemsSource = BoardManager.Instance;
			LoadFavoriteBoardList();

			TopHeaderList.DataContext = App.TopViewModel;
			HotHeaderList.DataContext = App.HotViewModel;
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

		}

		#region Board

		private void LoadFavoriteBoardList()
		{
			Settings settings = app.Resources["Settings"] as Settings;
			List<Board> favBrd = new List<Board>();
			if (settings.FavoriteBoardList.Count > 0)
			{
				FavoriteBoardListSelector.Visibility = Visibility.Visible;
				NoFavoriteBoardTextBox.Visibility = Visibility.Collapsed;
				foreach (var i in settings.FavoriteBoardList)
					favBrd.Add(new Board(i, BoardManager.GetBoardText(i)));
			}
			else
			{
				NoFavoriteBoardTextBox.Visibility = Visibility.Visible;
				FavoriteBoardListSelector.Visibility = Visibility.Collapsed;
			}
			FavoriteBoardListSelector.ItemsSource = favBrd;
		}


		private void AddFavoriteBoard_Click(object sender, RoutedEventArgs e)
		{
			Settings settings = app.Resources["Settings"] as Settings;
			Board brd = (sender as MenuItem).DataContext as Board;
			settings.AddFavoriteBoard(brd.Name);
			LoadFavoriteBoardList();
		}

		private void RemoveFavoriteBoard_Click(object sender, RoutedEventArgs e)
		{
			Settings settings = app.Resources["Settings"] as Settings;
			Board brd = (sender as MenuItem).DataContext as Board;
			settings.RemoveFavoriteBoard(brd.Name);
			LoadFavoriteBoardList();
		}

		private void BoardListSelector_Loaded(object sender, RoutedEventArgs e)
		{
			(sender as LongListSelector).SelectedItem = null;
		}

		private void BoardListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Board brd = (sender as LongListSelector).SelectedItem as Board;
			if (brd == null) return;		// clicking group header triggers this event too..
			gotoBoard(brd.Name);
		}

		private void FavoriteBoardButton_Click(object sender, RoutedEventArgs e)
		{
			if (FavoriteBoardGrid.Visibility != Visibility.Visible)
			{
				FavoriteBoardGrid.Visibility = Visibility.Visible;
				LoadFavoriteBoardList();
				AllBoardListSelector.Visibility = Visibility.Collapsed;
			}
		}

		private void AllBoardButton_Click(object sender, RoutedEventArgs e)
		{
			if (AllBoardListSelector.Visibility != Visibility.Visible)
			{
				AllBoardListSelector.Visibility = Visibility.Visible;
				FavoriteBoardGrid.Visibility = Visibility.Collapsed;
			}
		}
		#endregion

		#region ApplicationBar

		private void Pivot_Loaded(object sender, RoutedEventArgs e)
		{
			// TODO: ApplicationBar and TopTen/HotList overlay
			//ApplicationBar.IsVisible = true;
		}

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
					//HotList.UpdateLayout();
					break;
				case TopTenHeader:
					ApplicationBar.Mode = ApplicationBarMode.Default;
					if (!ApplicationBar.Buttons.Contains(RefreshButton))
						ApplicationBar.Buttons.Add(RefreshButton);
					//TopTenList.UpdateLayout();
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
					//FetchHotList();
					break;
				case TopTenHeader:
					//FetchTopTenList();
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
		
		private void gotoBoard(string board)
		{
			NavigationService.Navigate(Constants.MakeBoardViewUri(board));
		}

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

		#region ContextMenu

		private void GotoBoard_Click(object sender, RoutedEventArgs e)
		{
			Header h = (sender as MenuItem).DataContext as Header;
			gotoBoard(h.Board);
		}
		#endregion

		#region Hot&Top
		/*
		private void FetchHotList()
		{
			Utils.ShowIndicator("载入中");
			app.LilyApi.FetchHotList(FetchHotListCompleted);
		}

		private void FetchHotListCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
		}

		*/

		private void TopHeaderList_Loaded(object sender, RoutedEventArgs e)
		{
			var vm = (sender as LongListSelector).DataContext as TopViewModel;
			vm.LoadData();
		}

		private void HotHeaderList_Loaded(object sender, RoutedEventArgs e)
		{
			var vm = (sender as LongListSelector).DataContext as HotViewModel;
			vm.LoadData();
		}

		private void HeaderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (e.AddedItems.Count == 1)
			{
				var obj = sender as LongListSelector;
				Header hdr = obj.SelectedItem as Header;
				NavigationService.Navigate(Constants.MakeTopicViewUri(hdr.Board, hdr.Pid, hdr.Title));
				obj.SelectedItem = null;
			}
		}
		#endregion Hot&Top
	}
}