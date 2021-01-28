using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace Comfy.Data
{
    [Serializable]
    public class DataObjectList<T> : IList<T>, IList, ISupportInitializeNotification, IEditableObject, ICloneable
        where T : DataObject<T>
    {
        #region fileds

        List<T> list = new List<T>();
        [NonSerialized]
        List<T> deleted = new List<T>();
        [NonSerialized]
        List<T> modified = new List<T>();
        [NonSerialized]
        List<T> added = new List<T>();
        [NonSerialized]
        bool isInitialized = true;
        [NonSerialized]
        int editLevel = 0;
        [NonSerialized]
        List<EditedObject<T>> track = new List<EditedObject<T>>();
        [NonSerialized]
        bool editable = false;
        [NonSerialized]
        EventHandler<ListChangedEventArgs<T>> listChangedHandler;
        [NonSerialized]
        EventHandler initializedHandler;

        #endregion

        public DataObjectList()
        {
        }

        protected DataObjectList(SerializationInfo info, StreamingContext context)
        {
        }

        public event EventHandler<ListChangedEventArgs<T>> ListChanged
        {
            add
            {
                listChangedHandler = (EventHandler<ListChangedEventArgs<T>>)
                    System.Delegate.Combine(listChangedHandler, value);
            }
            remove
            {
                listChangedHandler = (EventHandler<ListChangedEventArgs<T>>)
                    System.Delegate.Remove(listChangedHandler, value);
            }
        }

        #region ISupportInitializeNotification Members

        public event EventHandler Initialized
        {
            add
            {
                initializedHandler = (EventHandler)
                    System.Delegate.Combine(initializedHandler, value);
            }
            remove
            {
                initializedHandler = (EventHandler)
                    System.Delegate.Remove(initializedHandler, value);
            }
        }

        public bool IsInitialized { get { return isInitialized; } }

        public void BeginInit()
        {
            isInitialized = false;
        }

        public void EndInit()
        {
            isInitialized = true;
            if (initializedHandler != null)
                initializedHandler.Invoke(this, new EventArgs());
        }

        #endregion

        public bool Editable
        {
            get { return editable; }
        }

        public bool CanUndo { get { return editLevel > 0; } }

        public bool CanRedo { get { return editLevel < track.Count; } }

        public List<T> ModifiedList { get { return modified; } }

        public List<T> DeletedList { get { return deleted; } }

        public List<T> AddedList { get { return added; } }

        public bool HasChanged { get { return modified.Count > 0 || added.Count > 0 || deleted.Count > 0; } }

        EditedObject<T> Undo(bool suppressEvent)
        {
            isInitialized = false;
            EditedObject<T> editTrack = track[--editLevel];
            switch (editTrack.NewState)
            {
                case DataState.Modified:
                    editTrack.DataObject.GetType().GetProperty(editTrack.PropertyName).SetValue(editTrack.DataObject, editTrack.OldValue, null);
                    if (editTrack.OldState == DataState.None)
                    {
                        editTrack.DataObject.ResetState();
                        modified.Remove(editTrack.DataObject);
                    }
                    editTrack.DataObject.OnPropertyChanged(editTrack.PropertyName);
                    editTrack.DataObject.OnValueChanged(editTrack.PropertyName, editTrack.OldValue, editTrack.NewValue);
                    break;
                case DataState.Deleted:
                    switch (editTrack.OldState)
                    {
                        case DataState.None:
                            editTrack.DataObject.ResetState();
                            deleted.Remove(editTrack.DataObject);
                            break;
                        case DataState.Created:
                            added.Add(editTrack.DataObject);
                            break;
                        case DataState.Modified:
                            modified.Add(editTrack.DataObject);
                            editTrack.DataObject.MarkModified();
                            deleted.Remove(editTrack.DataObject);
                            break;
                    }
                    list.Add(editTrack.DataObject);
                    editTrack.DataObject.ValueChanged += item_ValueChanged;
                    break;
                case DataState.Created:
                    added.Remove(editTrack.DataObject);
                    list.Remove(editTrack.DataObject);
                    editTrack.DataObject.ValueChanged -= item_ValueChanged;
                    break;
            }
            isInitialized = true;
            if (!suppressEvent)
                OnListChanged(new ListChangedEventArgs<T>(ListChangedType.ItemUndo, editTrack));
            return editTrack;
        }

        public EditedObject<T> Undo()
        {
            return Undo(false);
        }

        EditedObject<T> Redo(bool suppressEvent)
        {
            isInitialized = false;
            EditedObject<T> editTrack = track[editLevel++];
            switch (editTrack.NewState)
            {
                case DataState.Modified:
                    editTrack.DataObject.GetType().GetProperty(editTrack.PropertyName).SetValue(editTrack.DataObject, editTrack.NewValue, null);
                    if (editTrack.OldState == DataState.None)
                    {
                        modified.Add(editTrack.DataObject);
                        editTrack.DataObject.MarkModified();
                    }
                    editTrack.DataObject.OnPropertyChanged(editTrack.PropertyName);
                    editTrack.DataObject.OnValueChanged(editTrack.PropertyName, editTrack.NewValue, editTrack.OldValue);
                    break;
                case DataState.Deleted:
                    if (editTrack.DataObject.DataState == DataState.Created)
                        added.Remove(editTrack.DataObject);
                    else
                    {
                        if (editTrack.DataObject.DataState == DataState.Modified)
                            modified.Remove(editTrack.DataObject);
                        deleted.Add(editTrack.DataObject);
                    }
                    list.Remove(editTrack.DataObject);
                    editTrack.DataObject.MarkDeleted();
                    editTrack.DataObject.ValueChanged -= item_ValueChanged;
                    break;
                case DataState.Created:
                    added.Add(editTrack.DataObject);
                    list.Add(editTrack.DataObject);
                    editTrack.DataObject.MarkCreated();
                    editTrack.DataObject.ValueChanged += item_ValueChanged;
                    break;
            }
            isInitialized = true;
            if (!suppressEvent)
                OnListChanged(new ListChangedEventArgs<T>(ListChangedType.ItemRedo, editTrack));
            return editTrack;
        }

        public EditedObject<T> Redo()
        {
            return Redo(false);
        }

        protected void OnListChanged(ListChangedEventArgs<T> e)
        {
            if (listChangedHandler != null)
                listChangedHandler.Invoke(this, e);
        }

        protected void AddTrack(EditedObject<T> editTrack)
        {
            if (editLevel < track.Count)
                track.RemoveRange(editLevel, track.Count - editLevel);
            track.Add(editTrack);
            editLevel++;
        }

        [OnDeserialized()]
        protected internal void OnDeserialized(StreamingContext context)
        {
            deleted = new List<T>();
            modified = new List<T>();
            added = new List<T>();
            track = new List<EditedObject<T>>();
            isInitialized = true;
        }

        protected void SetModified(T obj, string propertyName, object oldValue, object newValue)
        {
            if (!IsInitialized || !Editable) return;
            EditedObject<T> editedObj = new EditedObject<T>()
            {
                DataObject = obj,
                OldState = obj.DataState,
                NewState = DataState.Modified,
                OldValue = oldValue,
                NewValue = newValue,
                PropertyName = propertyName
            };
            if (obj.DataState == DataState.None)
            {
                modified.Add(obj);
                obj.MarkModified();
            }
            AddTrack(editedObj);
            OnListChanged(new ListChangedEventArgs<T>(ListChangedType.ItemModified, editedObj));
        }

        protected void SetAdded(T item)
        {
            item.ValueChanged += item_ValueChanged;
            if (!IsInitialized || !Editable) return;
            EditedObject<T> editedObj = new EditedObject<T>()
            {
                DataObject = item,
                OldState = item.DataState,
                NewState = DataState.Created
            };
            added.Add(item);
            item.MarkCreated();
            AddTrack(editedObj);
            OnListChanged(new ListChangedEventArgs<T>(ListChangedType.ItemAdded, editedObj));
        }

        void item_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            SetModified((T)sender, e.PropertyName, e.OldValue, e.NewValue);
        }

        protected void SetDeleted(T item)
        {
            item.ValueChanged -= item_ValueChanged;
            if (!IsInitialized || !Editable) return;
            EditedObject<T> editedObj = new EditedObject<T>()
            {
                DataObject = item,
                OldState = item.DataState,
                NewState = DataState.Deleted,
            };
            if (item.DataState == DataState.Created)
                added.Remove(item);
            else
            {
                if (item.DataState == DataState.Modified)
                    modified.Remove(item);
                deleted.Add(item);
                item.MarkDeleted();
            }
            AddTrack(editedObj);
            OnListChanged(new ListChangedEventArgs<T>(ListChangedType.ItemDeleted, editedObj));
        }

        private bool IsCompatibleObject(object value)
        {
            if (!(value is T) && ((value != null) || typeof(T).IsValueType))
                return false;
            return true;
        }

        public void AddRange(IEnumerable<T> collection)
        {
            foreach (T item in collection)
                Add(item);
        }


        #region IList

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
            SetAdded(item);
        }

        public void RemoveAt(int index)
        {
            T item = list[index];
            list.RemoveAt(index);
            SetDeleted(item);
        }

        public T this[int index]
        {
            get { return list[index]; }
            set
            {
                T item = list[index];
                SetDeleted(item);
                list[index] = value;
                SetAdded(value);
            }
        }

        public void Add(T item)
        {
            list.Add(item);
            SetAdded(item);
        }

        public void Clear()
        {
            list.Clear();
            deleted.Clear();
            added.Clear();
            modified.Clear();
            editLevel = 0;
            track.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return list.Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return ((ICollection<T>)list).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            bool result = list.Remove(item);
            if (result)
                SetDeleted(item);
            return result;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }

        #endregion

        #region IList Members

        int IList.Add(object value)
        {
            Add((T)value);
            return (this.Count - 1);
        }

        bool IList.Contains(object value)
        {
            if (IsCompatibleObject(value))
                return Contains((T)value);
            return false;
        }

        int IList.IndexOf(object value)
        {
            if (IsCompatibleObject(value))
                return IndexOf((T)value);
            return -1;
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        bool IList.IsFixedSize
        {
            get { return ((IList)list).IsFixedSize; }
        }

        bool IList.IsReadOnly
        {
            get { return ((IList)list).IsReadOnly; }
        }

        void IList.Remove(object value)
        {
            if (IsCompatibleObject(value))
                Remove((T)value);
        }

        object IList.this[int index]
        {
            get { return this[index]; }
            set { this[index] = (T)value; }
        }

        #endregion

        #region ICollection Members

        void ICollection.CopyTo(Array array, int index)
        {
            Array.Copy(list.ToArray(), 0, array, index, this.list.Count);
        }

        int ICollection.Count
        {
            get { return Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return ((ICollection)list).IsSynchronized; }
        }

        object ICollection.SyncRoot
        {
            get { return ((ICollection)list).SyncRoot; }
        }

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return ObjectCloner.Clone(this);
        }

        #endregion

        #region IEditableObject Members

        public void BeginEdit()
        {
            editable = true;
        }

        public void CancelEdit()
        {
            while (CanUndo)
            {
                Undo(true);
            }
            track.Clear();
            OnListChanged(new ListChangedEventArgs<T>(ListChangedType.Reset, null));
        }

        public void EndEdit()
        {
            foreach (T item in list)
                item.ResetState();
            modified.Clear();
            deleted.Clear();
            added.Clear();
            track.Clear();
            editLevel = 0;
            editable = false;
        }

        #endregion
    }
}
