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
using System.Collections.ObjectModel;
using LilyBBS.Models;
using System.Runtime.Serialization.Json;

namespace LilyBBS.ViewModels
{
	public class TopTopicViewModel : ViewModelBase
	{

		private ObservableCollection<Header> items;
		public ObservableCollection<Header> Items
		{
			get
			{
				return items;	
			}
			set
			{
				items = value;
				NotifyPropertyChanged("Items");
			}
		}

		private WebRequest req;
		public void LoadData()
		{
			req = HttpWebRequest.Create(App.BaseUrl + "/top/");
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		private void Callback(IAsyncResult result)
		{
			var resp = req.EndGetResponse(result);
			var ser = new DataContractJsonSerializer(typeof(ObservableCollection<Header>));
			Items = ser.ReadObject(resp.GetResponseStream()) as ObservableCollection<Header>;
		}

	}
}
