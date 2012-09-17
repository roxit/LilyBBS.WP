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
	public class TopicViewModel : ViewModelBase
	{

		private ObservableCollection<Post> items = new ObservableCollection<Post>();
		public ObservableCollection<Post> Items
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

		private bool isLoading;
		public bool IsLoading
		{
			get
			{
				return isLoading;
			}
			set
			{
				isLoading = value;
				NotifyPropertyChanged("IsLoading");
				NotifyPropertyChanged("HasNext");
			}
		}

		public bool HasNext
		{
			get
			{
				return nextIdx.HasValue;
			}
		}

		private string board = "";
		private int pid;
		private int? nextIdx;

		private WebRequest req;
		public void LoadData(string board, int pid)
		{
			this.board = board;
			this.pid = pid;
			string url = string.Format("{0}/topic/{1}/{2}/", App.BaseUrl, board, pid);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
			Items.Clear();
		}

		public void LoadMore()
		{
			string url = string.Format("{0}/topic/{1}/{2}/{3}/", App.BaseUrl, board, pid, nextIdx);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		public void Refresh()
		{
			string url = string.Format("{0}/topic/{1}/{2}/", App.BaseUrl, board, pid);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
			Items.Clear();
		}

		private void Callback(IAsyncResult result)
		{
			var resp = req.EndGetResponse(result);
			var ser = new DataContractJsonSerializer(typeof(Topic));
			var topic = ser.ReadObject(resp.GetResponseStream()) as Topic;
			nextIdx = topic.NextIdx;
			Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					foreach (var p in topic.Posts)
						Items.Add(p);
				});
		}
	}

}
