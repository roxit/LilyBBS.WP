using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Serialization.Json;
using LilyBBS.Models;

namespace LilyBBS.ViewModels
{
	public class HotViewModel : ViewModelBase
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
			req = HttpWebRequest.Create(string.Format("{0}/{1}/", App.BaseUrl, "hot"));
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		private void Callback(IAsyncResult result)
		{
			var resp = req.EndGetResponse(result);
			var ser = new DataContractJsonSerializer(typeof(ObservableCollection<ObservableCollection<Header>>));
			// TODO
			var stream = resp.GetResponseStream();
			var data = ser.ReadObject(stream) as ObservableCollection<ObservableCollection<Header>>;

			var tmp = new ObservableCollection<HeaderGroup>();
			int sid = 0;
			// TODO
			//Items.Clear();
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
