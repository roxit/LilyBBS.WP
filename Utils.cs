using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.IsolatedStorage;
using System.Windows;
using System.Windows.Data;
using Coding4Fun.Phone.Controls;

namespace LilyBBS
{
	public class Utils
	{
		public static void ShowIndicator(string text=null)
		{
			var app = Application.Current as App;
			app.Indicator.Text = text;
			app.Indicator.IsVisible = true;
		}

		public static void HideIndicator()
		{
			var app = Application.Current as App;
			app.Indicator.Text = "";
			app.Indicator.IsVisible = false;
		}
	}

	public class LilyToast : ToastPrompt
	{
		public LilyToast(string msg=null) : base()
		{
			Message = msg;
			Margin = new Thickness(-20, -20, 0, 0);
			FontSize = 24;
		}

		public void ShowNetworkError()
		{
			var app = Application.Current as App;
			Message = (app.Resources["NetworkErrorMessage"] as NetworkErrorMessage).Message;
			Show();
		}
	}

	public class NetworkErrorMessage
	{
		private static List<string> msgList;

		public NetworkErrorMessage()
		{
			msgList = new List<string>();
			msgList.Add("连不上网了:-(");
			msgList.Add("没网上个毛毛啊");
			msgList.Add("亲，把天线插好啊");
		}

		public string Message
		{
			get
			{
				Random rnd = new Random();
				return msgList[rnd.Next(msgList.Count)];
			}
		}
	}

	public class Settings
	{
		private const string UsernameKey = "Username";
		private const string PasswordKey = "Password";
		private const string SignatureKey = "Signature";
		private const string IsFirstRunKey = "IsFirstRun";

		private T RetrieveSetting<T>(string key)
		{
			object ret;
			if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out ret))
			{
				return (T)ret;
			}
			return default(T);
		}

		public bool IsFirstRun
		{
			get
			{
				object ret;
				if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(IsFirstRunKey, out ret))
					return (bool)ret;
				else
					return true;
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[IsFirstRunKey] = value;
			}
		}
		// TODO invalidate connection.cookie
		public string Username
		{
			get
			{
				return RetrieveSetting<string>(UsernameKey);
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[UsernameKey] = value;
				var app = Application.Current as App;
				app.LilyApi.Reset();
			}
		}

		public string Password
		{
			get
			{
				return RetrieveSetting<string>(PasswordKey);
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[PasswordKey] = value;
				var app = Application.Current as App;
				app.LilyApi.Reset();
			}
		}

		public string Signature
		{
			get
			{
				return RetrieveSetting<string>(SignatureKey);
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[SignatureKey] = value;
			}
		}
	}

	public class IsLoadingConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			bool isLoading = (value as bool?).Value;
			if (isLoading) return "载入中...";
			else return "更多";
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

}
