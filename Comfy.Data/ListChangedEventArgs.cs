using System;

namespace Comfy.Data
{
    public class ListChangedEventArgs<T> : EventArgs
        where T: DataObject<T>
    {
        public ListChangedEventArgs() { }

        public ListChangedEventArgs(ListChangedType type, EditedObject<T> obj)
        {
            EditedObject = obj;
            ListChangedType = type;
        }

        public EditedObject<T> EditedObject { get; set; }

        public ListChangedType ListChangedType { get; set; }
    }
}
