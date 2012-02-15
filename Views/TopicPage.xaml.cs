using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Collections.ObjectModel;

namespace LilyBBS
{
	public partial class TopicPage : PhoneApplicationPage
	{
		private string board;
		private int pid;
		private string title;
		private string author;
		private int? nextStart;
		private ObservableCollection<Post> itemsSource;

		public TopicPage()
		{
			InitializeComponent();
			var app = Application.Current as App;
			SystemTray.SetProgressIndicator(this, app.Indicator);

			itemsSource = new ObservableCollection<Post>();
			PostList.ItemsSource = itemsSource;
		}

		private void LoadMore(string board, int pid, int? start=null)
		{
			var app = (Application.Current as App);
			app.Indicator.IsVisible = true;
			app.LilyApi.FetchTopic(FetchTopicCompleted, board, pid, start);
		}

		private void LoadMoreButton_Click(object sender, RoutedEventArgs e)
		{
			if (nextStart == null) return;
			LoadMore(board, pid, nextStart);
		}

		private void FetchTopicCompleted(object sender, BaseEventArgs e)
		{
			var app = Application.Current as App;
			app.Indicator.IsVisible = false;

			Topic t = e.Result as Topic;
			nextStart = t.nextStart;
			foreach (var i in t.PostList)
				itemsSource.Add(i);
		}

		private void PostList_Loaded(object sender, RoutedEventArgs e)
		{
			board = NavigationContext.QueryString["Board"];
			pid = int.Parse(NavigationContext.QueryString["Pid"]);
			title = NavigationContext.QueryString["Title"];
			author = NavigationContext.QueryString["Author"];
			BoardTextBlock.Text = board;
			TitleTextBlock.Text = title;

			LoadMore(board, pid);
		}

		private void PostList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{

		}

	}
}