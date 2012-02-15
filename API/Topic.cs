using System.Collections.Generic;

namespace LilyBBS.API
{
	public class Topic
	{
		// Pid is the same as PostList[0].Pid
		public int Pid { get; set; }
		public string Board { get; set; }
//		public int Num { get; set; }
		public List<Post> PostList { get; set; }
		public int? nextStart { get; set; }
		public Topic(int pid, string board)		//, int num)
		{
			Pid = pid;
			Board = board;
//			Num = num;
			PostList = new List<Post>();
		}
	}
}
