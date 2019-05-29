using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    public sealed partial class LoggerSectionParameterCollection : ConfigurationElementCollection
    {
        public LoggerSectionParameterCollection this[int index]
        {
            get
            {
                return base.BaseGet(index) as LoggerSectionParameterCollection;
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

        public new LoggerSectionParameterElement this[string responseString]
        {
            get { return (LoggerSectionParameterElement)BaseGet(responseString); }
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
            return new LoggerSectionParameterElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((LoggerSectionParameterElement)(element)).Key;
        }
    }
}