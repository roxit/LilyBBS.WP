using System;

namespace LilyBBS.Misc
{
	public class Constants
	{
		public static Uri MakeTopicViewUri(string board, int pid, string title)
		{
			string uri = string.Format("/Views/TopicPage.xaml?Board={0}&Pid={1}&Title={2}",
					Uri.EscapeUriString(board), pid, Uri.EscapeUriString(title));
			return new Uri(uri, UriKind.Relative);
		}
	}
}
