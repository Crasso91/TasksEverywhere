using Configuration;
using ICeQuartScheduler.Manager.ModelConfig;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
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
    /// Interaction logic for Preferences.xaml
    /// </summary>
    public partial class Preferences : Window
    {
        private JsonConfiguration config;

        public Preferences()
        {
            InitializeComponent();
            InitConfig();
        }

        private void InitConfig()
        {
            var path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var configPath = System.IO.Path.Combine(path, "config", "AppConfig.json");
            config = ExternalConfig.GetConfig<JsonConfiguration>(configPath);

            if (!string.IsNullOrEmpty(config.QuartzConfigPath)) QuartzFileSelector.Label = config.QuartzConfigPath;
            if (!string.IsNullOrEmpty(config.WhereRootPath)) WhereFileSelector.Label = config.WhereRootPath;
        }

        private void BtnSalva_Click(object sender, RoutedEventArgs e)
        {
            config.QuartzConfigPath = QuartzFileSelector.Path;
            config.WhereRootPath = WhereFileSelector.Path;
            ExternalConfig.Save(config);
            this.Close();
        }
    }
}
