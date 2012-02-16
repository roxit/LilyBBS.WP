using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchHotListRequest : BaseRequest
	{
		private static readonly Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A");
		private static readonly Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)\n</a>");
		private static readonly Regex RE = new Regex("○<a href=\"bbstcon\\?board=(\\w+)\\&file=M\\.(\\d+)\\.A\">(.*?)\\n</a> \\[<a href=\"bbsdoc\\?board=(\\w+)\">(\\w+?)</a>\\]\\n<t");
		public FetchHotListRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchHotList()
		{
			DoAction(FetchHotListCompleted, "bbstopall");
		}

		private void FetchHotListCompleted(object sender, BaseEventArgs e)
		{
			if (callback == null) return;
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			List<HeaderGroup> hotList = new List<HeaderGroup>();
			var items = doc.DocumentNode.SelectNodes("//tr");
			HeaderGroup grp = null;
			int sid = 0;

			foreach (var i in items)
			{
				if (i.SelectSingleNode("td/img") != null)
				{
					grp = new HeaderGroup(sid++);
					continue;
				}
				if (i.InnerHtml.StartsWith("<td>\n<tr><td colspan=\"2\">"))
				{
					hotList.Add(grp);
					continue;
				}
				MatchCollection matches = RE.Matches(i.InnerHtml);
				foreach (Match m in matches)
				{
					Header h = new Header();
					h.Board = m.Groups[1].ToString();
					h.Pid = int.Parse(m.Groups[2].ToString());
					h.Title = m.Groups[3].ToString();
					grp.Add(h);
					if (i.InnerHtml[m.Index + m.Length] == 'r')
						break;
				}
			}
			hotList.Add(grp);
			callback(this, new BaseEventArgs(hotList));
		}
	}
}
