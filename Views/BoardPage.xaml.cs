using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

using Microsoft.Phone.Shell;
using LilyBBS.API;

namespace LilyBBS
{
	public partial class BoardPage : PhoneApplicationPage
	{
		private string board = "";
		private int prevStart = -1;
		private ObservableCollection<Header> itemsSource;

		public BoardPage()
		{
			InitializeComponent();
			var app = (Application.Current as App);
			SystemTray.SetProgressIndicator(this, app.Indicator);
			itemsSource = new ObservableCollection<Header>();
			HeaderList.ItemsSource = itemsSource;
		}

		private void LoadMore(string board, int start=-1)
		{
			var app = (Application.Current as App);
			app.Indicator.IsVisible = true;
			/* null
			var LoadMoreButton = HeaderList.ListFooter as Button;
			LoadMoreButton.Content = "载入中";
			LoadMoreButton.IsEnabled = false;
			 */
			app.LilyApi.FetchPage(FetchPageCompleted, board, start);
		}
		
		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			LoadMore(board, prevStart);
		}

		private void FetchPageCompleted(object sender, BaseEventArgs e)
		{
			var app = (Application.Current as App);
			app.Indicator.IsVisible = false;
			/*	null
			var LoadMoreButton = HeaderList.ListFooter as Button;
			LoadMoreButton.Content = "更多";
			LoadMoreButton.IsEnabled = true;
			*/
			LilyBBS.API.Page page = e.Result as LilyBBS.API.Page;
			prevStart = page.PrevStart;
			foreach (var i in page.HeaderList)
				itemsSource.Add(i);
		}

		private void HeaderList_Loaded(object sender, RoutedEventArgs e)
		{
			board = this.NavigationContext.QueryString["Board"];
			PageTitle.Text = BoardManager.GetBoardText(board);
			PageSubtitle.Text = board;
			LoadMore(board);
		}


		private void HeaderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = HeaderList.SelectedItem as Header;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/TopicPage.xaml?Board={0}&Pid={1}&Author={2}&Title={3}",
							hdr.Board, hdr.Pid, hdr.Author, hdr.Title),
					UriKind.Relative));
		}

	}
}