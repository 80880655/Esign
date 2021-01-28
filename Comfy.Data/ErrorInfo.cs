using System;
using System.Collections.Generic;

namespace Comfy.Data
{
    [Serializable]
    public class ErrorInfoCollection : IEnumerable<ErrorInfo>
    {
        List<ErrorInfo> list = new List<ErrorInfo>();

        public void Add(ErrorInfo error)
        {
            ErrorInfo e = this[error.FiledName];
            if (e != null)
                e.Errors.AddRange(error.Errors);
            else
                list.Add(error);
        }

        public int Count
        {
            get { return list.Count; }
        }

        public void Clear()
        {
            list.Clear();
        }

        public ErrorInfo this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public ErrorInfo this[string fieldName]
        {
            get
            {
                foreach (ErrorInfo error in list)
                    if (error.FiledName.Equals(fieldName))
                        return error;
                return null;
            }
            set
            {
                for (int i = 0; i < list.Count; i++)
                    if (list[i].FiledName.Equals(fieldName))
                        list[i] = value;
            }
        }

        public bool ContainKey(string fieldName)
        {
            foreach (ErrorInfo error in list)
                if (error.FiledName.Equals(fieldName))
                    return true;
            return false;
        }

        public IEnumerator<ErrorInfo> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }

    [Serializable]
    public class ErrorInfo
    {
        public ErrorInfo()
        {
            Errors = new ErrorTextCollection();
        }

        public ErrorInfo(string fieldName, string key, string text, params object[] args)
            : this()
        {
            FiledName = fieldName;
            Errors.Add(new ErrorText(key, text, args));
        }

        public ErrorInfo(string fieldName, ErrorText error)
            : this()
        {
            FiledName = fieldName;
            Errors.Add(error);
        }

        public string FiledName { get; set; }

        public ErrorTextCollection Errors { get; set; }
    }
}
