using System;
using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Coding4Fun.Phone.Controls;

namespace LilyBBS
{
	public partial class SendPostPage : PhoneApplicationPage
	{
		App app;
		Connection conn;
		Settings settings;
		private string board;
		private int? pid;
		private int? num;
		private int? gid;
		private string title;
		public SendPostPage()
		{
			InitializeComponent();
			app = Application.Current as App;
			conn = app.LilyApi;
			settings = app.Resources["Settings"] as Settings;
			SystemTray.SetProgressIndicator(this, app.Indicator);
		}

		private void PhoneApplicationPage_Loaded(object sender, RoutedEventArgs e)
		{
			board = NavigationContext.QueryString["Board"];
			PageTitle.Text = board;
			if (NavigationContext.QueryString.TryGetValue("Title", out title))
			{
				TitleTextBox.Text = title;
				TitleTextBox.IsReadOnly = true;
				pid = int.Parse(NavigationContext.QueryString["Pid"]);
				num = int.Parse(NavigationContext.QueryString["Num"]);
				Utils.ShowIndicator("载入中");
				FetchPostRequest req = new FetchPostRequest(conn, FetchGidCompleted);
				req.FetchPost(board, pid.Value, num.Value);
			}
		}

		private void FetchGidCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				// TODO Check if the post was deleted.
				MessageBox.Show("无法获取数据，请检查网络情况", "出错啦", MessageBoxButton.OK);
				NavigationService.GoBack();
				return;
			}
			Utils.HideIndicator();
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
			Lock();
			if (conn.HasSession)
			{
				Utils.ShowIndicator("更新在线状态");
				conn.ValidateLogin(ValidateLoginCompleted);
			}
			else
				Login();
		}

		private void ValidateLoginCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				ShowError();
				Unlock();
				return;
			}
			bool? successful = e.Result as bool?;
			if (successful.Value)
				SendPost();
			else
				Login();
		}

		private void Login()
		{
			Utils.ShowIndicator("登录中");
			conn.Login(LoginCompleted, settings.Username, settings.Password);
		}

		private void LoginCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				ShowError();
				Unlock();
				return;
			}
			bool? successful = e.Result as bool?;
			if (successful.Value)
				SendPost();
			else
			{
				Utils.HideIndicator();
				MessageBox.Show("登录失败");
				Unlock();
			}
		}

		private void SendPost()
		{
			Utils.ShowIndicator("发送中");
			conn.SendPost(SendPostCompleted,
					board,
					TitleTextBox.Text,
					string.Format("{0}\n\n{1}", BodyTextBox.Text, settings.Signature),
					pid,
					gid);
		}

		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			Utils.HideIndicator();
			if (e.Error != null)
			{
				ShowError();
				Unlock();
				return;
			}
			MessageBox.Show("请手动刷新检查是否发送成功-_-||");
			NavigationService.GoBack();
		}

		#region Misc
		private void ShowError()
		{
			LilyToast toast = new LilyToast();
			toast.ShowNetworkError();
		}

		private void Lock()
		{
			// ToastPrompt does not show when keyboard is shown.
			// So take the textblock's focus to make SIP hide.
			TitleTextBox.IsEnabled = false;
			BodyTextBox.IsEnabled = false;
		}

		private void Unlock()
		{
			TitleTextBox.IsEnabled = true;
			BodyTextBox.IsEnabled = true;
		}
		#endregion

	}
}