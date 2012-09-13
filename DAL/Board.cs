using System.Collections.Generic;
using System.Xml.Linq;
using System;

namespace LilyBBS.DAL
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

	public class Section : List<Board>
	{
		public int Sid { get; private set; }
		public string Name { get; private set; }

		public Section(int sid, string name)
		{
			this.Sid = sid;
			this.Name = name;
		}
	}

}
