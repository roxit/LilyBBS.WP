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
	public class HotTopicViewModel : ViewModelBase
	{
		private ObservableCollection<HeaderGroup> items = new ObservableCollection<HeaderGroup>();
		public ObservableCollection<HeaderGroup> Items
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
			req = HttpWebRequest.Create(App.BaseUrl + "/hot/");
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		private void Callback(IAsyncResult result)
		{
			var resp = req.EndGetResponse(result);
			var ser = new DataContractJsonSerializer(typeof(ObservableCollection<ObservableCollection<Header>>));
			var data = ser.ReadObject(resp.GetResponseStream()) as ObservableCollection<ObservableCollection<Header>>;

			var tmp = new ObservableCollection<HeaderGroup>();
			int sid = 0;
			foreach (var i in data)
			{
				HeaderGroup hg = new HeaderGroup(sid);
				foreach (var h in i)
					hg.Add(h);
				tmp.Add(hg);
			}
			Items = tmp;
		}

	}
}
