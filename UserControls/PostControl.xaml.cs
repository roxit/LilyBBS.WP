using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using LilyBBS.API;

namespace LilyBBS
{
	public partial class PostControl : UserControl
	{
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

		public static readonly DependencyProperty BodyProperty = DependencyProperty.Register("Body",
				typeof(string),
				typeof(PostControl),
				new PropertyMetadata(OnBodyChanged));

		public string Body
		{
			get { return GetValue(BodyProperty) as string; }
			set { SetValue(BodyProperty, value); }
		}

		static void OnBodyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
		{
			(obj as PostControl).BodyTextBlock.Text = args.NewValue as string;
		}

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

		public string Title { get; set; }
		public int Num { get; set; }
		public PostControl()
		{
			InitializeComponent();
		}

		/*
		public void LoadData(Post post)
		{
			Author.Text = post.Author;
			Body.Text = post.Body;
			Title = post.Title;
			Num = post.Num;
		}
		 */
	}
}
