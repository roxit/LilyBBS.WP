using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace LilyBBS.API
{
	public class HeaderGroup : List<Header>
	{
		public int Sid { get; private set; }
		public HeaderGroup(int sid)
		{
			Sid = sid;
		}

		public string GroupName
		{
			get
			{
				return BoardManager.GetSectionName(Sid);
			}
		}
	}
}
