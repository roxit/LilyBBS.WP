using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LilyBBS.DAL
{
	public class BoardManager : List<Section>
	{
		private static BoardManager instance;
		private Dictionary<string, Board> boards;

		private BoardManager()
		{
			boards = new Dictionary<string, Board>();

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
					boards[brd.Name] = brd;
				}
			}
		}

		public static BoardManager Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new BoardManager();
				}
				return instance;
			}
		}

		public static string GetBoardText(string name)
		{
			return Instance.boards[name].Text;
		}

		public static string GetSectionName(int idx)
		{
			// TODO make sure idx == Instance[idx].Sid
			return Instance[idx].Name;
		}
	}
}
