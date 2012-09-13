using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace LilyBBS.Models
{
	[DataContract()]
	public class Page : INotifyPropertyChanged
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

		private int prevIdx;
		[DataMember(Name = "prevIdx")]
		public int PrevIdx
		{
			get
			{
				return prevIdx;
			}
			set
			{
				prevIdx = value; NotifyPropertyChanged("PrevIdx");
			}
		}

		private ObservableCollection<Header> headers;
		[DataMember(Name = "headers")]
		public ObservableCollection<Header> Headers
		{
			get
			{
				return headers;
			}
			set
			{
				headers = value; NotifyPropertyChanged("Headers");
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
