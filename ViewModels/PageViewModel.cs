using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Windows;
using LilyBBS.Models;

namespace LilyBBS.ViewModels
{
	public class PageViewModel : ViewModelBase
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

		private string board = "";
		public string Board
		{
			get
			{
				return board;
			}
			set
			{
				board = value;
				NotifyPropertyChanged("Board");
				NotifyPropertyChanged("BoardText");
			}
		}

		public string BoardText
		{
			get
			{
				return BoardManager.GetBoardText(Board);
			}
		}

		private int? prevIdx;

		private WebRequest req;
		public void LoadData(string board)
		{
			Board = board;
			string url = string.Format("{0}/board/{1}/", App.BaseUrl, Board);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		public void LoadMore()
		{
			string url = string.Format("{0}/board/{1}/{2}/", App.BaseUrl, Board, prevIdx);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
		}

		public void Refresh()
		{
			string url = string.Format("{0}/board/{1}/", App.BaseUrl, Board);
			req = HttpWebRequest.Create(url);
			req.BeginGetResponse(new AsyncCallback(Callback), null);
			Items.Clear();
		}

		private void Callback(IAsyncResult result)
		{
			var resp = req.EndGetResponse(result);
			var ser = new DataContractJsonSerializer(typeof(Page));
			var page = ser.ReadObject(resp.GetResponseStream()) as Page;
			prevIdx = page.PrevIdx;
			Deployment.Current.Dispatcher.BeginInvoke(() =>
				{
					foreach (var h in page.Headers)
						Items.Add(h);
				});
		}
	}
}
