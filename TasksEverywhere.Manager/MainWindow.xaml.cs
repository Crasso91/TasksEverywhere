using Configuration;
using ICeQuartScheduler.Manager.ModelConfig;
using ICeQuartScheduler.Manager.Windows;
using TasksEverywhere.CastleWindsor.Service.Concrete;
using TasksEverywhere.Quartz.Context.Concrete;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Quartz.Context.Jobs.Concrete;
using TasksEverywhere.Quartz.Jobs.Concrete;
using TasksEverywhere.Quartz.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static ICeQuartScheduler.Manager.Enumerators.Enumerators;

namespace ICeQuartScheduler.Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }
        public ServiceController windService { get; set; }
        private JsonConfiguration config;
        public ObservableCollection<ICustomJob> Jobs { get; set; }
        public bool IsNew { get; private set; }

        private ICustomJob editjob;
        
        public MainWindow()
        {
            InitializeComponent();
            InitServiceStatus();
            InitConfig();
            if (config != null)
            {
                InitJobList();
                DataContext = this;
            }
        }

        private void InitJobList()
        {
            var connection = new JsonConnection(config.QuartzConfigPath);
            QuartzJsonContext.Instance.Connection = connection;
            QuartzJsonContext.Instance.Init();
            Jobs = new ObservableCollection<ICustomJob>();
            QuartzJsonContext.Instance.data.Jobs.ForEach(x => Jobs.Add(x));
        }

        private void InitServiceStatus()
        {
            windService = new ServiceController("IceQuartzScheduler");
            try
            {
                switch (windService.Status)
                {
                    case ServiceControllerStatus.Paused:
                        icServiceStopped.Visibility = Visibility.Visible;
                        icServiceRunnig.Visibility = Visibility.Collapsed;
                        icServiceNotFound.Visibility = Visibility.Collapsed;
                        icLoader.Visibility = Visibility.Collapsed;
                        break;
                    case ServiceControllerStatus.Running:
                        icServiceStopped.Visibility = Visibility.Collapsed;
                        icServiceRunnig.Visibility = Visibility.Visible;
                        icServiceNotFound.Visibility = Visibility.Collapsed;
                        icLoader.Visibility = Visibility.Collapsed;
                        break;
                    case ServiceControllerStatus.Stopped:
                        icServiceStopped.Visibility = Visibility.Visible;
                        icServiceRunnig.Visibility = Visibility.Collapsed;
                        icServiceNotFound.Visibility = Visibility.Collapsed;
                        icLoader.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        icLoader.Visibility = Visibility.Visible;
                        break;
                }
                //this.ucServiceStatus.Text = ServiceStatus;
            }
            catch (Exception)
            {
                icServiceStopped.Visibility = Visibility.Collapsed;
                icServiceRunnig.Visibility = Visibility.Collapsed;
                icServiceNotFound.Visibility = Visibility.Visible;
                var wantInstall = false;
                WpfUtils.DialogUtils.Alert("Il servizio ICeQuartzScheduler non risulta installato, procedo all'installazione?", () => {
                    wantInstall = true;
                }, () => { });
                if (wantInstall)
                {
                    var instellerPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "IceQuartzServiceInstaller", "ICeQuartzSchedulerServiceWizard.msi");
                    var p = System.Diagnostics.Process.Start(instellerPath);
                    p.WaitForExit();
                }

            }
            
        }

        private void InitConfig()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = System.IO.Path.Combine(path, "config", "AppConfig.json");
            if (!System.IO.File.Exists(configPath))
                WpfUtils.DialogUtils.Alert("Configurare il path del file .json");
            else
            {
                config = ExternalConfig.GetConfig<JsonConfiguration>(configPath);
                if (string.IsNullOrEmpty(config.QuartzConfigPath) || !System.IO.File.Exists(config.QuartzConfigPath))
                {
                    WpfUtils.DialogUtils.Alert("Configurare il path del file .json");
                    config = null;
                }
            }
            
        }

        private void BtnAddJob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var jobTypeWindow = new JobTypeSelection();
            jobTypeWindow.Owner = this;
            jobTypeWindow.OnConfirm = (jobType, jobName, jobGroup) => {
                switch (jobType)
                {
                    case JobType.CustomJob:
                        editjob = new CustomJob()
                        {
                            Name = jobName,
                            Group = jobGroup
                        };
                        break;
                    case JobType.ReflectionJob:
                        editjob = new ReflectionJob()
                        {
                            Name = jobName,
                            Group = jobGroup
                        };
                        break;
                    default:
                        break;
                }
                QuartzJsonContext.Instance.Add(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
                RefreshJobList();
                //dgParams.ItemsSource = editjob.Parameters;
                EditJob();
            };
            jobTypeWindow.ShowDialog();
        }

        private void BtnEditJob_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //editjob = (ICustomJob)dgJobs.SelectedItem;
            //EditJob();
        }

        private void EditJob()
        {
            //txtName.IsEnabled = true;
            //txtGroup.IsEnabled = true;
            txtDescription.IsEnabled = true;
            chkActive.IsEnabled = true;
            txtName.Text = editjob.Name;
            txtGroup.Text = editjob.Group;
            txtDescription.Text = editjob.Description;
            chkActive.IsChecked = editjob.Active;
            dgParams.ItemsSource = editjob.Parameters;
            dgTriggers.ItemsSource = editjob.Triggers;
        }

        private void DgJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (((DataGrid)sender).SelectedItem == null) return;
            MakeConfigBakup();
            editjob = ((DataGrid)sender).SelectedItem as ICustomJob;
            EditJob();
        }

        private void MakeConfigBakup()
        {
            var backupNameTemplate = "{0}_{1}_{2}.bkp";
            var backupName = string.Format(backupNameTemplate, config.QuartzConfigPath, Process.GetCurrentProcess().Id, DateTime.Now.ToString("ddMMyyyy"));
            var directory = System.IO.Path.GetDirectoryName(config.QuartzConfigPath);
            var backupPath = System.IO.Path.Combine(directory, backupName);
            if (!System.IO.File.Exists(backupPath)) {
                WpfUtils.DialogUtils.Alert("Eseguire un backup prima delle eventuali modifiche?", () =>
                {
                    System.IO.File.Copy(config.QuartzConfigPath, backupPath);
                }, () => { }, null);
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void BtnAddTrigger_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (editjob == null) return;
            var trigger = new CustomTrigger();
            var triggerWindow = new TriggerWindow(trigger, true);
            triggerWindow.Owner = this;
            triggerWindow.OnSave = (trg) =>
            {
                editjob.Triggers.Add(trg);
                dgTriggers.ItemsSource = null;
                dgTriggers.ItemsSource = editjob.Triggers;
                QuartzJsonContext.Instance.Update(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
            };
            triggerWindow.ShowDialog();
        }

        private void BtnAddParam_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (editjob == null) return;
            var param = new JobParameter();
            var paramWindow = new ParameterWindow(param, true);
            paramWindow.Owner = this;
            paramWindow.OnSave = (prm) =>
            {
                editjob.Parameters.Add(prm);
                dgParams.ItemsSource = null;
                dgParams.ItemsSource = editjob.Parameters;
                QuartzJsonContext.Instance.Update(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
            };
            paramWindow.ShowDialog();
        }

        private void BtnEditParam_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (editjob == null) return;
            var param = ((FrameworkElement)sender).DataContext as IJobParameter;
            var parameterWindow = new ParameterWindow(param, false);
            parameterWindow.Owner = this;
            parameterWindow.OnSave = (prm) =>
            {
                var _param = editjob.Parameters.FirstOrDefault(x => x.Key == prm.Key);
                editjob.Parameters.Remove(_param);
                editjob.Parameters.Add(prm);
                dgParams.ItemsSource = null;
                dgParams.ItemsSource = editjob.Parameters;
                QuartzJsonContext.Instance.Update(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
            };
            parameterWindow.ShowDialog();
        }

        private void BtnEditTrigger_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (editjob == null) return;
            var trigger = ((FrameworkElement)sender).DataContext as ICustomTrigger;
            var triggerWindow = new TriggerWindow(trigger, false);
            triggerWindow.Owner = this;
            triggerWindow.OnSave = (trg) => {
                var _trg = editjob.Triggers.FirstOrDefault(x => x.Name == trg.Name && x.Group == trg.Group);
                editjob.Triggers.Remove(_trg);
                editjob.Triggers.Add(trg);
                dgTriggers.ItemsSource = null;
                dgTriggers.ItemsSource = editjob.Triggers;
                QuartzJsonContext.Instance.Update(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
            };
            triggerWindow.ShowDialog();
        }

        private void RefreshJobList()
        {
            Jobs.Clear();
            QuartzJsonContext.Instance.data.Jobs.ForEach(x => Jobs.Add(x));
        }

        private void TxtName_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editjob == null || string.IsNullOrEmpty(txtName.Text)) return;
            editjob.Name = txtName.Text;
            QuartzJsonContext.Instance.Update(editjob);
            QuartzJsonContext.Instance.Commit();
            QuartzJsonContext.Instance.Reload();
            RefreshJobList();
        }

        private void TxtGroup_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editjob == null || string.IsNullOrEmpty(txtGroup.Text)) return;
            editjob.Group = txtGroup.Text;
            QuartzJsonContext.Instance.Update(editjob);
            QuartzJsonContext.Instance.Commit();
            QuartzJsonContext.Instance.Reload();
            RefreshJobList();
        }

        private void TxtDescription_LostFocus(object sender, RoutedEventArgs e)
        {
            if (editjob == null || string.IsNullOrEmpty(txtDescription.Text)) return;
            editjob.Description = txtDescription.Text;
            QuartzJsonContext.Instance.Update(editjob);
            QuartzJsonContext.Instance.Commit();
            QuartzJsonContext.Instance.Reload();
            RefreshJobList();
        }

        private void ChkActive_Checked(object sender, RoutedEventArgs e)
        {
            if (editjob == null) return;
            if (!string.IsNullOrEmpty(editjob.Name) && !string.IsNullOrEmpty(editjob.Group))
            {
                editjob.Active = ((CheckBox)sender).IsChecked.Value;
                QuartzJsonContext.Instance.Update(editjob);
                QuartzJsonContext.Instance.Commit();
                QuartzJsonContext.Instance.Reload();
                RefreshJobList();
            }
            else
            {
                WpfUtils.DialogUtils.Alert("Impostare nome e gruppo del job");
                ((CheckBox)sender).IsChecked = false;
            }
        }

        private void BtnStopService_Click(object sender, RoutedEventArgs e)
        {
            icServiceStopped.Visibility = Visibility.Collapsed;
            icServiceRunnig.Visibility = Visibility.Collapsed;
            icServiceNotFound.Visibility = Visibility.Collapsed;
            icLoader.Visibility = Visibility.Visible;
            Task.Run(() =>
            {
                windService.Stop();
                windService.WaitForStatus(ServiceControllerStatus.Stopped);
                WpfUtils.AsyncUtils.ExecuteInView(this, () => {
                    InitServiceStatus();
                });
            });
        }

        private void BtnRestratService_Click(object sender, RoutedEventArgs e)
        {
            icServiceStopped.Visibility = Visibility.Collapsed;
            icServiceRunnig.Visibility = Visibility.Collapsed;
            icServiceNotFound.Visibility = Visibility.Collapsed;
            icLoader.Visibility = Visibility.Visible;
            Task.Run(() =>
            {
                windService.Stop();
                windService.WaitForStatus(ServiceControllerStatus.Stopped);
                WpfUtils.AsyncUtils.ExecuteInView(this, () => {
                    InitServiceStatus();
                });
            }).ContinueWith((t) => {
                WpfUtils.AsyncUtils.ExecuteInView(this, () => {
                    icServiceStopped.Visibility = Visibility.Collapsed;
                    icServiceRunnig.Visibility = Visibility.Collapsed;
                    icServiceNotFound.Visibility = Visibility.Collapsed;
                    icLoader.Visibility = Visibility.Visible;
                });
                windService.Start();
                windService.WaitForStatus(ServiceControllerStatus.Running);
                WpfUtils.AsyncUtils.ExecuteInView(this, () => {
                    InitServiceStatus();
                });
            });
        }

        private void BtnInstallSevice_Click(object sender, RoutedEventArgs e)
        {
            var instellerPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "IceQuartzServiceInstaller", "ICeQuartzSchedulerServiceWizard.msi");
            var p = System.Diagnostics.Process.Start(instellerPath);
            p.WaitForExit();
            InitServiceStatus();
        }

        private void BtnRunService_Click(object sender, RoutedEventArgs e)
        {
            icServiceStopped.Visibility = Visibility.Collapsed;
            icServiceRunnig.Visibility = Visibility.Collapsed;
            icServiceNotFound.Visibility = Visibility.Collapsed;
            icLoader.Visibility = Visibility.Visible;
            Task.Run(() =>
            {
                windService.Start();
                windService.WaitForStatus(ServiceControllerStatus.Running);
                WpfUtils.AsyncUtils.ExecuteInView(this, () => {
                    InitServiceStatus();
                });
            });
        }

        private void BtnSettings_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var preferencesWin = new Windows.Preferences();
            preferencesWin.Owner = this;
            preferencesWin.ShowDialog();
            InitServiceStatus();
            InitConfig();
            if (config != null)
            {
                InitJobList();
                DataContext = this;
            }
        }
    }

    public class StringsToContentConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is List<DayOfWeek>)) return Binding.DoNothing;

            return String.Join(",", (List<DayOfWeek>)value );
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
