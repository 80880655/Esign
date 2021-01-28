using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.Data
{
    [Serializable]
    public class EditedObject<T> where T:DataObject<T>
    {
        public DataState NewState { get; set; }
        public DataState OldState { get; set; }
        public T DataObject { get; set; }
        public string PropertyName { get; set; }
        public object NewValue { get; set; }
        public object OldValue { get; set; }
    }
}
