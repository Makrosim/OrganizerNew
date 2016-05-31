using System;
using System.Collections.Generic;
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

    public partial class Diagram : Page
    {
        List<Composition> CompList;
        IPlaylist Iplaylist;
        Dictionary<string, long> genres;
        int CompAmount;
        float x = 160;
        float y = 160 - 20;
        float radius = 120;

        public Diagram(IPlaylist Iplay)
        {
            InitializeComponent();
            Iplaylist = Iplay;
            Iplaylist.CollectionFilled += DrawDiagram;
            CompAmount = Iplaylist.GetAllCompositions().Count();
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
            CompList = this.Iplaylist.GetAllCompositions();
            genres = new Dictionary<string, long>();

            foreach (var comp in CompList)
            {
                if(genres.ContainsKey(comp.Genre))
                {
                    long count;
                    genres.TryGetValue(comp.Genre, out count);
                    count++;
                    genres.Remove(comp.Genre);
                    genres.Add(comp.Genre, count);
                }
                else
                if(!((chb_AllowOther.IsChecked == false)&&(comp.Genre.Equals("Other"))) && !comp.Genre.Equals(""))
                    genres.Add(comp.Genre, 1);
            }

            var sorted = genres.OrderByDescending(el => el, new Comparer());
            var DrawInfo = new List<DrawInfo>();

            foreach (var el in sorted)
                DrawInfo.Add(new DrawInfo(el.Key, el.Value * 100 / sorted.First().Value));

            DrawInfo.RemoveRange(5, DrawInfo.Count() - 5);

            var OuterRectVer = GetVertices(radius, x, y);
            var LabelVer = GetVertices(radius + 10, x, y);

            for (int i = 0; i < 5; i++)
            {
                var GenreText = new TextBlock();
                GenreText.Text = DrawInfo[i].Genre;
                GenreText.Padding = new Thickness(0);
                GenreText.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Canvas.SetLeft(GenreText, LabelVer[i].x - GenreText.DesiredSize.Width / 2);
                Canvas.SetTop(GenreText, LabelVer[i].y - GenreText.DesiredSize.Height / 2);
                cnvs_Diagram.Children.Add(GenreText);
            }

            var OuterPentagon = new Polygon();
            OuterPentagon.Fill = System.Windows.Media.Brushes.BurlyWood;
            OuterPentagon.Opacity = 0.2;
            PointCollection myPointCollection = new PointCollection();

            foreach (var ver in OuterRectVer)
            {
                myPointCollection.Add(new System.Windows.Point(ver.x, ver.y));

            }

            OuterPentagon.Points = myPointCollection;
            cnvs_Diagram.Children.Add(OuterPentagon);

            foreach (var ver in OuterRectVer)
            {
                var myLine = new Line();
                myLine.Stroke = System.Windows.Media.Brushes.Black;
                myLine.Opacity = 0.5;
                myLine.X1 = 160;
                myLine.X2 = ver.x;
                myLine.Y1 = 160;
                myLine.Y2 = ver.y;
                myLine.HorizontalAlignment = HorizontalAlignment.Left;
                myLine.VerticalAlignment = VerticalAlignment.Center;
                myLine.StrokeThickness = 2;
                cnvs_Diagram.Children.Add(myLine);
            }

            var InnerRectVer = GetVertices(120, 160, 160, DrawInfo);

            var InnerPentagon = new Polygon();
            Brush str = Brushes.Gold;
            InnerPentagon.Stroke = str;
            InnerPentagon.StrokeThickness = 2;
            InnerPentagon.Opacity = 0.5;
            LinearGradientBrush pent = new LinearGradientBrush();
            InnerPentagon.Fill = Brushes.Goldenrod;
            PointCollection InnerPointColletion = new PointCollection();

            foreach (var ver in InnerRectVer)
            {
                InnerPointColletion.Add(new System.Windows.Point(ver.x, ver.y));
            }

            InnerPentagon.Points = InnerPointColletion;
            cnvs_Diagram.Children.Add(InnerPentagon);
        }

        /// <summary>
        /// ...
        /// </summary>
        /// <param name="R">Радиус</param>
        /// <param name="x0">X центра</param>
        /// <param name="y0">Y центра</param>
        /// <returns>Список вершин</returns>
        private List<Vector2> GetVertices(float R, float x0, float y0)
        {
            List<Vector2> v = new List<Vector2>();
            for (int i = 0; i < 5; i++)
                v.Add(new Vector2(x0 - R * Math.Sin(2 * Math.PI * i / 5), y0 - R * (float)Math.Cos(2 * Math.PI * i / 5)));
            return v;
        }

        private List<Vector2> GetVertices(float R, float x0, float y0, List<DrawInfo> ratio)
        {
            List<Vector2> v = new List<Vector2>();
            for (int i = 0; i < 5; i++)
                v.Add(new Vector2(x0 - R * Math.Sin(2 * Math.PI * i / 5) * ratio[i].Ratio, y0 - R * (float)Math.Cos(2 * Math.PI * i / 5) * ratio[i].Ratio));
            return v;
        }

        private void OtherSwitch(object sender, RoutedEventArgs e)
        {
            cnvs_Diagram.Children.Clear();
            DrawDiagram(this, new EventArgs());
        }
    }
}
