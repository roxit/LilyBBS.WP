using Microsoft.Phone.Controls;

namespace LilyBBS
{
	public partial class SettingsPage : PhoneApplicationPage
	{
		public SettingsPage()
		{
			InitializeComponent();
		}
		/*
		 * It's useless to check user validity here, since password can be changed before sending posts.
		 */
	}
}