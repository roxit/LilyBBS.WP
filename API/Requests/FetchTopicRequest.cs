using HtmlAgilityPack;

namespace LilyBBS.API
{
	public class FetchTopicRequest : BaseRequest
	{
		private int Pid;
		private string Board;
		private int Num;

		public FetchTopicRequest(Connection connection, BaseHandler callback)
			: base(connection, callback)
		{
		}

		public void FetchTopic(int pid, string board)
		{
			this.Pid = pid;
			this.Board = board;

			ParameterList qry = new ParameterList();
			qry.Add("board", board);
			qry.Add("file", Utils.Pid2Str(pid));
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
			for (int i = 0; i < items.Count; i++)
			{
				string c = items[i].SelectSingleNode("tr/td/a").GetAttributeValue("href", "");
				Post p = new Post(Utils.ParsePid(c), topic.Board, Utils.ParserNum(c));
				c = bodies[i].InnerHtml;
				p.ParsePost(c);
				topic.PostList.Add(p);
			}
			callback(this, new BaseEventArgs(topic));
		}
	}
}
