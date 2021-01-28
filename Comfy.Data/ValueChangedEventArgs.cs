using System;

namespace Comfy.Data
{
    public class ValueChangedEventArgs : EventArgs
    {
        public ValueChangedEventArgs() { }

        public ValueChangedEventArgs(string propertyName, object newValue, object oldValue)
        {
            PropertyName = propertyName;
            NewValue = newValue;
            OldValue = oldValue;
        }

        public string PropertyName { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}
