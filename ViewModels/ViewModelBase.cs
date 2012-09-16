using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace LilyBBS.ViewModels
{
	public abstract class ViewModelBase : INotifyPropertyChanged
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
