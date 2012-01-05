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

		private void SendPostButton_Click(object sender, RoutedEventArgs e)
		{
			(Application.Current as App).LilyApi.SendPost(
				SendPostCompleted, "test", TitleTextBox.Text, BodyTextBox.Text);
		}
		
		private void SendPostCompleted(object sender, BaseEventArgs e)
		{
			NavigationService.GoBack();
		}
	}
}