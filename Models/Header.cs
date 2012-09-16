using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace LilyBBS.Models
{
	[DataContract()]
	public class Header : ModelBase
	{

		private string author;
		[DataMember(Name = "author")]
		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				author = value; NotifyPropertyChanged("Author");
			}
		}

		private string board;
		[DataMember(Name = "board")]
		public string Board
		{
			get
			{
				return board;
			}
			set
			{
				board = value; NotifyPropertyChanged("Board");
			}
		}

		private DateTime date;
		[DataMember(Name = "date")]
		public DateTime Date
		{
			get
			{
				return date;
			}
			set
			{
				date = value; NotifyPropertyChanged("Date");
			}
		}

		private int num;
		[DataMember(Name = "num")]
		public int Num
		{
			get
			{
				return num;
			}
			set
			{
				num = value; NotifyPropertyChanged("Num");
			}
		}

		private int pid;
		[DataMember(Name = "pid")]
		public int Pid
		{
			get
			{
				return pid;
			}
			set
			{
				pid = value; NotifyPropertyChanged("Pid");
			}
		}

		private int replyCount;
		[DataMember(Name = "replyCount")]
		public int ReplyCount
		{
			get
			{
				return replyCount;
			}
			set
			{
				replyCount = value; NotifyPropertyChanged("ReplyCount");
			}
		}

		private string title;
		[DataMember(Name = "title")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value; NotifyPropertyChanged("Title");
			}
		}

		private int viewCount;
		[DataMember(Name = "viewCount")]
		public int ViewCount
		{
			get
			{
				return viewCount;
			}
			set
			{
				viewCount = value; NotifyPropertyChanged("ViewCount");
			}
		}

	}
}
