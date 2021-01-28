using System;
using System.Collections.Generic;

namespace Comfy.Data
{
    [Serializable]
    public class KeyTextList : List<KeyTextPair>
    {
        public void Add(int key, string text)
        {
            Add(new KeyTextPair(key, text));
        }
    }

    [Serializable]
    public class KeyTextPair : ICloneable, IComparable<KeyTextPair>, IEquatable<KeyTextPair>
    {
        public int Key { get; set; }

        public string Text { get; set; }

        public KeyTextPair() { }

        public KeyTextPair(int key, string text)
        {
            Key = key;
            Text = text;
        }

        public override string ToString()
        {
            if (Text == null)
                return string.Empty;
            return Text;
        }

        public override bool Equals(object obj)
        {
            KeyTextPair o = obj as KeyTextPair;
            if (object.Equals(o,null))
                return false;
            return Equals(o);
        }

        public override int GetHashCode()
        {
            return Key;
        }

        public object Clone()
        {
            return new KeyTextPair(Key, Text);
        }

        public int CompareTo(KeyTextPair other)
        {
            return Text.CompareTo(other.Text);
        }

        public bool Equals(KeyTextPair other)
        {
            return Key == other.Key && Text == other.Text;
        }

        public static bool operator ==(KeyTextPair a, KeyTextPair b)
        {
            if (object.Equals(a, null))
                return false;
            return a.Equals(b);
        }

        public static bool operator !=(KeyTextPair a, KeyTextPair b)
        {
            return !(a == b);
        }
    }
}
