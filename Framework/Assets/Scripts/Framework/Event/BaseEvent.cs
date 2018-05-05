namespace Frameworks
{
    using System.Collections;

    public class BaseEvent
    {
        protected Hashtable arguments;
        protected string type;

        public BaseEvent(string type)
        {
            this.Type = type;
        }

        public BaseEvent(string type, object target)
        {
            this.Type = type;
            this.Target = target;
        }

        public BaseEvent Clone()
        {
            return new BaseEvent(this.type);
        }

        public Hashtable Params
        {
            get { return this.arguments; }
            set { this.arguments = value; }
        }

        public object Target { get; set; }

        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
    }
}

