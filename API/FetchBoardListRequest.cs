using System.Collections.Generic;

namespace LilyBBS.API
{
	// a fake request
	public class FetchBoardListRequest
	{
		private static readonly int GROUP_COUNT = 12;

		protected Connection connection;
		protected BaseHandler callback;

		public Dictionary<int, Group> BoardList { get; private set; }

		public FetchBoardListRequest(Connection connection, BaseHandler callback)
		{
			this.connection = connection;
			this.callback = callback;
			this.BoardList = new Dictionary<int,Group>();
		}

		public void FetchBoardList()
		{
			for (int i = 0; i < GROUP_COUNT; i++)
			{
				FetchGroupRequest req = new FetchGroupRequest(connection, FetchGroupCompleted);
				req.FetchGroupList(i);
			}
		}

		private void FetchGroupCompleted(object sender, BaseEventArgs e)
		{
			Group group = e.Result as Group;
			BoardList[group.Sid] = group;
			if (BoardList.Count == GROUP_COUNT)
			{
				callback(this, new BaseEventArgs(BoardList));
			}
		}

	}
}
