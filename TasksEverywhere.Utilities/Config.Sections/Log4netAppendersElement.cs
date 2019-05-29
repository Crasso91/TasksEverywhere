using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed partial class Log4netAppendersElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get
            {
                return ((string)(this["name"]));
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("active", IsRequired = true)]
        public bool Active
        {
            get
            {
                return ((bool)(this["active"]));
            }
            set
            {
                this["active"] = value;
            }
        }

    }
}