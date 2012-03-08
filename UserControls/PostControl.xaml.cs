using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace LilyBBS
{
	public partial class PostControl : UserControl
	{
		private static readonly int MAX_LINES = 25;		// !!!
		#region Author
		public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register("Author", 
				typeof(string),
				typeof(PostControl),
				new PropertyMetadata(OnAuthorChanged));

		public string Author
		{
			get { return GetValue(AuthorProperty) as string; }
			set { SetValue(AuthorProperty, value); }
		}

		static void OnAuthorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as PostControl).AuthorTextBlock.Text = args.NewValue as string;
		}
		#endregion

		#region Floor
		public static readonly DependencyProperty FloorProperty = DependencyProperty.Register("Floor",
				typeof(int?),
				typeof(PostControl),
				new PropertyMetadata(OnFloorChanged));

		public int? Floor
		{
			get
			{
				return GetValue(FloorProperty) as int?;
			}
			set { SetValue(FloorProperty, value); }
		}

		static void OnFloorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			TextBlock textBlock = (obj as PostControl).FloorTextBlock;
			if (args.NewValue == null)
			{
				textBlock.Text = "";
				return;
			}
			int floor = (args.NewValue as int?).Value;
			string s;
			switch (floor)
			{
				case 0:
					s = "楼主"; break;
				case 1:
					s = "沙发"; break;
				case 2:
					s = "板凳"; break;
				default:
					s = floor.ToString() + "楼"; break;
			}
			textBlock.Text = s;
		}
		#endregion

		#region Body
		public static readonly DependencyProperty BodyProperty = DependencyProperty.Register("Body",
				typeof(string),
				typeof(PostControl),
				new PropertyMetadata(OnBodyChanged));

		public string Body
		{
			get { return GetValue(BodyProperty) as string; }
			set { SetValue(BodyProperty, value); }
		}

		private static bool isPicture(string s)
		{
			return false;
		}

		private static bool isUrl(string s)
		{
			return false;
		}

		private static TextBlock buildTextBlock(string s)
		{
			var app = Application.Current as App;
			TextBlock ret = new TextBlock();
			ret.Text = s;
			ret.TextWrapping = TextWrapping.Wrap;
			ret.Style = app.Resources["PhoneTextNormalStyle"] as Style;
			ret.FontSize = (app.Resources["PhoneFontSizeMedium"] as double?).Value;
			ret.Margin = new Thickness();
			return ret;
		}

		static void OnBodyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			StackPanel panel = (obj as PostControl).BodyPanel;
			string text = args.NewValue as string;
			var lines = text.Split('\n');
			List<string> buf = new List<string>();
			int count = 0;
			foreach (var i in lines)
			{
				if (isPicture(i))
				{

					count = 0;
					buf.Clear();
				}
				else if (isUrl(i))
				{

					count = 0;
					buf.Clear();
				}
				else
				{
					buf.Add(i);
					if (++count < MAX_LINES) continue;
					panel.Children.Add(buildTextBlock(string.Join("\n", buf)));
					count = 0;
					buf.Clear();
				}
			}
			if (buf.Count > 0)
			{
				panel.Children.Add(buildTextBlock(string.Join("\n", buf)));
			}

		}
		#endregion

		public PostControl()
		{
			InitializeComponent();
		}

	}
}
