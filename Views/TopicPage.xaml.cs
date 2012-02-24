using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;
using System;
using Coding4Fun.Phone.Controls;

namespace LilyBBS
{
	/*
	 * TODO
	 * http://bbs.nju.edu.cn/bbstcon?board=D_Computer&file=M.1329204278.A
	 * http://bbs.nju.edu.cn/bbstcon?board=WorldFootball&file=M.1329342210.A
	 * 1st post partly shown, api data ok
	 */
	public partial class TopicPage : PhoneApplicationPage
	{
		App app;
		private string board;
		private int pid;
		private string title;
		private string author;
		private int? nextStart;
		private ObservableCollection<Post> itemsSource;

		public TopicPage()
		{
			InitializeComponent();
			app = Application.Current as App;
			SystemTray.SetProgressIndicator(this, app.Indicator);

			itemsSource = new ObservableCollection<Post>();
			PostList.ItemsSource = itemsSource;
		}

		private void LoadMore(string board, int pid, int? start=null)
		{
			Utils.ShowIndicator("载入中");			
			app.LilyApi.FetchTopic(FetchTopicCompleted, board, pid, start);
		}

		private void FetchTopicCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				LilyToast toast = new LilyToast();
				toast.ShowNetworkError();
				return;
			}
			Topic t = e.Result as Topic;
			nextStart = t.nextStart;
			foreach (var i in t.PostList)
				itemsSource.Add(i);
		}

		private void PostList_Loaded(object sender, RoutedEventArgs e)
		{
			PostList.SelectedItem = null;
			if (itemsSource.Count != 0) return;
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
			if (nextStart == null)
			{
				LilyToast toast = new LilyToast("再也没有了");
				toast.Show();
				return;
			}
			LoadMore(board, pid, nextStart);
		}

		private void ReplyButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri(
					string.Format("/Views/SendPostPage.xaml?Board={0}&Title={1}&Pid={2}&Num={3}",
							board, "Re: "+itemsSource[0].Title, pid, itemsSource[0].Num),
					UriKind.Relative));
		}

		private void RefreshButton_Click(object sender, EventArgs e)
		{
			itemsSource.Clear();
			nextStart = null;
			LoadMore(board, pid, nextStart);
		}

	}
}