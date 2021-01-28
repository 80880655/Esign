using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.Data
{
    [Serializable]
    public class ErrorTextCollection : List<ErrorText>
    {
        public override string ToString()
        {
            StringBuilder text = new StringBuilder();
            foreach (ErrorText t in this)
                text.Append(t + ",");
            return text.ToString().TrimEnd(',');
        }
    }

    [Serializable]
    public class ErrorText
    {
        public ErrorText() { }

        public ErrorText(string key, string text)
        {
            Key = key;
            Text = text;
        }

        public ErrorText(string key, string text, params object[] args)
            : this(key, text)
        {
            Args = args;
        }

        public string Key { get; set; }

        public string Text { get; set; }

        public object[] Args { get; set; }


        #region ErrorText

        /// <summary>
        /// Is required
        /// </summary>
        public static ErrorText Require = new ErrorText("Require", "is required");
        /// <summary>
        /// Exists
        /// </summary>
        public static ErrorText Exists = new ErrorText("Exists", "already exists");
        /// <summary>
        /// Is not found
        /// </summary>
        public static ErrorText NotFound = new ErrorText("NotFound", "was not found");
        /// <summary>
        /// Maximum length is {0}, current is {1}
        /// </summary>
        public static ErrorText MaxLength = new ErrorText("MaxLength", "maximum length is {0}, current is {1}");
        /// <summary>
        /// Minimum length is {0}, current is {1}
        /// </summary>
        public static ErrorText MinLength = new ErrorText("MinLength", "minimum length is {0}, current is {1}");
        /// <summary>
        /// Maximum value is {0}, current is {1}
        /// </summary>
        public static ErrorText MaxValue = new ErrorText("MaxValue", "maximum length is {0}, current is {1}");
        /// <summary>
        /// Minimum value is {0}, current is {1}
        /// </summary>
        public static ErrorText MinValue = new ErrorText("MinValue", "minimum value is {0}, current is {1}");

        #endregion

        public override string ToString()
        {
            return string.Format(Text, Args);
        }
    }
}
