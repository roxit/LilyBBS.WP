using System;
using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	public class Post
	{
		public static readonly int MAX_LINE_LEN = 37;
		public string Author { get; set; }
		public string Board { get; set; }
		public string Body { get; set; }
		// `Gid` will be send as 'pid' when replying a post
		public int Gid { get; set; }
		// public string Ip { get; set; }
		public int Num { get; set; }
		public int Pid { get; set; }
		// public DateTime Time { get; set; }
		public string Title { get; set; }
		public int Floor { get; set; }

		private static readonly Regex AUTHOR_RE = new Regex(@"发信人: (\w+?) \(", RegexOptions.Compiled);
		// multiple ip's
		// private static Regex IP_RE = new Regex(@"\[FROM: (.+)\]");
		private static readonly Regex TIME_RE = new Regex(@"发信站: .*?\((.+?)\)", RegexOptions.Compiled);
		// private static string TIME_FMT = "ddd MMM dd HH:mm:ss yyyy";
		private static readonly Regex TITLE_RE = new Regex(@"标  题: (.+?)\n", RegexOptions.Compiled);
		private static readonly Regex COLOR_RE = new Regex(@"\x1b\[\d*(;\d+)?m");

		public Post(string board, int pid, int num, int floor=-1)
		{
			Board = board;
			Pid = pid;
			Num = num;
			Floor = floor;
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
			try {
				Author = AUTHOR_RE.Match(txt).Groups[1].ToString().Trim();
				Title = TITLE_RE.Match(txt).Groups[1].ToString().Trim();
				Match timeMatch = TIME_RE.Match(txt);
				/*
				Time = DateTime.ParseExact(timeMatch.Groups[1].ToString().Replace("  ", " 0"),
					TIME_FMT, CultureInfo.InvariantCulture);
				 */
				int begIdx = txt.IndexOf("\n", timeMatch.Index);
				int endIdx = txt.LastIndexOf("--\n");
				if (endIdx == -1)
				{
					endIdx = txt.Length - 1;
				}
				string raw = txt.Substring(begIdx, endIdx-begIdx).Trim();
				raw = COLOR_RE.Replace(raw, "");
				Body = raw;
				/*
				List<String> body = new List<string>();
				var lines = raw.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
				string prev = lines[0];
				int prevLen = prev.Length;
				body.Add(prev);
				for (int i = 1; i < lines.Length; i++)
				{
					lines[i] = lines[i].Trim();
					//if (lines[i].Trim().Length == 0) continue;
					if (lines[i][0] != ' ' && prevLen > MAX_LINE_LEN)
						body[body.Count - 1] = prev + lines[i];
					else
						body.Add(lines[i]);
					prevLen = lines[i].Length;
					prev = body[body.Count - 1];
				}
				Body = string.Join("\n", body);
				*/
			} catch (Exception)
			{
				Author = "错误的用户";
				Title = "错误的标题";
				Body = "错误的正文";
			}
		}

	}

}
