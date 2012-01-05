using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LilyBBS
{
	public partial class TopicPage : PhoneApplicationPage
	{
		ProgressIndicator progressIndicator = new ProgressIndicator()
		{
			IsVisible = true,
			IsIndeterminate = true,
			Text = "Downloading"
		};

		public TopicPage()
		{
			InitializeComponent();
		}
		
		private void TopicListBox_Loaded(object sender, RoutedEventArgs e)
		{
			Board.Text = NavigationContext.QueryString["Board"];
			TopicAuthor.Text = NavigationContext.QueryString["Author"];
			PageTitle.Text = NavigationContext.QueryString["Title"];
			SystemTray.SetProgressIndicator(this, progressIndicator);
			(Application.Current as App).LilyApi.FetchTopic(FetchTopicCompleted,
				int.Parse(NavigationContext.QueryString["Pid"]),
				Board.Text);
		}

		private void FetchTopicCompleted(object sender, BaseEventArgs e)
		{
			Topic t = e.Result as Topic;
			TopicListBox.ItemsSource = t.PostList;
			progressIndicator.IsVisible = false;
		}
	}
}