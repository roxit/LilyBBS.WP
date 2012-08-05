using HtmlAgilityPack;

namespace LilyBBS.SDK
{
	public class FetchTopicRequest : BaseRequest
	{
		private string Board;
		private int Pid;
		private int? Start;
		//private int Num;

		public FetchTopicRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchTopic(string board, int pid, int? start=null)
		{
			this.Board = board;
			this.Pid = pid;
			this.Start = start;

			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			qry.Add("file", Utils.Pid2Str(pid));
			if (start != null)
			{
				qry.Add("start", start.Value.ToString());
			}
			DoAction(FetchTopicCompleted, "bbstcon", qry);
		}

		private void FetchTopicCompleted(object sender, BaseEventArgs e)
		{
			if (this.callback == null) return;
			Topic topic = new Topic(Pid, Board);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			var items = doc.DocumentNode.SelectNodes("//table[@class='main']");
			var bodies = doc.DocumentNode.SelectNodes("//textarea");
			int idx, floor;
			if (Start != null)
			{
				idx = 1; floor = Start.Value+1;
			} else {
				idx = 0; floor = 0;
			}
			for (int i = idx; i < items.Count; i++)
			{
				string c = items[i].SelectSingleNode("tr/td/a").GetAttributeValue("href", "");
				Post p = new Post(topic.Board, Utils.ParsePid(c), Utils.ParserNum(c), floor++);
				c = bodies[i].InnerHtml;
				p.ParsePost(c);
				topic.PostList.Add(p);
			}
			if ((e.Result as string).Contains(">本主题下30篇</a>][<a href="))
			{
				topic.nextStart = Start.GetValueOrDefault(0) + 30;
			}
			else
			{
				topic.nextStart = null;
			}
			callback(this, new BaseEventArgs(topic));
		}
	}
}
