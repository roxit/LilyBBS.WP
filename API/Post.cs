using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	public class Post
	{
		public string Author { get; set; }
		public string Board { get; set; }
		public string Body { get; set; }
		// public string Ip { get; set; }
		public int Num { get; set; }
		public int Pid { get; set; }
		// public DateTime Time { get; set; }
		public string Title { get; set; }

		private static Regex AUTHOR_RE = new Regex(@"发信人: ([\w|\d]+)? \(");
		// multiple ip's
		// private static Regex IP_RE = new Regex(@"\[FROM: (.+)\]");
		private static Regex TIME_RE = new Regex(@"发信站: .*?\((.+)?\)");
		private static string TIME_FMT = "ddd MMM dd HH:mm:ss yyyy";
		private static Regex TITLE_RE = new Regex(@"标  题: (.+)?\n");

		public Post(int pid, string board, int num)
		{
			Pid = pid;
			Board = board;
			Num = num;
		}

		/*
		public void ParsePost(string txt)
		{
			int ia = 0;
			int ib = txt.IndexOf("\n", ia);
			string s = txt.Substring(ia, ib - ia);
			Author = AUTHOR_RE.Match(txt.Substring(ia, ib-ia)).Groups[1].ToString();
			ia = ib+1;
			ib = txt.IndexOf("\n", ia);
			s = txt.Substring(ia, ib - ia);
			Title = TITLE_RE.Match(txt.Substring(ia, ib-ia)).Groups[1].ToString();
			ia = ib+1;
			ib = txt.IndexOf("\n", ia);
			s = txt.Substring(ia, ib - ia);
			string timeStr = TIME_RE.Match(txt.Substring(ia, ib-ia)).Groups[1].ToString();
			timeStr = timeStr.Replace("  ", " 0");
			Time = DateTime.ParseExact(timeStr, TIME_FMT, CultureInfo.InvariantCulture);
			ia = ib+2;
			ib = txt.LastIndexOf("\r\n")+2;
			Body = txt.Substring(ia, ib-ia);
			Ip = IP_RE.Match(txt.Substring(ib, txt.Length-ib)).Groups[1].ToString();
		}
		*/
		public void ParsePost(string txt)
		{
			// just ignore the f**king exceptions
			// Trim() everything
//			try {
				Author = AUTHOR_RE.Match(txt).Groups[1].ToString().Trim();
				Title = TITLE_RE.Match(txt).Groups[1].ToString().Trim();
				Match timeMatch = TIME_RE.Match(txt);
				/*
				Time = DateTime.ParseExact(timeMatch.Groups[1].ToString().Replace("  ", " 0"),
					TIME_FMT, CultureInfo.InvariantCulture);
				 */
				int begIdx = txt.IndexOf("\n", timeMatch.Index);
				int endIdx = txt.LastIndexOf("--\n");
				Body = txt.Substring(begIdx, endIdx-begIdx).Trim();
//			} catch (Exception e)
			{
			}
		}

	}

}
