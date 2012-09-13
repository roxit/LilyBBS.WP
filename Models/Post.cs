using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace LilyBBS.Models
{
	[DataContract()]
	public class Post : INotifyPropertyChanged
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

		private string body;
		[DataMember(Name = "body")]
		public string Body
		{
			get
			{
				return body;
			}
			set
			{
				body = value; NotifyPropertyChanged("Body");
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

		private string ip;
		[DataMember(Name = "ip")]
		public string IP
		{
			get
			{
				return ip;
			}
			set
			{
				ip = value; NotifyPropertyChanged("IP");
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

		public event PropertyChangedEventHandler PropertyChanged;
		private void NotifyPropertyChanged(String propertyName)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
