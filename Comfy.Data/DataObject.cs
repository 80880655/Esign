using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Comfy.Data
{
    [Serializable]
    public abstract partial class DataObject<T> : BindableObject, ICloneable
        where T : DataObject<T>
    {
        protected DataState dataState = DataState.None;

        public int GridRowCount { get; set; }
        /// <summary>
        /// The state of the object.
        /// </summary>
        [Browsable(false)]
        public DataState DataState
        {
            get { return dataState; }
        }

        /// <summary>
        /// Set the value by ref.And the events will be fired if the DataState is not Initializing.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        protected void SetValue<V>(ref V field, V value)
        {
            if (!object.Equals(field, value))
            {
                //object oldValue = ObjectCloner.Clone(field);
                //object newValue = ObjectCloner.Clone(value);
                object oldValue = field;// ObjectCloner.Clone(field);
                object newValue = value;// ObjectCloner.Clone(value);
                field = value;
                if (DataState != DataState.Initializing)
                {
                    string propertyName = new System.Diagnostics.StackTrace().GetFrame(1).GetMethod().Name.Substring(4);
                    OnValueChanged(propertyName, newValue, oldValue);
                    OnPropertyChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Set the value by ref. And the events will be fired if the DataState is not Initializing.
        /// </summary>
        /// <typeparam name="V"></typeparam>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="propertyName"></param>
        protected void SetValue<V>(ref V field, V value, string propertyName)
        {
            if (!object.Equals(field, value))
            {
                //object oldValue = ObjectCloner.Clone(field);
                //object newValue = ObjectCloner.Clone(value);
                object oldValue = field;// ObjectCloner.Clone(field);
                object newValue = value;// ObjectCloner.Clone(value);
                field = value;
                if (DataState != DataState.Initializing)
                {
                    OnValueChanged(propertyName, newValue, oldValue);
                    OnPropertyChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Change the DataState to Created.
        /// </summary>
        public void MarkCreated()
        {
            dataState = DataState.Created;
        }

        /// <summary>
        /// Change the DataState to Modified.
        /// </summary>
        public void MarkModified()
        {
            dataState = DataState.Modified;
        }

        /// <summary>
        /// Change the DataState to Deleted.
        /// </summary>
        public void MarkDeleted()
        {
            dataState = DataState.Deleted;
        }

        /// <summary>
        /// Change the DataState to None.
        /// </summary>
        public void ResetState()
        {
            dataState = DataState.None;
        }

        public object Clone()
        {
            return ObjectCloner.Clone(this);
        }

        /// <summary>
        /// Change the DataState to Initializing.
        /// </summary>
        public void BeginInit()
        {
            dataState = DataState.Initializing;
        }

        /// <summary>
        /// Call ResetState().
        /// </summary>
        public void EndInit()
        {
            ResetState();
        }
    }
}
