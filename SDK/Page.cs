using System.Collections.Generic;

namespace LilyBBS.SDK
{
	public class Page
	{
		public string Board { get; set; }
		//public int Start { get; set; }
		public int PrevStart { get; set; }
		public List<Header> HeaderList { get; set; }
		
		public Page(string board=null)
		{
			Board = board;
			HeaderList = new List<Header>();
		}

	}
}
