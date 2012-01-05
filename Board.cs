using System.Collections.Generic;

namespace LilyBBS
{
	public class Board
	{
		public string Name { get; set; }
		public string Text { get; set; }
	}

	public class Group
	{
		public int Sid { get; set; }
		public string Name { get; set; }
		public List<Board> BoardList { get; set; }
	}

}
