using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class ValidateLoginRequest : BaseRequest
	{
		public ValidateLoginRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void ValidateLogin()
		{
			DoAction(ValidateLoginCompleted, "bbsfoot");
		}

		private void ValidateLoginCompleted(object sender, BaseEventArgs e)
		{
			string html = e.Result as string;
			// not using current id
			callback(this, new BaseEventArgs(!html.Contains("bbsqry?userid=guest")));
		}
	}
}
