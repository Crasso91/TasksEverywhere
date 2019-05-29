using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed partial class LoggerSectionElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return ((string)(this["type"]));
            }
            set
            {
                this["type"] = value;
            }
        }

        [ConfigurationProperty("level", IsRequired = true)]
        public string Level
        {
            get
            {
                return ((string)(this["level"]));
            }
            set
            {
                this["level"] = value;
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

        [ConfigurationProperty("parameters")]
        [ConfigurationCollection(typeof(LoggerSectionParameterElement), AddItemName = "parameter")]
        public LoggerSectionParameterCollection Parameters
        {
            get
            {
                return ((LoggerSectionParameterCollection)base["parameters"]);
            }
        }
    }
}