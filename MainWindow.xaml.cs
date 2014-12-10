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

namespace FuncDraw_WPF_
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    /// 

    public static class Draw
    {
        public static string code;
        public static string file_path = @"F:\School\Programing\C# Programing\FuncDraw\myfile.txt";
        public static Polyline[] line=new Polyline[100000];
        public static Color new_Color=new Color();
        public static void Initlizer_draw(int i)
        {
            Draw.line[i] = new Polyline();
            Draw.line[i].VerticalAlignment = VerticalAlignment.Center;
            Draw.line[i].HorizontalAlignment = HorizontalAlignment.Left;

            Draw.new_Color.A = 255;
            Draw.new_Color.R = 255;
            Draw.new_Color.G = 171;
            Draw.new_Color.B = 137;

            Draw.line[i].Stroke = new SolidColorBrush(new_Color);
            Draw.line[i].StrokeThickness = 2;
        }
        public static void draw(double x,double y)
        {
            Draw.line[(int)Program.scanner.Line_Count].Points.Add(new Point(x, y));
        }
    }

    public partial class MainWindow : Window
    {
        

        public MainWindow()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen; 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.grid_draw.Children.Clear();

            Program.scanner.Line_Count = 0;
           
                
            //Draw.line.Points.Add(new Point(1, 2));
            //Draw.line.Points.Add(new Point(50, 50));
            //Draw.line.Points.Add(new Point(100, 2));


            //Draw.Initlizer_draw(0);
            //for (int i = 100; i <= 300; i++)
            //    Draw.line[0].Points.Add(new Point(i, 100));
            //Draw.Initlizer_draw(1);
            //for (int i = 100; i >= -50; i--)
            //    Draw.line[1].Points.Add(new Point(100, i));


            //Draw.Initlizer_draw(2);
            //for (double i = 0; i <= 2 * System.Math.PI; i += System.Math.PI / 50)
            //    Draw.line[2].Points.Add(new Point(System.Math.Cos(i) * 30 + 100, System.Math.Sin(i) * 30 + 100));

            //this.grid_draw.Children.Add(Draw.line[0]);
            //this.grid_draw.Children.Add(Draw.line[1]);
            //this.grid_draw.Children.Add(Draw.line[2]);



            
            Program.parser.Parser(Draw.file_path);
            for (int i = 0; i < (int)Program.scanner.Line_Count; i++)
            {
                this.grid_draw.Children.Add(Draw.line[i]);
            }
            Comm.Text = Draw.code;
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            Window Cdwin = new Code();
            Cdwin.ShowDialog();
        }
    }
}
