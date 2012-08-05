using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using LilyBBS.SDK;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LilyBBS
{
	public partial class BoardPage : PhoneApplicationPage
	{
		App app;
		private string board;
		private int? prevStart;
		private ObservableCollection<Header> itemsSource;

		#region IsLoading
		public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading",
				typeof(bool?),
				typeof(BoardPage),
				new PropertyMetadata(false));

		public bool? IsLoading
		{
			get
			{
				return GetValue(IsLoadingProperty) as bool?;
			}
			set
			{
				bool isLoading = (value as bool?).Value;
				if (isLoading)
					Utils.ShowIndicator("载入中");
				else
					Utils.HideIndicator();
				SetValue(IsLoadingProperty, value);
			}
		}
		#endregion

		public BoardPage()
		{
			InitializeComponent();
			app = (Application.Current as App);
			SystemTray.SetProgressIndicator(this, app.Indicator);
			itemsSource = new ObservableCollection<Header>();
			HeaderList.ItemsSource = itemsSource;
		}

		private void LoadMore(string board, int? start=null)
		{
			IsLoading = true;
			app.LilyApi.FetchPage(FetchPageCompleted, board, start);
		}
		
		private void FetchPageCompleted(object sender, BaseEventArgs e)
		{
			IsLoading = false;
			if (e.Error != null)
			{
				LilyToast toast = new LilyToast();
				toast.ShowNetworkError();
				return;
			}
			LilyBBS.SDK.Page page = e.Result as LilyBBS.SDK.Page;
			prevStart = page.PrevStart;
			foreach (var i in page.HeaderList)
				itemsSource.Add(i);
		}

		private void HeaderList_Loaded(object sender, RoutedEventArgs e)
		{
			// Otherwise when user click this item again, it won't fire SelectionChanged event
			// Navigation happens after this method exists, so build the uri before changing `hdr` to `null`
			HeaderList.SelectedItem = null;
			if (itemsSource.Count != 0) return;
			board = this.NavigationContext.QueryString["board"];
			PageTitle.Text = BoardManager.GetBoardText(board);
			PageSubtitle.Text = board;
			LoadMore(board);
		}

		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			LoadMore(board, prevStart);
		}

		private void HeaderList_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Header hdr = HeaderList.SelectedItem as Header;
			if (hdr == null) return;
			string uri = string.Format("/Views/TopicPage.xaml?board={0}&Pid={1}&Author={2}&Title={3}",
					hdr.Board, hdr.Pid, hdr.Author, hdr.Title);
			NavigationService.Navigate(new Uri(uri, UriKind.Relative));
		}

		private void ComposeButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(
					string.Format("/Views/SendPostPage.xaml?Board={0}", board),
					UriKind.Relative));
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			itemsSource.Clear();
			prevStart = null;
			LoadMore(board);
		}
	}
}