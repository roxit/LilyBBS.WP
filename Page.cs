using System.Collections.Generic;

namespace LilyBBS
{
	public class Page
	{
		public string Board { get; set; }
		public int Start { get; set; }
		public List<Header> HeaderList { get; set; }
		
		public Page(string board=null, int start=-1)
		{
			Board = board;
			Start = start;
			HeaderList = new List<Header>();
		}

	}
}
