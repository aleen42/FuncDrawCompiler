using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace FuncDraw_WPF_
{
    /// <summary>
    /// Code.xaml 的交互逻辑
    /// </summary>
    public partial class Code : Window
    {
        public Code()
        {
            InitializeComponent();
            WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Draw.code = this.Code_text.Text;
            File.WriteAllText(Draw.file_path, "");
            FileStream outfile = new FileStream(Draw.file_path, FileMode.Open, FileAccess.Write);   
            StreamWriter WriteMyfile = new StreamWriter(outfile);
            WriteMyfile.WriteLine(Draw.code);
            WriteMyfile.Flush();
            outfile.Close();
            this.Close();
        }
    }
}
