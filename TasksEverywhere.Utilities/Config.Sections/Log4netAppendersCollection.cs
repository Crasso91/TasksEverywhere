using System.Configuration;

namespace TasksEverywhere.Utilities.Config.Sections
{
    
    public sealed partial class Log4netAppendersCollection : ConfigurationElementCollection
    {
        public Log4netAppendersElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as Log4netAppendersElement;
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

        public new Log4netAppendersElement this[string responseString]
        {
            get { return (Log4netAppendersElement)BaseGet(responseString); }
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
            return new Log4netAppendersElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((Log4netAppendersElement)(element)).Name;
        }
    }
}