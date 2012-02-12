using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class IsLoggedInRequest : BaseRequest
	{
		public IsLoggedInRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void IsLoggedIn()
		{
			DoAction(IsLoggedInCompleted, "bbsfoot");
		}

		private void IsLoggedInCompleted(object sender, BaseEventArgs e)
		{
			string html = e.Result as string;
			// not using current id
			callback(this, new BaseEventArgs(!html.Contains("bbsqry?userid=guest")));
		}
	}
}
