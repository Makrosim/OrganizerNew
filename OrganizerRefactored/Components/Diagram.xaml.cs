using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OrganizerRefactored
{
    public partial class Diagram : Page
    {
        public ActionCommand cmd_Share { get; set; }
        IPlaylist Iplaylist;
        IIO IoVk;
        int CompAmount;
        float x = 160;
        float y = 160 - 20;
        float radius = 120;

        public Diagram(IPlaylist Iplay, IIO Iovk)
        {
            InitializeComponent();
            DataContext = this;
            cmd_Share = new ActionCommand(Share) { IsExecutable = true };
            Iplaylist = Iplay;
            IoVk = Iovk;
            Iplaylist.CollectionFilled += DrawDiagram;
            CompAmount = Iplaylist.GetAllCompositions().Count();
        }

        class Comparer : IComparer<KeyValuePair<string, long>>
        {
            public int Compare(KeyValuePair<string, long> one, KeyValuePair<string, long> two)
            {
                if (one.Value == two.Value)
                    return 0;

                if (one.Value < two.Value)
                    return -1;
                else
                    return 1;
            }
        }

        public struct DrawInfo
        {
            public string Genre;
            public double Ratio;

            public DrawInfo(string genre, double ratio)
            {
                Genre = genre;
                Ratio = ratio / 100;
            }
        }

        public struct Vector2
        {
            public float x, y;
            public Vector2(double X, double Y)
            {
                x = (float)X;
                y = (float)Y;
            }
        }

        private void DrawDiagram(object sender, EventArgs e)
        {
            var DrawInfo = CalculateRatio();
            PlaceOuterPentagon();
            PlaceInnerPentagon(DrawInfo);
            PlaceLines(DrawInfo);
            PlaceLabels(DrawInfo);
        }

        private void PlaceOuterPentagon()
        {
            var OuterRectVer = GetVertices(radius, x, y);
            var OuterPentagon = new Polygon();
            var myPointCollection = new PointCollection();

            OuterPentagon.Fill = Brushes.BurlyWood;
            OuterPentagon.Opacity = 0.2;


            foreach (var ver in OuterRectVer)
                myPointCollection.Add(new Point(ver.x, ver.y));

            OuterPentagon.Points = myPointCollection;
            cnvs_Diagram.Children.Add(OuterPentagon);
        }

        private void PlaceInnerPentagon(List<DrawInfo> info)
        {
            var InnerRectVer = GetVertices(radius, x, y, info);
            var InnerPentagon = new Polygon();
            var InnerPointColletion = new PointCollection();

            InnerPentagon.Stroke = Brushes.Gold;
            InnerPentagon.StrokeThickness = 2;
            InnerPentagon.Opacity = 0.5;
            InnerPentagon.Fill = Brushes.Goldenrod;

            foreach (var ver in InnerRectVer)
                InnerPointColletion.Add(new Point(ver.x, ver.y));

            InnerPentagon.Points = InnerPointColletion;
            cnvs_Diagram.Children.Add(InnerPentagon);
        }

        private void PlaceLines(List<DrawInfo> info)
        {
            var LinesVer = GetVertices(radius, x, y);
            foreach (var ver in LinesVer)
            {
                var myLine = new Line();
                myLine.Stroke = Brushes.Black;
                myLine.Opacity = 0.5;
                myLine.X1 = x;
                myLine.X2 = ver.x;
                myLine.Y1 = y;
                myLine.Y2 = ver.y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 2;
                cnvs_Diagram.Children.Add(myLine);
            }
        }

        private void PlaceLabels(List<DrawInfo> info)
        {
            var LabelVer = GetVertices(radius + 10, x, y);

            for (int i = 0; i < 5; i++)
            {
                var GenreText = new TextBlock();
                GenreText.Text = info[i].Genre;
                GenreText.Padding = new Thickness(0);
                GenreText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(GenreText, LabelVer[i].x - GenreText.DesiredSize.Width / 2);
                Canvas.SetTop(GenreText, LabelVer[i].y - GenreText.DesiredSize.Height / 2);
                cnvs_Diagram.Children.Add(GenreText);
            }
        }

        private List<DrawInfo> CalculateRatio()
        {
            var CompList = this.Iplaylist.GetAllCompositions();
            var genres = new Dictionary<string, long>();

            foreach (var comp in CompList)
            {
                if (genres.ContainsKey(comp.Genre))
                {
                    long count;
                    genres.TryGetValue(comp.Genre, out count);
                    count++;
                    genres.Remove(comp.Genre);
                    genres.Add(comp.Genre, count);
                }
                else
                if (!((chb_AllowOther.IsChecked == false) && (comp.Genre.Equals("Other"))) && !comp.Genre.Equals(""))
                    genres.Add(comp.Genre, 1);
            }

            var sorted = genres.OrderByDescending(el => el, new Comparer());
            var DrawInfo = new List<DrawInfo>();

            foreach (var el in sorted)
                DrawInfo.Add(new DrawInfo(el.Key, el.Value * 100 / sorted.First().Value));

            DrawInfo.RemoveRange(5, DrawInfo.Count() - 5);
            return DrawInfo;
        }

        private List<Vector2> GetVertices(float R, float x0, float y0)
        {
            var v = new List<Vector2>();
            for (int i = 0; i < 5; i++)
                v.Add(new Vector2(x0 - R * Math.Sin(2 * Math.PI * i / 5), y0 - R * (float)Math.Cos(2 * Math.PI * i / 5)));
            return v;
        }

        private List<Vector2> GetVertices(float R, float x0, float y0, List<DrawInfo> ratio)
        {
            var v = new List<Vector2>();
            for (int i = 0; i < 5; i++)
                v.Add(new Vector2(x0 - R * Math.Sin(2 * Math.PI * i / 5) * ratio[i].Ratio, y0 - R * (float)Math.Cos(2 * Math.PI * i / 5) * ratio[i].Ratio));
            return v;
        }

        private void OtherSwitch(object sender, RoutedEventArgs e)
        {
            cnvs_Diagram.Children.Clear();
            DrawDiagram(this, new EventArgs());
        }

        public void Share()
        {
            var fileResult = System.IO.Path.Combine(Environment.CurrentDirectory, "tmp.jpg");
            var path = new Uri(fileResult, UriKind.Absolute);
            ExportToPng(path, cnvs_Diagram);
            IoVk.SharePicture(fileResult);
        }

        private void ExportToPng(Uri path, Canvas canvas)
        {                        
            var size = new Size(canvas.ActualWidth, canvas.ActualHeight);                                    
            var renderBitmap = new RenderTargetBitmap((int)size.Width, (int)size.Height, 96d, 96d, PixelFormats.Pbgra32);

            renderBitmap.Render(canvas);
     
            using (var outStream = new FileStream(path.LocalPath, FileMode.Create))
            {               
                var encoder = new JpegBitmapEncoder();         
                encoder.Frames.Add(BitmapFrame.Create(renderBitmap));             
                encoder.Save(outStream);
            }                                 
        }

    }
}
