using System;
using System.Collections.Generic;
using System.Text;

namespace Comfy.Data
{
    [Serializable()]
    public class Validator
    {
        ErrorInfoCollection errors = new ErrorInfoCollection();

        /// <summary>
        /// Checks for a condition and add error if the condition is false.
        /// <para>Return false if has error.</para>
        /// </summary>
        /// <param name="result"></param>
        /// <param name="fieldName"></param>
        /// <param name="error"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Check(bool condition, string fieldName, ErrorText error, params object[] args)
        {
            if (!condition)
            {
                AddError(fieldName, error, args);
                return false;
            }
            return true;
        }

        public bool Check(bool condition, string fieldName, string key, string text, params object[] args)
        {
            if (!condition)
            {
                AddError(fieldName, key, text, args);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value equals default(T).
        /// <para>Return false if has error.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        public bool Require<T>(T value, string fieldName)
        {
            if (object.Equals(value, default(T)))
            {
                AddError(fieldName, ErrorText.Require);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value is null or empty.
        /// <para>Return false if has error.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        public bool Require(string value, string fieldName)
        {
            if (string.IsNullOrEmpty(value))
            {
                AddError(fieldName, ErrorText.Require);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value is DateTime.MinValue or DateTime.MaxValue.
        /// <para>Return false if has error.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="property"></param>
        public bool Require(DateTime value, string fieldName)
        {
            if (value == DateTime.MinValue || value == DateTime.MaxValue)
            {
                AddError(fieldName, ErrorText.Require);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value's length is greater then lenght.
        /// <para>Return false if has error.</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="property"></param>
        public bool MaxLength(string value, int length, string fieldName)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > length)
            {
                AddError(fieldName, ErrorText.MaxLength, 
                    length, value.Length);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value's length is less then lenght.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="length"></param>
        /// <param name="property"></param>
        public bool MinLength(string value, int length, string fieldName)
        {
            if (string.IsNullOrEmpty(value) || value.Length < length)
            {
                AddError(fieldName, ErrorText.MinLength, 
                    length, value == null ? 0 : value.Length);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Add error if value.CompareTo(minValue) &lt; 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="minValue"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public bool MinValue<T>(T value, T minValue, string fieldName) where T : IComparable
        {
            if (value != null && value.CompareTo(minValue) >= 0)
                return true;
            AddError(fieldName, ErrorText.MinValue, minValue, value);
            return false;
        }

        /// <summary>
        /// Add error if value.CompareTo(maxValue) &gt; 0.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <param name="maxValue"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public bool MaxValue<T>(T value, T maxValue, string fieldName) where T : IComparable
        {
            if (value != null && value.CompareTo(maxValue) <= 0)
                return true;
            AddError(fieldName, ErrorText.MaxValue, maxValue, value);
            return false;
        }

        public void AddError(string fieldName, ErrorText error, params object[] args)
        {
            error.Args = args;
            errors.Add(new ErrorInfo(fieldName, error));
        }

        public void AddError(string fieldName, ErrorText error)
        {
            errors.Add(new ErrorInfo(fieldName, error));
        }

        public void AddError(string fieldName, string key, string text, params object[] args)
        {
            errors.Add(new ErrorInfo(fieldName, key, text, args));
        }

        public void AddError(string fieldName, string text)
        {
            errors.Add(new ErrorInfo(fieldName, "", text));
        }

        /// <summary>
        /// Return true if there's no error.
        /// </summary>
        public bool IsValid
        {
            get { return errors.Count == 0; }
        }

        /// <summary>
        /// Error info.
        /// </summary>
        public ErrorInfoCollection ErrorInfos
        {
            get { return errors; }
        }

        public override string ToString()
        {
            if (IsValid)
                return "Valid";
            else
            {
                StringBuilder text = new StringBuilder();
                foreach (ErrorInfo error in errors)
                    text.Append(error.FiledName + ":" + error.Errors + ";");
                return text.ToString();
            }
        }
    }
}
