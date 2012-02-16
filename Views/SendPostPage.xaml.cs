using System;
using System.Windows;
using LilyBBS.API;
using Microsoft.Phone.Controls;

namespace LilyBBS
{
	public partial class SendPostPage : PhoneApplicationPage
	{
		public SendPostPage()
		{
			InitializeComponent();
		}
		
		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			NavigationService.GoBack();
		}

		private void SendButton_Click(object sender, EventArgs e)
		{
			var app = Application.Current as App;

			app.LilyApi.SendPost(
					SendPostCompleted, "test", TitleTextBox.Text, BodyTextBox.Text);
		}

		private void SettingsButton_Click(object sender, EventArgs e)
		{
			NavigationService.Navigate(new Uri("/Views/SettingsPage.xaml", UriKind.Relative));
		}
	}
}