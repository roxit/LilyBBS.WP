using System.Collections.Generic;

namespace LilyBBS.API
{
	public class Board
	{
		public string Name { get; private set; }
		public string Text { get; private set; }

		public Board(string name, string text)
		{
			this.Name = name;
			this.Text = text;
		}
	}

	public class Section
	{
		public int Sid { get; private set; }
		public string Name { get; private set; }
		public List<Board> BoardList { get; private set; }

		public Section(int sid, string name)
		{
			this.Sid = sid;
			this.Name = name;
			this.BoardList = new List<Board>();
		}

		public void Add(Board board)
		{
			BoardList.Add(board);
		}
	}
}
