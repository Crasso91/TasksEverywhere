using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed class LoggerSectionParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get
            {
                return ((string)(this["key"]));
            }
            set
            {
                this["key"] = value;
            }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get
            {
                return ((string)(this["value"]));
            }
            set
            {
                this["value"] = value;
            }
        }
    }
}