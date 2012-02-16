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
		private const string NotFirstRunKey = "NotFirstRun";

		private T RetrieveSetting<T>(string key)
		{
			object ret;
			if (IsolatedStorageSettings.ApplicationSettings.TryGetValue(key, out ret))
			{
				return (T)ret;
			}
			return default(T);
		}

		public bool NotFirstRun
		{
			get
			{
				return RetrieveSetting<bool>(NotFirstRunKey);
			}
			set
			{
				IsolatedStorageSettings.ApplicationSettings[NotFirstRunKey] = value;
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
