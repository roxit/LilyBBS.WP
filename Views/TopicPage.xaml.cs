using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using LilyBBS.DAL;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using LilyBBS.ViewModels;

namespace LilyBBS.Views
{
	public partial class TopicPage : PhoneApplicationPage
	{
		App app;

		#region IsLoading
		public static readonly DependencyProperty IsLoadingProperty = DependencyProperty.Register("IsLoading",
				typeof(bool?),
				typeof(TopicPage),
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

		#region NextStart
		public static readonly DependencyProperty NextStartProperty = DependencyProperty.Register("NextStart",
				typeof(int?),
				typeof(TopicPage),
				new PropertyMetadata(null));

		public int? NextStart
		{
			get
			{
				return GetValue(NextStartProperty) as int?;
			}
			set { SetValue(NextStartProperty, value); }
		}
		#endregion

		public TopicPage()
		{
			InitializeComponent();
			app = Application.Current as App;
			SystemTray.SetProgressIndicator(this, app.Indicator);
			DataContext = App.TopicViewModel;
		}

		private void PostList_Loaded(object sender, RoutedEventArgs e)
		{
			string board = NavigationContext.QueryString["Board"];
			int pid = int.Parse(NavigationContext.QueryString["Pid"]);
			string title = Uri.UnescapeDataString(NavigationContext.QueryString["Title"]);

			var vm = DataContext as TopicViewModel;
			vm.LoadData(board, pid);

			BoardTextBlock.Text = board;
			TitleTextBlock.Text = title;
		}

		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			var vm = DataContext as TopicViewModel;
			vm.LoadMore();
		}

		private void ReplyButton_Click(object sender, EventArgs e)
		{
/*			NavigationService.Navigate(new Uri(
					string.Format("/Views/SendPostPage.xaml?Board={0}&Title={1}&Pid={2}&Num={3}",
							board, "Re: "+items[0].Title, pid, items[0].Num),
					UriKind.Relative));
*/		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			var vm = DataContext as TopicViewModel;
			vm.Refresh();
		}
	}

	public class HasMoreConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return value != null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

}
