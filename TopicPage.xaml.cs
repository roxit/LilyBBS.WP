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

		private void FetchTopicCompleted(LilyArgs e)
		{
			Topic t = e.Result as Topic;
			TopicListBox.ItemsSource = t.PostList;
			progressIndicator.IsVisible = false;
		}
	}
}