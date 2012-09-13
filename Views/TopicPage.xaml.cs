using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using LilyBBS.DAL;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LilyBBS
{
	public partial class TopicPage : PhoneApplicationPage
	{
		App app;
		private string board;
		private int pid;
		private string title;
		private string author;
		private ObservableCollection<Post> items;

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
			items = new ObservableCollection<Post>();
			PostList.ItemsSource = items;
			PostList.DataContext = this;
		}

		private void LoadMore(string board, int pid, int? start=null)
		{
			IsLoading = true;
			app.LilyApi.FetchTopic(FetchTopicCompleted, board, pid, start);
		}

		private void FetchTopicCompleted(object sender, BaseEventArgs e)
		{
			IsLoading = false;
			if (e.Error != null)
			{
				LilyToast toast = new LilyToast();
				toast.ShowNetworkError();
				return;
			}
			/*
			 * HTMLAgilityPack fails often!
			 */
			try
			{
				Topic t = e.Result as Topic;
				NextStart = t.nextStart;
				foreach (var i in t.PostList)
					items.Add(i);
			}
			catch (Exception exc)
			{
				
			}
		}

		private void PostList_Loaded(object sender, RoutedEventArgs e)
		{
			PostList.SelectedItem = null;
			if (items.Count != 0) return;
			board = NavigationContext.QueryString["board"];
			pid = int.Parse(NavigationContext.QueryString["Pid"]);
			title = NavigationContext.QueryString["Title"];
			NavigationContext.QueryString.TryGetValue("Author", out author);
			BoardTextBlock.Text = board;
			TitleTextBlock.Text = title;
			LoadMore(board, pid);
		}

		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			if (NextStart == null)
			{
				LilyToast toast = new LilyToast("再也没有了");
				toast.Show();
				return;
			}
			LoadMore(board, pid, NextStart);
		}

		private void ReplyButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(
					string.Format("/Views/SendPostPage.xaml?Board={0}&Title={1}&Pid={2}&Num={3}",
							board, "Re: "+items[0].Title, pid, items[0].Num),
					UriKind.Relative));
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			items.Clear();
			NextStart = null;
			LoadMore(board, pid, NextStart);
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
