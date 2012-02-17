using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	class LilyError : Exception
	{
	}

	public class Utils
	{
		public static readonly Encoding Enc = new GB2312.GB2312Encoding();
		private static readonly Regex PID_RE = new Regex(@"M\.(\d+)\.A");
		private static readonly Regex NUM_RE = new Regex(@"num=(\d+)");

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

	public class ParameterList
	{
		public List<KeyValuePair<string, string>> Items { get; private set; }
		private static readonly List<char> ENC_CHARS = new List<char>()
		{
			// put % first
			'%', '+', '{', '}', '|', '\\', '^', '~',
			'[', ']', '`', ';', '/', '?', ':', '@', '=', '&', '$'//,
			//'#', '<', '>', ' ',
		};

		public ParameterList()
		{
			this.Items = new List<KeyValuePair<string, string>>(5);
		}

		public void Add(string key, string value)
		{
			this.Items.Add(new KeyValuePair<string, string>(key, value));
		}

		private static string UrlEscape(string s)
		{
			foreach (char c in ENC_CHARS)
				s = s.Replace(c.ToString(), string.Format("%{0}", Convert.ToString(c, 16)));
			s = s.Replace(" ", "+");
			return s;
		}

		public string BuildQueryString()
		{
			StringBuilder sb = new StringBuilder();
			foreach (var i in Items)
			{
				if (sb.Length != 0) sb.Append("&");
				// WebClient will encode Chinese
				sb.AppendFormat("{0}={1}", UrlEscape(i.Key), UrlEscape(i.Value));
			}
			return sb.ToString();
		}
	}
}
