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
using System.Windows.Threading;

namespace NodeMCU_Studio_2015
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow() {
            InitializeComponent();
        }

        public bool CommandMode {
            get { return ( bool ) GetValue( CommandModeProperty ); }
            set { SetValue( CommandModeProperty , value ); }
        }

        public static readonly DependencyProperty CommandModeProperty =
            DependencyProperty.Register( nameof(CommandMode) , typeof(bool) , typeof(MainWindow) , new PropertyMetadata( false, OnChanged ) );

        private static void OnChanged( DependencyObject d , DependencyPropertyChangedEventArgs e ) {
            var main = d as MainWindow;
            var command_mode = ( bool ) e.NewValue;
            if ( command_mode ) {

            }
        }

        private void new_lua_Click( Object sender , RoutedEventArgs e ) {
            NewLua newlua = new NewLua();
            newlua.ShowInTaskbar = false;
            newlua.Owner = this;
            var ret = newlua.ShowDialog();
            if ( ret.HasValue && ret.Value == true ) {

            }
        }


        private void Window_Loaded( Object sender , RoutedEventArgs e ) {
            Workspace.Instance.IsValidChanged += ( se , args ) => {
                this.download_button.IsEnabled = Workspace.Instance.IsValid;
                this.exe_button.IsEnabled = Workspace.Instance.IsValid;
                this.explorer_button.IsEnabled = Workspace.Instance.IsValid;
            };
            Workspace.Instance.OnDataReceived += ( se2 , txt ) => {
                this.output.Dispatcher.Invoke( new Action( () => {
                    this.output.Text = this.output.Text + txt.Text;
                } ) );
            };

            Workspace.Instance.OnTimeout += ( se3 , arg1 ) => {
                this.Dispatcher.Invoke( new Action(()=>{
                    MessageBox.Show( "COM Port TimeoutException Occurred, \nPlease Check The Com Port And Restart this Application" , "Error" , MessageBoxButton.OK , MessageBoxImage.Error );
                }) );
            };

            SerialSetting setting = new SerialSetting();
            setting.Owner = this;
            var ret = setting.ShowDialog();
            
            if ( ret.HasValue && ret == true ) {


                Binding bind = new Binding( "ProjectList" );
                bind.Source = Workspace.Instance;
                bind.Mode = BindingMode.OneWay;
                this.lua_programs_list.SetBinding( ListBox.ItemsSourceProperty , bind );
            } else {
                this.Close();
            }
        }

        private void lua_programs_list_SelectionChanged( Object sender , SelectionChangedEventArgs e ) {
            if ( e.RemovedItems.Count > 0 ) {
                foreach ( var item in e.RemovedItems ) {
                    var proj = item as LuaProject;
                    proj?.Save(this.codeedit.Text);
                }
            }
            var listbox = sender as ListBox;
            var project = listbox?.SelectedItem as LuaProject;
            this.codeedit.Text = project?.SourceCode;
            this.output.Text = "";
        }

        private void download_button_Click( Object sender , RoutedEventArgs e ) {
           
            var listbox = this.lua_programs_list;
            var project = listbox?.SelectedItem as LuaProject;
            if ( project != null ) {
                project.Save(this.codeedit.Text);
                Workspace.Instance.Write( project, ()=> { this.DoEvent();  } );
            }
        }

        public void DoEvent() {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke( DispatcherPriority.Background , new DispatcherOperationCallback( ExitFrame ) , frame );
            Dispatcher.PushFrame( frame );
        }
        public object ExitFrame( object f ) {
            ( ( DispatcherFrame ) f ).Continue = false;
            return null;
        }

        private void Window_Closed( Object sender , EventArgs e ) {
            Workspace.Instance.Close();
        }

        private void exe_button_Click( Object sender , RoutedEventArgs e ) {
            var listbox = this.lua_programs_list;
            var project = listbox?.SelectedItem as LuaProject;
            if ( project != null ) {
                Workspace.Instance.Execute( project );
            }
        }

        private void close_menu_Click( Object sender , RoutedEventArgs e ) {
            this.Close();
        }

        private void explorer_button_Click( Object sender , RoutedEventArgs e ) {
            FileExplorer exp = new FileExplorer();
            exp.Owner = this;
            exp.ShowInTaskbar = false;
            exp.ShowDialog();
        }

        private void codeedit_KeyUp( Object sender , KeyEventArgs e ) {
            if ( e.Key == Key.Enter && this.CommandMode == true ) {
                string v = this.codeedit.Text;
                string[] line = v.Split( new string[] { "\n" , "\r\n" , "\r" } , StringSplitOptions.None );
                if ( line.Length > 1 ) {
                    string lst_line = line[line.Length - 2];
                    if ( !string.IsNullOrWhiteSpace( lst_line ) ) {
                        Workspace.Instance.Write( lst_line );
                    }
                }
            }
        }
    }
}
