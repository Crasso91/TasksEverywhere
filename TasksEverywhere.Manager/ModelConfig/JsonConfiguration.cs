using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration.Entities.Abstract;

namespace ICeQuartScheduler.Manager.ModelConfig
{
    public class JsonConfiguration : Configuration.Entities.Abstract.IJsonConfig
    {
        public string ConfigName { get; set; }
        public string QuartzConfigPath { get; set; }
        public string WhereRootPath { get; set; }
    }
}
