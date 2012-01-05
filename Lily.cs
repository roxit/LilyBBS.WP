using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace LilyBBS
{
	using ParameterDict = Dictionary<string, string>;

	class LilyError : Exception
	{
	}

	class LilyUtils
	{
		private static Regex PID_RE = new Regex(@"M\.(\d+)\.A");
		private static Regex NUM_RE = new Regex(@"num=(\d+)");
		private static string UrlEscape(string s)
		{
			List<char> ENC_CHARS = new List<char>()
			{
				' ', '%', '{', '}', '|', '\\', '^', '~',
				'[', ']', '`', ';', '/', '?', ':', '@', '=', '&', '$'//,
				//'#', '<', '>',
			};

			foreach (char c in ENC_CHARS)
			{
				s = s.Replace(c.ToString(), string.Format("%{0}", Convert.ToString(c, 16)));
			}
			return s;
		}

		public static string ConstructQueryString(ParameterDict qry)
		{
			if (qry == null) return "";
			List<string> s = new List<string>();
			foreach (string i in qry.Keys)
				s.Add(string.Format("{0}={1}", i, qry[i]));
			return string.Join("&", s);
		}

		public static string ConstructPostData(ParameterDict data)
		{
			List<string> s = new List<string>();
			foreach (string i in data.Keys)
			{
				s.Add(string.Format("{0}={1}", i, LilyUtils.UrlEscape(data[i])));
			}
			return string.Join("&", s);
		}

		public static string Pid2Str(int pid)
		{
			return string.Format("M.{0}.A", pid);
		}

		public static int ParsePid(string s)
		{
			return int.Parse(PID_RE.Match(s).Groups[1].ToString());
		}

		public static int ParserNum(string s)
		{
			return int.Parse(NUM_RE.Match(s).Groups[1].ToString());
		}
	}

	public class LilyArgs
	{
		public object Result { get; private set; }
		public Exception Error { get; private set; }
		public LilyArgs(object result=null, Exception error=null)
		{
			Result = result;
			Error = error;
		}
	}

	public delegate void LilyCallback(LilyArgs e);

	public class LilyConnection
	{
		public static Encoding Enc = new GB2312.GB2312Encoding();
		public bool IsLoggedIn { get; private set; }
		private string BBSURL = "http://bbs.nju.edu.cn/";
		private string baseUrl;
		private WebClient client = new WebClient();
		private LilyCallback callback;
		private LilyCallback internalCallback;
		private Dictionary<string, object> args;

		public LilyConnection()
		{
			IsLoggedIn = false;
			baseUrl = BBSURL;
			client = new WebClient();
			client.Encoding = Enc;
			client.UploadStringCompleted += client_UploadStringCompleted;
			client.DownloadStringCompleted += client_DownloadStringCompleted;
		}

		#region DoAction
		private void client_UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
		{
			if (e.Cancelled) return;
			if (internalCallback != null)
			{
				internalCallback(new LilyArgs(e.Result, e.Error));
			}
		}

		private void client_DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
		{
			if (e.Cancelled) return;
			if (internalCallback != null)
			{
				internalCallback(new LilyArgs(e.Result, e.Error));
			}
		}

		private void DoAction(LilyCallback internalCallback, string action, ParameterDict qry=null, ParameterDict data=null)
		{
			if (client.IsBusy)
			{
				client.CancelAsync();
			}
			this.internalCallback = internalCallback;
			StringBuilder sb = new StringBuilder(baseUrl);
			sb.Append(action);
			sb.Append("?");
			sb.Append(LilyUtils.ConstructQueryString(qry));
			string url = sb.ToString();
			if (data != null)
			{
				client.UploadStringAsync(new Uri(url), LilyUtils.ConstructPostData(data));
			}
			else
			{
				client.DownloadStringAsync(new Uri(url));
			}
		}
		#endregion

		#region Login
		public void Login(LilyCallback callback, string username, string password)
		{
			this.callback = callback;
			Random rand = new Random();
			baseUrl = string.Format("{0}/vd{1}/", baseUrl, rand.Next(10000, 100000));

			ParameterDict qry = new ParameterDict()
			{
				{"type", "2"}
			};
			ParameterDict data = new ParameterDict()
			{
				{"id", username},
				{"pw", password}
			};
			DoAction(LoginCompleted, "bbslogin", qry, data);
		}

		private void LoginCompleted(LilyArgs e)
		{
			Regex re = new Regex(@"setCookie\('(.*)'\)");
			string s = re.Match(e.Result as string).Groups[1].ToString();
			if (s == "")
			{
				// TODO
				throw new LilyError();
			}
			StringBuilder sb = new StringBuilder();
			string[] ss = s.Split(new char[] { '+' });
			sb.AppendFormat("_U_KEY={0}; ", int.Parse(ss[1]) - 2);
			ss = ss[0].Split(new char[] { 'N' });
			sb.AppendFormat("_U_UID={0}; ", ss[1]);
			sb.AppendFormat("_U_NUM={0}", int.Parse(ss[0]) + 2);
			client.Headers["Cookie"] = sb.ToString();
			IsLoggedIn = true;
			if (callback != null)
			{
				callback(new LilyArgs(sb.ToString(), e.Error));		// return the cookie for now
			}
		}
		#endregion

		#region SendPost
		public void SendPost(LilyCallback callback, string brd, string title, string text, string autocr="on", int signature=1)
		{
			this.callback = callback;
			ParameterDict qry = new ParameterDict()
			{
				{"board", brd}
			};
			ParameterDict data = new ParameterDict()
			{
				{"title", title},
				{"text", text},
				{"autocr", autocr},
				{"signature", signature.ToString()}
			};
			DoAction(SendPostCompleted, "bbssnd", qry, data);
		}

		private void SendPostCompleted(LilyArgs e)
		{
			if (callback != null)
			{
				callback(new LilyArgs());
			}
		}
		#endregion

		#region FetchPost
		public void FetchPost(LilyCallback callback, int pid, string board, int num)
		{
			this.callback = callback;
			args = new Dictionary<string, object>()
			{
				{"pid", pid},
				{"board", board},
				{"num", num}
			};
			ParameterDict qry = new ParameterDict()
			{
				{"board", board},
				{"file", LilyUtils.Pid2Str(pid)},
				{"num", num.ToString()}
			};
			DoAction(FetchPostCompleted, "bbscon", qry);
		}

		private void FetchPostCompleted(LilyArgs e)
		{
			if (this.callback == null) return;
			Post post = new Post(((int)args["pid"]), args["board"] as string, ((int)args["num"]));
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			string txt = doc.DocumentNode.SelectNodes("//textarea")[0].InnerHtml;
			post.ParserPost(txt);
			callback(new LilyArgs(post));
		}
		#endregion

		#region FetchTopic
		public void FetchTopic(LilyCallback callback, int pid, string board)
		{
			this.callback = callback;
			args = new Dictionary<string, object>()
			{
				{"pid", pid},
				{"board", board},
			};
			ParameterDict qry = new ParameterDict()
			{
				{"board", board},
				{"file", LilyUtils.Pid2Str(pid)},
			};
			DoAction(FetchTopicCompleted, "bbstcon", qry);
		}

		private void FetchTopicCompleted(LilyArgs e)
		{
			if (this.callback == null) return;
			Topic topic = new Topic(((int)args["pid"]), args["board"] as string);
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			var items = doc.DocumentNode.SelectNodes("//table[@class='main']");
			var bodies = doc.DocumentNode.SelectNodes("//textarea");
			for (int i = 0; i < items.Count; i++)
			{
				string c = items[i].SelectSingleNode("tr/td/a").GetAttributeValue("href", "");
				Post p = new Post(LilyUtils.ParsePid(c), topic.Board, LilyUtils.ParserNum(c));
				c = bodies[i].InnerHtml;
				p.ParserPost(c);
				topic.PostList.Add(p);
			}
			callback(new LilyArgs(topic));
		}
		#endregion

		#region FetchPage
		public void FetchPage(LilyCallback callback, string board, int start=-1)
		{
			this.callback = callback;
			args = new Dictionary<string, object>()
			{
				{"board", board},
			};
			if (start != -1)
				args.Add("start", start.ToString());

			ParameterDict qry = new ParameterDict()
			{
				{"board", board},
			};
			if (start != -1)
				qry.Add("start", start.ToString());
			DoAction(this.FetchPageCompleted, "bbstdoc", qry);
		}

		private void FetchPageCompleted(LilyArgs e)
		{
			if (this.callback == null) return;
			Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A");
			Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)</a>");
			Regex AUTHOR_RE = new Regex(@"bbsqry\?userid=(\w+)");
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			List<Header> headerList = new List<Header>();

			string txt = doc.DocumentNode.SelectSingleNode("//table").InnerHtml;
			Page page = new Page(args["board"] as string);
			if (args.ContainsKey("start"))
			{
				page.Start = (int)args["start"];
			}
			var authorList = AUTHOR_RE.Matches(txt);
			var pidList = PID_RE.Matches(txt);
			var titleList = TITLE_RE.Matches(txt);
			for (int i = 0; i < authorList.Count; i++)
			{
				Header h = new Header();
				h.Author = authorList[i].Groups[1].ToString().Trim();
				h.Board = page.Board;
				h.Pid = int.Parse(pidList[i].Groups[2].ToString().Trim());
				h.Title = titleList[i].Groups[1].ToString().Trim().Remove(0, 2);
				page.HeaderList.Add(h);
			}
			callback(new LilyArgs(page));
		}
		#endregion

		#region FetchTopTenList
		public void FetchTopTenList(LilyCallback callback)
		{
			this.callback = callback;
			DoAction(FetchTopTenListCompleted, "bbstop10");
		}

		private void FetchTopTenListCompleted(LilyArgs e)
		{
			if (this.callback == null) return;
			Regex PID_RE = new Regex(@"bbstcon\?board=(\w+)\&file=M\.(\d+)\.A");
			Regex TITLE_RE = new Regex(@"bbstcon\?board=.+?>(.*?)\n</a>");
			Regex AUTHOR_RE = new Regex(@"bbsqry\?userid=(\w+)");
			HtmlDocument doc = new HtmlDocument();
			doc.LoadHtml(e.Result as string);
			List<Header> headerList = new List<Header>();
			var items = doc.DocumentNode.SelectNodes("//tr");
			// TODO: I'll try LINQ later
			bool flag = true;
			foreach (var i in items)
			{
				if (flag)
				{
					flag = false; continue;
				}
				Header h = new Header();
				Match m = PID_RE.Match(i.InnerHtml);
				h.Board = m.Groups[1].ToString().Trim();
				h.Pid = int.Parse(m.Groups[2].ToString().Trim());
				h.Title = TITLE_RE.Match(i.InnerHtml).Groups[1].ToString().Trim();
				h.Author = AUTHOR_RE.Match(i.InnerHtml).Groups[1].ToString().Trim();
				headerList.Add(h);
			}
			callback(new LilyArgs(headerList));
		}
		#endregion

	}

}
