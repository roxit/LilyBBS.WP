using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Windows;

namespace LilyBBS.Models
{
	[DataContract()]
	public abstract class ModelBase : INotifyPropertyChanged
	{

		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged(String propertyName)
		{
			Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
				if (PropertyChanged != null)
					PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
				});
		}

	}
}
