using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed partial class LoggerSectionCollection : ConfigurationElementCollection
    {
        public LoggerSectionElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as LoggerSectionElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        public new LoggerSectionElement this[string responseString]
        {
            get { return (LoggerSectionElement)BaseGet(responseString); }
            set
            {
                if (BaseGet(responseString) != null)
                {
                    BaseRemoveAt(BaseIndexOf(BaseGet(responseString)));
                }
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new LoggerSectionElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerSectionElement)(element)).Type;
        }
    }
}