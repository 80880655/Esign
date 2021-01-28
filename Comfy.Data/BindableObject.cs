using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Comfy.Data
{
    [Serializable()]
    public class BindableObject : INotifyPropertyChanged
    {
        [NonSerialized]
        PropertyChangedEventHandler propertyChanged;
        [NonSerialized]
        EventHandler<ValueChangedEventArgs> valueChanged;

        public event EventHandler<ValueChangedEventArgs> ValueChanged
        {
            add
            {
                valueChanged = (EventHandler<ValueChangedEventArgs>)
                    System.Delegate.Combine(valueChanged, value);
            }
            remove
            {
                valueChanged = (EventHandler<ValueChangedEventArgs>)
                    System.Delegate.Remove(valueChanged, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                propertyChanged = (PropertyChangedEventHandler)
                    System.Delegate.Combine(propertyChanged, value);
            }
            remove
            {
                propertyChanged = (PropertyChangedEventHandler)
                    System.Delegate.Remove(propertyChanged, value);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual void OnPropertyChanged(string propertyName)
        {
            if (propertyChanged != null)
                propertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal virtual void OnValueChanged(string propertyName, object newValue, object oldValue)
        {
            if (valueChanged != null)
                valueChanged.Invoke(this, new ValueChangedEventArgs(propertyName, newValue, oldValue));
        }
    }
}
