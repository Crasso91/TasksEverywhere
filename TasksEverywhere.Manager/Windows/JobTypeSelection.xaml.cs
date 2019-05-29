using TasksEverywhere.Extensions;
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
using static ICeQuartScheduler.Manager.Enumerators.Enumerators;

namespace ICeQuartScheduler.Manager.Windows
{
    /// <summary>
    /// Interaction logic for JobTypeSelection.xaml
    /// </summary>
    public partial class JobTypeSelection : Window
    {
        public Action<JobType, string, string> OnConfirm;
        public JobTypeSelection(Action<JobType, string, string> onConfirm = null)
        {
            OnConfirm = onConfirm;
            InitializeComponent();
            cbJobType.ItemsSource = Enum.GetValues(typeof(JobType));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            var selectedType = cbJobType.SelectedValue.ToEnum<JobType>();
            OnConfirm(selectedType, txtName.Text, txtGroup.Text);
            this.Close();
        }
    }
}
