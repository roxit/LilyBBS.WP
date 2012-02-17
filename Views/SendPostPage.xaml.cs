using System;
using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace LilyBBS
{
	public partial class SendPostPage : PhoneApplicationPage
	{
		Connection conn;
		Settings settings;
		ProgressIndicator indicator;
		private string board;
		private int? pid;
		private int? num;
		private int? gid;
		private string title;

		public SendPostPage()
		{
			InitializeComponent();
			var app = Application.Current as App;
			conn = app.LilyApi;
			settings = app.Resources["AppSettings"] as Settings;
			indicator = app.Indicator;
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			board = NavigationContext.QueryString["Board"];
			if (NavigationContext.QueryString.TryGetValue("Title", out title))
			{
				TitleTextBox.Text = title;
				TitleTextBox.IsReadOnly = true;
				pid = int.Parse(NavigationContext.QueryString["Pid"]);
				num = int.Parse(NavigationContext.QueryString["Num"]);
				indicator.IsVisible = true;
				FetchPostRequest req = new FetchPostRequest(conn, FetchGidCompleted);
				req.FetchPost(board, pid.Value, num.Value);
			}
		}

		private void FetchGidCompleted(object sender, BaseEventArgs e)
		{
			indicator.IsVisible = false;
			Post p = e.Result as Post;
			gid = p.Gid;
		}

		private void SettingsButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			TitleTextBox.Text = TitleTextBox.Text.Trim();
			if (TitleTextBox.Text == "")
			{
				MessageBox.Show("请填写标题");
				return;
			}
			if (settings.Username == "" || settings.Password == "")
			{
				MessageBox.Show("请先设置登录帐号");
				return;
			}

			indicator.IsVisible = true;
			if (conn.HasSession)
			{
				indicator.Text = "更新在线状态";
				conn.ValidateLogin(ValidateLoginCompleted);
			}
			else
				Login();
		}

		private void ValidateLoginCompleted(object sender, BaseEventArgs e)
		{
			bool? successful = e.Result as bool?;
			if (successful.Value)
				SendPost();
			else
				Login();
		}

		private void Login()
		{
			indicator.Text = "登录中";
			conn.Login(LoginCompleted, settings.Username, settings.Password);
		}

		private void LoginCompleted(object sender, BaseEventArgs e)
		{
			bool? successful = e.Result as bool?;
			if (successful.Value)
				SendPost();
			else
			{
				MessageBox.Show("登录失败");
				indicator.Text = "";
				indicator.IsVisible = false;
			}
		}

		private void SendPost()
		{
			indicator.Text = "发送中";
			conn.SendPost(SendPostCompleted,
					board,
					TitleTextBox.Text,
					string.Format("{0}\n\n{1}", BodyTextBox.Text, settings.Signature),
					pid,
					gid);
		}

		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			indicator.IsVisible = false;
			indicator.Text = "";
			MessageBox.Show("请手动刷新检查是否发送成功-_-||");
			NavigationService.GoBack();
		}
	}
}