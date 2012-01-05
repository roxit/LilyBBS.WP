using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchGroupRequest : BaseRequest
	{
		private static readonly Regex BOARD_RE = new Regex("<a href=bbsdoc\\?board=(\\w+?)> ○ (\\w+?)</a>");
		private static readonly Regex GROUP_RE = new Regex("\\[(\\w+?)区\\]<hr");
		private int Sid;

		public FetchGroupRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchGroupList(int sid)
		{
			this.Sid = sid;
			ParameterList qry = new ParameterList();
			qry.Add("sec", sid.ToString());
			DoAction(FetchGroupCompleted, "bbsboa", qry);
		}

		private void FetchGroupCompleted(object sender, BaseEventArgs e)
		{
			string html = e.Result as string;
			Group group = new Group(Sid, GROUP_RE.Match(html).Groups[1].ToString());
			MatchCollection items = BOARD_RE.Matches(html);
			foreach (Match i in items)
			{
				Board board = new Board(i.Groups[1].ToString(), i.Groups[2].ToString());
				group.Add(board);
			}
			callback(this, new BaseEventArgs(group));
		}
	}
}
