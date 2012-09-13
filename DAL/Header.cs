using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ComponentModel;
using System;

namespace LilyBBS.DAL
{
	[DataContract()]
	public class Header : INotifyPropertyChanged
	{
		private int pid;
		[DataMemberAttribute(Name = "pid")]
		public int Pid
		{
			get
			{
				return pid;
			}
			set
			{
				pid = value;
				NotifyPropertyChanged("Pid");
			}
		}

		private string board;
		[DataMemberAttribute(Name = "board")]
		public string Board
		{
			get
			{
				return board;
			}
			set
			{
				board = value;
				NotifyPropertyChanged("Board");
			}
		}

		private string author;
		[DataMemberAttribute(Name = "author")]
		public string Author
		{
			get
			{
				return author;
			}
			set
			{
				author = value;
				NotifyPropertyChanged("Author");
			}
		}

		private string title;
		[DataMemberAttribute(Name = "title")]
		public string Title
		{
			get
			{
				return title;
			}
			set
			{
				title = value;
				NotifyPropertyChanged("Title");
			}
		}

		private int replyCount;
		[DataMemberAttribute(Name = "replyCount")]
		public int ReplyCount
		{
			get
			{
				return replyCount;
			}
			set
			{
				replyCount = value;
				NotifyPropertyChanged("ReplyCount");
			}
		}

		private int viewCount;
		[DataMemberAttribute(Name = "viewCount")]
		public int ViewCount
		{
			get
			{
				return viewCount;
			}
			set
			{
				viewCount = value;
				NotifyPropertyChanged("ViewCount");
			}
		}

		private string date;
		[DataMemberAttribute(Name = "date")]
		public string Date
		{
			get
			{
				return date;
			}
			set
			{
				date = value;
				NotifyPropertyChanged("Date");
			}
		}

		private int num;
		[DataMemberAttribute(Name = "num")]
		public int Num
		{
			get
			{
				return num;
			}
			set
			{
				num = value;
				NotifyPropertyChanged("Num");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
