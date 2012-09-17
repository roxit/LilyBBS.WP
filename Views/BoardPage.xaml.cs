using System;
using System.Windows;
using System.Windows.Controls;
using LilyBBS.Misc;
using LilyBBS.Models;
using LilyBBS.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LilyBBS.Views
{
	public partial class BoardPage : PhoneApplicationPage
	{
		App app;

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
			DataContext = App.BoardViewModel;
		}

		private void HeaderList_Loaded(object sender, RoutedEventArgs e)
		{
			string board = NavigationContext.QueryString["Board"];
			var vm = DataContext as BoardViewModel;
			vm.LoadData(board);
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

		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as BoardViewModel;
			vm.LoadMore();
		}

		private void ComposeButton_Click(object sender, EventArgs e)
		{
			var vm = DataContext as BoardViewModel;
			NavigationService.Navigate(new Uri(
					string.Format("/Views/SendPostPage.xaml?Board={0}", vm.Board),
					UriKind.Relative));
		}


		private void RefreshButton_Click(object sender, EventArgs e)
		{
			var vm = DataContext as BoardViewModel;
			vm.Refresh();
		}
	}
}