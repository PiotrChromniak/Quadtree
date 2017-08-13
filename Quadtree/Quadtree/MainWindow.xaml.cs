using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuadtreeVisualization
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Quadtree Root;

        public MainWindow()
        {
            InitializeComponent();
            Root = Quadtree.Root(new Region { X = 0, Y = 0, Width = canvas.Width });
        }

        private void Canvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var pos = Mouse.GetPosition(canvas);
            Root.Insert(pos);
            canvas.Children.Clear();
            DrawQuadtree();
        }

        private void DrawQuadtree()
        {
            if (Root.Type != Type.EMPTY)
            {
                var stack = new Stack<Quadtree>();
                var current = Root;
                
                while(true)
                {
                    if (current.Type == Type.NODE)
                    {
                        foreach (var quadtree in current.Children)
                            DrawRegion(quadtree.Region);

                        stack.Push(current.Children[0, 1]);
                        stack.Push(current.Children[1, 0]);
                        stack.Push(current.Children[1, 1]);
                        current = current.Children[0, 0];
                    }
                    else
                    {
                        if (current.Type == Type.PARTICLE)
                            DrawPoint(current.Point);
                        if (stack.Any())
                            current = stack.Pop();
                        else
                            break;
                    }
                }
            }
        }

        private void DrawRegion(Region reg)
        {
            var rect = new Rectangle()
            {
                Width = reg.Width + 1,
                Height = reg.Width + 1,
                Stroke = Brushes.Black,
                SnapsToDevicePixels = true,
                StrokeThickness = 1
            };

            //rect.SetValue(RenderOptions.EdgeModeProperty, EdgeMode.Aliased);
            rect.SetValue(Canvas.LeftProperty, reg.X);
            rect.SetValue(Canvas.TopProperty, reg.Y);
            canvas.Children.Add(rect);
        }

        private void DrawPoint(Point point)
        {
            var ellipse = new Ellipse()
            {
                Width = 5,
                Height = 5,
                Fill = Brushes.Green
            };

            ellipse.SetValue(Canvas.LeftProperty, point.X - 2.5);
            ellipse.SetValue(Canvas.TopProperty, point.Y - 2.5);
            canvas.Children.Add(ellipse);
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Root = Quadtree.Root(new Region { X = 0, Y = 0, Width = canvas.Width });
            canvas.Children.Clear();
        }
    }
}
