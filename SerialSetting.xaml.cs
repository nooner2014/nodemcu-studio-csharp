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

namespace NodeMCU_Studio_2015
{
    /// <summary>
    /// Interaction logic for SerialSetting.xaml
    /// </summary>
    public partial class SerialSetting : Window
    {
        public SerialSetting() {
            InitializeComponent();
        }

        private void Window_Loaded( Object sender , RoutedEventArgs e ) {
            foreach(var sp in Workspace.Instance.SerialPorts ) {
                this.serial_comboBox.Items.Add( sp );
            }
            this.serial_comboBox.SelectionChanged -= this.serial_comboBox_SelectionChanged;
            if ( this.serial_comboBox.Items.Count > 0 ) {
                this.serial_comboBox.SelectedIndex = 0;
            }

            this.serial_comboBox.SelectionChanged += this.serial_comboBox_SelectionChanged;
        }

        private void serial_comboBox_SelectionChanged( Object sender , SelectionChangedEventArgs e ) {
           
            
        }

        private void ok_button_Click( Object sender , RoutedEventArgs e ) {
            var combox = this.serial_comboBox as ComboBox;
            var item = combox?.SelectedItem as string;
            if ( !string.IsNullOrWhiteSpace( item ) ) {
                try {
                    Workspace.Instance.Open( item );
                    this.DialogResult = true;
                } catch {
                    MessageBox.Show( @"Can not open or validate the com port" , "error" , MessageBoxButton.OK , MessageBoxImage.Error );
                }
            }
        }

        private void exit_button_Click( Object sender , RoutedEventArgs e ) {
            this.DialogResult = false;
        }
    }
}
