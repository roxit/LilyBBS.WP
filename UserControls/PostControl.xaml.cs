using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using ImageTools;
using System.Net;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LilyBBS
{
	public partial class PostControl : UserControl
	{
		private static readonly int MAX_LINE_CHARS = 37;
		private static readonly double MAX_HEIGHT = 800;
		private static readonly string IMG_PREFIX = "http://lilysvc.sinaapp.com/fetch?url=";
		private static readonly Regex IMG_RE = new Regex(@"http://(www\.)?[\w./-]+?\.(jpe?g|gif|png)", RegexOptions.Compiled);
		private static readonly Regex URL_RE = new Regex(@"http://(www\.)?[\w./-]+?", RegexOptions.Compiled);

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

		private static TextBlock buildTextBlock()
		{
			var app = Application.Current as App;
			TextBlock ret = new TextBlock();
			ret.TextWrapping = TextWrapping.Wrap;
			ret.Style = app.Resources["PhoneTextNormalStyle"] as Style;
			ret.FontSize = (app.Resources["PhoneFontSizeMedium"] as double?).Value;
			ret.Margin = new Thickness();
			return ret;
		}

		private static Image buildImage(string src)
		{
			Image img = new Image();
			img.Source = new BitmapImage(new Uri(IMG_PREFIX+src));
			return img;
		}

		private static void addTextBlock(StackPanel panel, TextBlock block)
		{
			if (block.Text.Length > 0)
				panel.Children.Add(block);
		}

		static void OnBodyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			StackPanel panel = (obj as PostControl).BodyPanel;
			panel.Children.Clear();
			string text = args.NewValue as string;
			var lines = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
			string i;
			int prevLen = MAX_LINE_CHARS+1;
			TextBlock block = buildTextBlock();
			foreach (var line in lines)
			{
				i = line.Trim();
				if (i.Length == 0) continue;
				if (isPicture(i))
				{
					addTextBlock(panel, block);
					block = buildTextBlock();
					prevLen = 0;
					var img = buildImage(i);
					panel.Children.Add(img);
				}
				else if (isUrl(i))
				{
				}
				else
				{
					/*
					if (prevLen > MAX_LINE_CHARS)
						block.Text = block.Text + i;
					else
						block.Text = block.Text + "\n" + i;
					 */
					if (block.Text.Length > 0)
						block.Text = block.Text + "\n" + i;
					else
						block.Text = block.Text + i;
					prevLen = i.Length;
					// Defect: next line might be appended to previous block
					if (block.ActualHeight > MAX_HEIGHT) //|| block.ActualWidth > MAX_HEIGHT*8)
					{
						addTextBlock(panel, block);
						block = buildTextBlock();
						prevLen = 0;
					}
				}
			}
			addTextBlock(panel, block);
		}
		#endregion
		
		private static bool isPicture(string s)
		{
			return IMG_RE.IsMatch(s);
		}

		private static bool isUrl(string s)
		{
			return URL_RE.IsMatch(s);
		}

		public PostControl()
		{
			InitializeComponent();
		}

	}
}
