using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Serialization.Json;
using LilyBBS.Models;

namespace LilyBBS.ViewModels
{
	public class TopTopicViewModel : ViewModelBase
	{

		private ObservableCollection<Header> items = new ObservableCollection<Header>();
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
