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
using System.Windows.Shapes;
using System.IO;

namespace NodeMCU_Studio_2015
{
    /// <summary>
    /// Interaction logic for NewLua.xaml
    /// </summary>
    public partial class NewLua : Window
    {
        public NewLua() {
            InitializeComponent();
        }

        private void ok_button_Click( Object sender , RoutedEventArgs e ) {
            var path = System.IO.Path.GetFullPath( Workspace.Instance.LuaProgramsDirectory );
            var des_path1 = System.IO.Path.Combine( path , this.filename_textBox.Text );
            var temp = System.IO.Path.GetFileNameWithoutExtension( des_path1 );
            if ( string.IsNullOrWhiteSpace( temp ) ) {
                temp = "unnamed";
            }
            var des_path = System.IO.Path.Combine(path, temp) + ".lua";
            if ( File.Exists( des_path ) ) {
                MessageBox.Show( "Programs exist! Please select a new name..." , "File exist..." , MessageBoxButton.OK , MessageBoxImage.Question );
                return;
            } else {
                LuaProject project = new LuaProject();
                project.File = des_path;
                project.UnSaved = false;
                project.Save( "" );
                Workspace.Instance.Add( project );
                this.DialogResult = true;
            }
        }

        private void cancel_button_Click( Object sender , RoutedEventArgs e ) {
            this.DialogResult = false;
        }
    }
}
