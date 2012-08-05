namespace LilyBBS.SDK
{
	public class Header
	{
		public int Pid { get; set; }
		public string Board { get; set; }
		public string Author { get; set; }
		public string Title { get; set; }
		public int ReplyCount { get; set; }
		public int ViewCount { get; set; }
		public Header()
		{
		}

		public string CountText {
			get {
				return string.Format("{0}/{1}", ReplyCount, ViewCount);
			}
		}
	}
}
