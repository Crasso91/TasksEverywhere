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
using WpfUtils;

namespace ICeQuartScheduler.Manager.UserControls
{
    /// <summary>
    /// Interaction logic for FileSelector.xaml
    /// </summary>
   
    public enum SelectorType
    {
        File,
        Folder
    }

    public partial class FileSelector : UserControl
    {
        public Action<string> OnSelection { get; set; }
        public string Path { get { return txtFilePath.Text; } }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Label", typeof(string), typeof(FileSelector), new PropertyMetadata(string.Empty, ValueChanged));
        public static readonly DependencyProperty SelectorTypeProperty = DependencyProperty.Register("SelectorType", typeof(SelectorType), typeof(FileSelector), new PropertyMetadata(SelectorType.File, SelectorTypeValueChanged));

        private static void ValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileSelector;
            control.Label = (string)e.NewValue;
        }
        private static void SelectorTypeValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as FileSelector;
            control.SelectorType = (SelectorType)e.NewValue;
        }

        public string Label
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public SelectorType SelectorType
        {
            get { return (SelectorType)GetValue(SelectorTypeProperty); }
            set { SetValue(SelectorTypeProperty, value); }
        }
        public FileSelector()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void BtnSfoglia_MouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (SelectorType)
            {
                case SelectorType.File:
                    DialogUtils.File(x => {
                        if (!string.IsNullOrEmpty(x))
                        {
                            txtFilePath.Text = x;
                            OnSelection?.Invoke(x);
                        }
                    });
                    break;
                case SelectorType.Folder:
                    DialogUtils.Folder(x => {
                        if (!string.IsNullOrEmpty(x))
                        {
                            txtFilePath.Text = x;
                            OnSelection?.Invoke(x);
                        }
                    });
                    break;
                default:
                    break;
            }
        }
    }
}
