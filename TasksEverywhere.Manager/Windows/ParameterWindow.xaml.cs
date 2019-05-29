using TasksEverywhere.Extensions;
using TasksEverywhere.Extensions.Extensions;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Context.Models;
using TasksEverywhere.Utilities.Enums;
using Newtonsoft.Json.Linq;
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

namespace ICeQuartScheduler.Manager.Windows
{
    /// <summary>
    /// Interaction logic for ParameterWindow.xaml
    /// </summary>
    public partial class ParameterWindow : Window
    {
        private TextBox txtboxLorOrPrograms;
        public Action<IJobParameter> OnSave;
        private IJobParameter param;
        private bool IsNew;

        public ParameterWindow(IJobParameter _param, bool isNew, Action<IJobParameter> onSave = null)
        {
            InitializeComponent();
            OnSave = onSave;
            IsNew = isNew;
            param = _param;
            cbParamType.ItemsSource = Enum.GetValues(typeof(JobDataMapKeys));
            if (!isNew)
            {
                cbParamType.SelectedValue = _param.Key;
                switch (_param.Key)
                {
                    case JobDataMapKeys.Programs:
                        InitPrograms();
                        break;
                    case JobDataMapKeys.LogLevel:
                        InitLog();
                        break;
                    case JobDataMapKeys.ReflectionJobData:
                        InitReflected();
                        break;
                    default:
                        break;
                }
            }
        }

        private void InitReflected()
        {
            var dg = new DataGrid();
            dg.Uid = "dgDynamic";
            dg.AutoGenerateColumns = false;
            dg.IsReadOnly = false;
            dg.Columns.Add(new DataGridTextColumn { Binding = new Binding("LibraryPath"), Header = "Path Libreria" });
            dg.Columns.Add(new DataGridTextColumn { Binding = new Binding("ClassName"), Header = "Nome Classe" });
            dg.Columns.Add(new DataGridTextColumn { Binding = new Binding("ConstructorArgs"), Header = "Argomenti Costruttore" });
            dg.Columns.Add(new DataGridTextColumn { Binding = new Binding("MethodName"), Header = "Nome Metodo" });
            dg.Columns.Add(new DataGridTextColumn { Binding = new Binding("MethodArgs"), Header = "Argomenti Metodo" });
            dg.CanUserAddRows = true;
            if (!IsNew && param.Value != null)
            {
                var reflectedJobDataList = (param.Value as List<ReflectionJobData>);
                dg.ItemsSource = reflectedJobDataList;
            }
            else
            {
                dg.ItemsSource = new List<ReflectionJobData>();
            }
            stackpanel.Children.Add(dg);
        }

        private void InitLog()
        {
            var txtBox = new TextBox();
            txtBox.Text = "Valore del log tra: Info, Error, Debug";
            txtBox.Uid = "LogValue";
            if (!IsNew && param.Value != null)
            {
                txtBox.Text = param.Value.ToString();
            }
            txtboxLorOrPrograms = txtBox;
            stackpanel.Children.Add(txtBox);
        }

        private void InitPrograms()
        {
            var txtBox = new TextBox();
            txtBox.Uid = "ProgramsValue";
            txtBox.Text = "lista dei path dei programmi";
            if (!IsNew && param.Value != null)
            {
                var list = (param.Value as JArray).ToObject<List<string>>();
                txtBox.Text = string.Join(",", list);
            }
            txtboxLorOrPrograms = txtBox;
            stackpanel.Children.Add(txtBox);
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            param.Key = cbParamType.SelectedValue.ToEnum<JobDataMapKeys>();
            switch (param.Key)
            {
                case JobDataMapKeys.Programs:
                    SaveProgramsData();
                    break;
                case JobDataMapKeys.LogLevel:
                    SaveLogData();
                    break;
                case JobDataMapKeys.ReflectionJobData:
                    SaveReflectedData();
                    break;
                default:
                    break;
            }

            OnSave(param);
            this.Close();
        }

        private void SaveProgramsData()
        {
            foreach(UIElement ctrl in stackpanel.Children)
            {
                if(ctrl.Uid == "ProgramsValue")
                {
                    param.Value = ((TextBox)ctrl).Text;
                }
            }
        }

        private void SaveLogData()
        {
            foreach (UIElement ctrl in stackpanel.Children)
            {
                if (ctrl.Uid == "LogValue")
                {
                    param.Value = ((TextBox)ctrl).Text;
                }
            }
        }

        private void SaveReflectedData()
        {
            foreach(UIElement ctrl in stackpanel.Children)
            {
                if(ctrl.Uid == "dgDynamic")
                {
                    DataGrid dg = (DataGrid)ctrl;
                    param.Value = dg.Items.SourceCollection.Cast<ReflectionJobData>();
                }
            }
        }

        private void BtnAbort_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CbParamType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((ComboBox)sender).SelectedValue == null) return;
            stackpanel.Children.Clear();
            var paramType = ((ComboBox)sender).SelectedValue.ToEnum<JobDataMapKeys>();
            switch (paramType)
            {
                case JobDataMapKeys.Programs:
                    InitPrograms();
                    break;
                case JobDataMapKeys.LogLevel:
                    InitLog();
                    break;
                case JobDataMapKeys.ReflectionJobData:
                    InitReflected();
                    break;
                default:
                    break;
            }
        }
    }
}
