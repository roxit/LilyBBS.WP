using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace LilyBBS.Models
{
	[DataContract()]
	public class Topic : INotifyPropertyChanged
	{
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

		private int nextIdx;
		[DataMember(Name = "nextIdx")]
		public int NextIdx
		{
			get
			{
				return nextIdx;
			}
			set
			{
				nextIdx = value; NotifyPropertyChanged("NextIdx");
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

		private ObservableCollection<Post> posts;
		[DataMember(Name = "posts")]
		public ObservableCollection<Post> Posts
		{
			get
			{
				return posts;
			}
			set
			{
				posts = value; NotifyPropertyChanged("Posts");
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
