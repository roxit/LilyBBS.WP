using System.Collections.Generic;
using System.Xml.Linq;
using System;

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

	public class BoardManager : List<Section>
	{
		private static BoardManager instance;
		private BoardManager() {
			XDocument doc = XDocument.Load("Resources/BoardManager.xml");
			foreach (var s in doc.Root.Elements("Section"))
			{
				int sid = Convert.ToInt32(s.Attribute("sid").Value);
				string text = s.Attribute("text").Value;
				Section sec = new Section(sid, text);
				this.Add(sec);
				foreach (var b in s.Elements("Board"))
				{
					Board brd = new Board(b.Attribute("name").Value, b.Attribute("text").Value);
					sec.Add(brd);
				}
			}
		}
		public static BoardManager Instance {
			get
			{
				if (instance == null)
				{
					instance = new BoardManager();
				}
				return instance;
			}
		}
	}

}
