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
using System.IO.IsolatedStorage;

namespace LilyBBS
{
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
}
