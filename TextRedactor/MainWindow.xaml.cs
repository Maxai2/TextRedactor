using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TextRedactor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        StringBuilder temp;

        System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();

        SaveFileDialog saveFileDialog;
        OpenFileDialog openFileDialog;

        bool firstSave = false;

        public MainWindow()
        {
            InitializeComponent();

            temp = new StringBuilder();

            saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.txt)|*.txt";

            openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt)|*.txt";

            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 10);
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            File.WriteAllText(tbPath.Text, tbUserText.Text);
            lAutoSave.Content = "Autosave: " + DateTime.Now.ToUniversalTime();
        }

        private void WrapPanel_Click(object sender, RoutedEventArgs e)
        {
            var controll = e.Source;

            if (controll as CheckBox == cbAutoSave)
            {
                if (firstSave && cbAutoSave.IsChecked == true)
                {
                    dispatcherTimer.Start();
                }
                else
                {
                    cbAutoSave.IsChecked = false;
                    dispatcherTimer.Stop();
                }

                return;
            }

            controll = e.Source as Button;

            if (controll == bOpen)
            {
                if (tbUserText.Text != "")
                {
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, tbUserText.Text);
                    }
                }

                if (openFileDialog.ShowDialog() == true)
                {
                    tbUserText.Text = File.ReadAllText(openFileDialog.FileName);

                    ToolTip toolTip = new ToolTip();

                    toolTip.Content = tbPath.Text = openFileDialog.FileName;

                    tbPath.ToolTip = toolTip;

                    firstSave = false;
                }
            }
            else
            if (controll == bSaveAs)
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveFileDialog.FileName, tbUserText.Text);

                    tbPath.Text = saveFileDialog.FileName;

                    firstSave = true;
                }
            }
            else
            if (controll == bCopy)
            {
                temp.Clear();
                temp.Append(tbUserText.SelectedText);
            }
            else
            if (controll == bPaste)
            {
                tbUserText.Text = tbUserText.Text.Insert(tbUserText.SelectionStart, temp.ToString());
            }
            else
            if (controll == bCut)
            {
                temp.Clear();
                temp.Append(tbUserText.SelectedText);

                tbUserText.Text = tbUserText.Text.Remove(tbUserText.SelectionStart, tbUserText.SelectionLength);
            }
            else
            if (controll == bSelectAll)
            {
                tbUserText.Focus();
                tbUserText.SelectAll();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tbUserText.Text == String.Empty)
                return;

            MessageBoxResult result = MessageBox.Show("Do you want to save file?", "Warning!", MessageBoxButton.YesNoCancel);

            switch (result)
            {
                case MessageBoxResult.None:
                case MessageBoxResult.Cancel:
                    e.Cancel = true;
                    break;
                case MessageBoxResult.Yes:
                    if (saveFileDialog.ShowDialog() == true)
                        File.WriteAllText(saveFileDialog.FileName, tbUserText.Text);
                    break;
                case MessageBoxResult.No:
                    break;
            }
        }
    }
}
