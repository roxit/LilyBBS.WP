using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LilyBBS.API
{
	public class Connection
	{
		//public bool IsLoggedIn { get; set; }
		public static readonly string BbsUrl = "http://bbs.nju.edu.cn/";
		public string BaseUrl { get; set; }
		public string Cookie { get; set; }

		public Connection()
		{
			//IsLoggedIn = false;
			BaseUrl = BbsUrl;
		}

		public void Login(BaseHandler callback, string username, string password)
		{
			LoginRequest req = new LoginRequest(this, callback);
			req.Login(username, password);
		}

		public void IsLoggedIn(BaseHandler callback)
		{
			IsLoggedInRequest req = new IsLoggedInRequest(this, callback);
			req.IsLoggedIn();
		}

		public void SendPost(BaseHandler callback, string brd, string title, string text, string autocr="on", int signature=1)
		{
			SendPostRequest req = new SendPostRequest(this, callback);
			req.SendPost(brd, title, text);
		}

		public void FetchPost(BaseHandler callback, int pid, string board, int num)
		{
			FetchPostRequest req = new FetchPostRequest(this, callback);
			req.FetchPost(pid, board, num);
		}

		public void FetchTopic(BaseHandler callback, string board, int pid, int? start=null)
		{
			FetchTopicRequest req = new FetchTopicRequest(this, callback);
			req.FetchTopic(board, pid, start);
		}

		public void FetchPage(BaseHandler callback, string board, int start=-1)
		{
			FetchPageRequest req = new FetchPageRequest(this, callback);
			req.FetchPage(board, start);
		}

		public void FetchTopTenList(BaseHandler callback)
		{
			FetchTopTenListRequest req = new FetchTopTenListRequest(this, callback);
			req.FetchTopTenList();
		}

		public void FetchHotList(BaseHandler callback)
		{
			FetchHotListRequest req = new FetchHotListRequest(this, callback);
			req.FetchHotList();
		}

		#region FetchBoardList
		public void FetchBoardList(BaseHandler callback)
		{
			FetchBoardListRequest req = new FetchBoardListRequest(this, callback);
			req.FetchBoardList();
		}

		private void FetchBoardListCompleted(object sender, BaseEventArgs e)
		{
			int i;
		}
		#endregion

	}

}
