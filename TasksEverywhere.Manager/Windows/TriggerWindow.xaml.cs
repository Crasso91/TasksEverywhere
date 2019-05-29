using TasksEverywhere.Extensions;
using TasksEverywhere.Quartz.Context.Jobs.Abstract;
using TasksEverywhere.Utilities.Enums;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Interaction logic for TriggerWindow.xaml
    /// </summary>
    public partial class TriggerWindow : Window
    {
        private ICustomTrigger trigger;
        public Action<ICustomTrigger> OnSave;
        private bool IsNew;
        public TriggerWindow(ICustomTrigger _trigger, bool isNew, Action<ICustomTrigger> onSave = null)
        {
            InitializeComponent();
            trigger = _trigger;
            IsNew = isNew;
            OnSave = onSave;
            cbCadencyUnit.ItemsSource = Enum.GetValues(typeof(IntervalUnit));
            cbLifeUnit.ItemsSource = Enum.GetValues(typeof(IntervalUnit));
            chkLun.IsChecked = false;
            chkMar.IsChecked = false;
            chkMer.IsChecked = false;
            chkGio.IsChecked = false;
            chkVen.IsChecked = false;
            chkSab.IsChecked = false;
            chkDom.IsChecked = false;
            txtName.IsEnabled = isNew;
            txtGroup.IsEnabled = isNew;
            if (!isNew) EditTrigger(trigger);
        }

        private void EditTrigger(ICustomTrigger _trigger)
        {
            txtName.Text = _trigger.Name;
            txtGroup.Text = _trigger.Group;
            dtStartDate.Text = _trigger.StartDate.ToString("dd/MM/yyyy");
            txtStartHour.Text = _trigger.StartDate.ToString("HH:mm");
            chkActive.IsChecked = trigger.Active;

            if(_trigger.Period == PeriodType.Giornaliero)
            {
                rbDaily.IsChecked = true;
                SetDailyPane(_trigger);
            }
            if(_trigger.Period == PeriodType.Settimanale)
            {
                rbWeekly.IsChecked = true;
                SetDailyPane(_trigger);
                foreach(var day in _trigger.WeekDays)
                {
                    if (!chkLun.IsChecked.Value) chkLun.IsChecked = day == DayOfWeek.Monday;
                    if(!chkMar.IsChecked.Value) chkMar.IsChecked = day == DayOfWeek.Tuesday;
                    if(!chkMer.IsChecked.Value) chkMer.IsChecked = day == DayOfWeek.Wednesday;
                    if(!chkGio.IsChecked.Value) chkGio.IsChecked = day == DayOfWeek.Thursday;
                    if(!chkVen.IsChecked.Value) chkVen.IsChecked = day == DayOfWeek.Friday;
                    if(!chkSab.IsChecked.Value) chkSab.IsChecked = day == DayOfWeek.Saturday;
                    if(!chkDom.IsChecked.Value) chkDom.IsChecked = day == DayOfWeek.Sunday;
                }
            }
            if (_trigger.Period == PeriodType.Mensile)
                txtMonthDay.Text = _trigger.StartDate.Day.ToString();
        }

        private void SetDailyPane(ICustomTrigger _trigger)
        {
            chkRepeat.IsChecked = _trigger.Interval == 0;
            cbCadencyUnit.SelectedValue = _trigger.IntervalUnit;
            txtCadency.Text = _trigger.Interval.ToString();
            cbLifeUnit.SelectedValue = _trigger.LifeUnit;
            txtLife.Text = _trigger.Life.ToString();
        }

        private void RbDaily_Checked(object sender, RoutedEventArgs e)
        {
            DailyData.Visibility = Visibility.Visible;
            WeeklyData.Visibility = Visibility.Collapsed;
            MonthlyData.Visibility = Visibility.Collapsed;
        }
        private void RbWeekly_Checked(object sender, RoutedEventArgs e)
        {
            DailyData.Visibility = Visibility.Visible;
            WeeklyData.Visibility = Visibility.Visible;
            MonthlyData.Visibility = Visibility.Collapsed;
        }
        private void RbMonthly_Checked(object sender, RoutedEventArgs e)
        {
            DailyData.Visibility = Visibility.Collapsed;
            WeeklyData.Visibility = Visibility.Collapsed;
            MonthlyData.Visibility = Visibility.Visible;
        }
        private void RbUnique_Checked(object sender, RoutedEventArgs e)
        {
            DailyData.Visibility = Visibility.Collapsed;
            WeeklyData.Visibility = Visibility.Collapsed;
            MonthlyData.Visibility = Visibility.Collapsed;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]{2}[:]{1}[0-9]{2}");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnConferma_Click(object sender, RoutedEventArgs e)
        {
            trigger.Name = txtName.Text;
            trigger.Group = txtGroup.Text;
            trigger.Interval = txtCadency.Text.ToType<int>();
            trigger.IntervalUnit = cbCadencyUnit.SelectedValue.ToString().ToEnum<IntervalUnit>();
            trigger.Life = txtLife.Text.ToType<int>();
            trigger.LifeUnit = cbLifeUnit.SelectedValue.ToString().ToEnum<IntervalUnit>();
            trigger.Active = chkActive.IsChecked.Value;

            if (rbDaily.IsChecked.Value) trigger.Period = PeriodType.Giornaliero;
            if (rbWeekly.IsChecked.Value) trigger.Period = PeriodType.Settimanale;
            if (rbMonthly.IsChecked.Value) trigger.Period = PeriodType.Mensile;

            trigger.WeekDays.Clear();
            if (chkLun.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Monday);
            if (chkMar.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Tuesday);
            if (chkMer.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Wednesday);
            if (chkGio.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Thursday);
            if (chkVen.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Friday);
            if (chkSab.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Saturday);
            if (chkDom.IsChecked.Value) trigger.WeekDays.Add(DayOfWeek.Sunday);
            if (trigger.Period != PeriodType.Mensile)
            {
                var date = DateTime.Parse(dtStartDate.Text + " " + txtStartHour.Text);
                trigger.StartDate = date;
            }
            else
            {
                trigger.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(-1).Month, txtMonthDay.Text.ToType<int>());
            }
            OnSave(trigger);
            this.Close();
        }

        private void BtnAnnulla_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
