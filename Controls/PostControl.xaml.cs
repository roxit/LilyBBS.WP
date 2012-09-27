using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageTools;
using ImageTools.Controls;
using System.IO;
using System.Windows.Documents;
using System.Collections.Generic;
using System.Windows.Resources;
using Newtonsoft.Json;
using System.Windows.Media;

namespace LilyBBS.Views
{
	public partial class PostControl : UserControl
	{

		private static readonly int MAX_LINE_CHARS = 37;
		private static readonly double MAX_HEIGHT = 800;
		private static readonly string IMG_PREFIX = "http://lilysvc.sinaapp.com/fetch?url=";
		private static readonly Regex IMG_RE = new Regex(@"^http://(www\.)?[\w./-]+?\.(jpe?g|gif|png)$", RegexOptions.Compiled);
		private static readonly Regex URL_RE = new Regex(@"^http://(www\.)?[\w./-]+?$", RegexOptions.Compiled);
		private static readonly Regex ICON_RE = new Regex(@"\[[:;].{1,4}\]", RegexOptions.Compiled);
		private static Dictionary<string, string> Icons;

		public PostControl()
		{
			InitializeComponent();

			StreamResourceInfo sri = Application.GetResourceStream(new Uri("Assets/Emoticons.json", UriKind.Relative));
			StreamReader reader = new StreamReader(sri.Stream);
			Icons = JsonConvert.DeserializeObject<Dictionary<string, string>>(reader.ReadToEnd());
	
			ImageTools.IO.Decoders.AddDecoder<ImageTools.IO.Gif.GifDecoder>();
		}

		#region Author
		public static readonly DependencyProperty AuthorProperty = DependencyProperty.Register(
				"Author", 
				typeof(string),
				typeof(PostControl),
				new PropertyMetadata("Author", OnAuthorPropertyChanged));

		public string Author
		{
			get { return GetValue(AuthorProperty) as string; }
			set { SetValue(AuthorProperty, value); }
		}

		static void OnAuthorPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as PostControl).AuthorTextBlock.Text = args.NewValue as string;
		}
		#endregion

		#region Idx
		public static readonly DependencyProperty IdxProperty = DependencyProperty.Register(
				"Idx",
				typeof(int?),
				typeof(PostControl),
				new PropertyMetadata(0, OnIdxPropertyChanged));

		public int? Idx
		{
			get
			{
				return GetValue(IdxProperty) as int?;
			}
			set { SetValue(IdxProperty, value); }
		}

		static void OnIdxPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
		public static readonly DependencyProperty BodyProperty = DependencyProperty.Register(
				"Body",
				typeof(string),
				typeof(PostControl),
				new PropertyMetadata("Body", OnBodyPropertyChanged));

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

		private static void addTextBlock(StackPanel panel, TextBlock block)
		{
			if (block.Text.Length > 0)
				panel.Children.Add(block);
		}

		private static UIElement buildImage(string src)
		{
			if (!src.ToLower().EndsWith(".gif"))
			{
				Image ret = new Image();
				ret.HorizontalAlignment = HorizontalAlignment.Left;
				ret.Margin = new Thickness(3, 3, 3, 3);
				BitmapImage img = new BitmapImage(new Uri(IMG_PREFIX+src));
				ret.Source = img;
				ret.ImageOpened += (s, e) =>
				{
					/*
					ret.Width = img.PixelWidth;
					ret.Height = img.PixelHeight;
					 */
				};
				ret.ImageFailed += (s, e) =>
				{
					// corp proxy returns 302
				};
				return ret;
			}
			ImageTools.IO.Decoders.AddDecoder <ImageTools.IO.Gif.GifDecoder>();
			AnimatedImage gifRet = new AnimatedImage();
			gifRet.HorizontalAlignment = HorizontalAlignment.Left;
			gifRet.Margin = new Thickness(3, 3, 3, 3);
			ExtendedImage gifImg = new ExtendedImage();
			// fetch gif images directly
			gifImg.UriSource = new Uri(src);
			gifRet.Source = gifImg;
			gifRet.LoadingCompleted += (s, e) =>
			{
				/*
				gifRet.Width = gifImg.PixelWidth;
				gifRet.Height = gifImg.PixelHeight;
				 */
			};
			return gifRet;
		}

		static void OnBodyPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			PostControl s = obj as PostControl;
			s.ParseBody(args.NewValue as string);
		}

		private void ParseBody(string value)
		{
			this.BodyPanel.Children.Clear();
			StringReader reader = new StringReader(value);
			while (reader.Peek() > 0)
			{
				string line = reader.ReadLine();
				AddBlock(line);
			}
		}

		private void AddBlock(string line)
		{
			RichTextBox rtb = CreateRichTextBox();
			List<Inline> inlines = CreateInlines(line);
			Paragraph para = new Paragraph();
			foreach (var i in inlines)
				para.Inlines.Add(i);
			rtb.Blocks.Add(para);
			BodyPanel.Children.Add(rtb);
		}

		private List<Inline> CreateInlines(string line)
		{
			List<Inline> ret = new List<Inline>();
			MatchCollection matches = ICON_RE.Matches(line);
			int index = 0;
			foreach (Match m in matches)
			{
				if (!Icons.ContainsKey(m.Value))
					continue;
				if (index < m.Index)
					ret.Add(CreateRun(line.Substring(index, m.Index)));
				ret.Add(CreateEmoticon(Icons[m.Value]));
				index = m.Index + m.Length;
			}
			if (index != line.Length)
				ret.Add(CreateRun(line.Substring(index)));
			return ret;
		}

		private static RichTextBox CreateRichTextBox()
		{
			var app = Application.Current as App;
			RichTextBox ret = new RichTextBox();
			ret.TextWrapping = TextWrapping.Wrap;
			//ret.Style = app.Resources["PhoneTextNormalStyle"] as Style;
			ret.FontSize = (app.Resources["PhoneFontSizeMedium"] as double?).Value;
			ret.Margin = new Thickness();
			return ret;
		}

		private Run CreateRun(string text)
		{
			Run ret = new Run();
			ret.Text = text;
			return ret;
		}

		private InlineUIContainer CreateEmoticon(string fname)
		{
			ExtendedImage img = new ExtendedImage();
			Uri uri = new Uri(string.Format("Assets/Emoticons/{0}", fname), UriKind.Relative);
			StreamResourceInfo sri = Application.GetResourceStream(uri);
			img.SetSource(sri.Stream);

			AnimatedImage icon = new AnimatedImage();
			icon.Source = img;
			icon.Height = icon.Width = 20;
			icon.Stretch = Stretch.Uniform;

			InlineUIContainer ret = new InlineUIContainer();
			ret.Child = icon;
			return ret;
		}

		private static bool isPicture(string s)
		{
			return IMG_RE.IsMatch(s.ToLower());
		}

		private static bool isUrl(string s)
		{
			return URL_RE.IsMatch(s.ToLower());
		}

		#endregion

	}
}
