using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NodeMCU_Studio_2015
{
    /// <summary>
    /// Interaction logic for FileExplorer.xaml
    /// </summary>
    public partial class FileExplorer : Window
    {
        public FileExplorer() {
            InitializeComponent();
        }

        private void load_button_Click( Object sender , RoutedEventArgs e ) {
            var last_value = this.load_button.IsEnabled;
            Task.Factory.StartNew( () => {
                this.listBox.Dispatcher.Invoke( new Action( () => {
                    this.load_button.IsEnabled = !last_value;
                } ) );
                var path = Workspace.Instance.LuaDeivceProgramsDirectory;
                if ( Directory.Exists( path ) ) {
                    Directory.Delete( path , true );
                    while ( Directory.Exists( path ) ) ;
                }
                Directory.CreateDirectory( path );
                while ( !Directory.Exists( path ) ) ;
                var list = Workspace.Instance.Load();
                this.listBox.Dispatcher.Invoke( new Action( () => {
                    this.listBox.Items.Clear();
                    foreach ( var f in list ) {
                        this.listBox.Items.Add( f );
                    }
                    this.load_button.IsEnabled = last_value;
                } ) );
            } );
        }

        private void listBox_SelectionChanged( Object sender , SelectionChangedEventArgs e ) {
            var f = this.listBox.SelectedItem as string;
            var path = Workspace.Instance.LuaDeivceProgramsDirectory;
            if ( string.IsNullOrWhiteSpace( f ) ) {
                this.delete_button.IsEnabled = false;
            } else {
                if ( !File.Exists( path + f ) ) {
                    var context = Workspace.Instance.Load( f );
                    FileStream file = new FileStream( path + f , FileMode.Create );
                    StreamWriter writer = new StreamWriter( file );
                    writer.Write( context );
                    writer.Flush();
                    file.Close();
                }
                this.delete_button.IsEnabled = true;
            }
        }

        private void delete_button_Click( Object sender , RoutedEventArgs e ) {
            var last_value = this.delete_button.IsEnabled;
            this.listBox.Dispatcher.Invoke( new Action( () => {
                this.delete_button.IsEnabled = !last_value;
            } ) );

            var file = this.listBox.SelectedItem as string;
            if ( !string.IsNullOrWhiteSpace( file ) ) {
                Workspace.Instance.Delete( file );
                this.listBox.Items.Remove( file );
            }
            this.delete_button.IsEnabled = last_value;
        }

        private void ok_button_Click( Object sender , RoutedEventArgs e ) {
            this.DialogResult = true;
        }

        private void CopyAll_Click( Object sender , RoutedEventArgs e ) {

            Task.Factory.StartNew( () => {
                this.listBox.Dispatcher.Invoke( new Action( () => {
                    this.CopyAll.IsEnabled = false;
                } ) );
                var list = this.listBox.Items.Cast<string>();
                var path = Workspace.Instance.LuaDeivceProgramsDirectory;
                foreach ( var f in list ) {
                    if ( !File.Exists( path + f ) ) {
                        var context = Workspace.Instance.Load( f );
                        FileStream file = new FileStream( path + f , FileMode.Create );
                        StreamWriter writer = new StreamWriter( file );
                        writer.Write( context );
                        writer.Flush();
                        file.Close();
                    }
                    if ( !File.Exists( Workspace.Instance.LuaProgramsDirectory + f ) ) {
                        File.Copy( path + f , Workspace.Instance.LuaProgramsDirectory + f , false );
                    }
                }
                this.listBox.Dispatcher.Invoke( new Action( () => {
                    this.CopyAll.IsEnabled = true;
                    Workspace.Instance.RefreshWorkspace();
                } ) );
            } );
        }
    }
}
