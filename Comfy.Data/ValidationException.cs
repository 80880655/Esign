using System;

namespace Comfy.Data
{
    [Serializable]
    public class ValidationException : Exception
    {
        ErrorInfoCollection _errors;
        string _innerStackTrace;

        public ErrorInfoCollection ErrorInfos
        {
            get { return _errors; }
            set { _errors = value; }
        }

        public override string StackTrace
        {
            get
            {
                return String.Format("{0}{1}{2}",
                    _innerStackTrace, Environment.NewLine, base.StackTrace);
            }
        }

        public ValidationException(string message)
            : base(message)
        {
            _errors = new ErrorInfoCollection();
        }

        public ValidationException(Validator v)
            : base("Invalid")
        {
            _errors = v.ErrorInfos;
        }

        public ValidationException(string message, Validator v)
            : base(message)
        {
            _errors = v.ErrorInfos;
        }

        public ValidationException(string fieldName, string key, string text, params object[] args)
            : base(fieldName + " invalid")
        {
            _errors = new ErrorInfoCollection();
            _errors.Add(new ErrorInfo(fieldName, new ErrorText(key, text, args)));
        }

        public ValidationException(string fieldName, ErrorText error)
            : base(fieldName + " invalid")
        {
            _errors = new ErrorInfoCollection();
            _errors.Add(new ErrorInfo(fieldName, error));
        }

        public ValidationException(string fieldName, ErrorText error, params object[] args)
            : base(fieldName + " invalid")
        {
            error.Args = args;
            _errors = new ErrorInfoCollection();
            _errors.Add(new ErrorInfo(fieldName, error));
        }

        public ValidationException(Exception ex, Validator v)
            : base(ex.Message, ex)
        {
            _errors = v.ErrorInfos;
        }

        public ValidationException(Exception ex)
            : base(ex.Message, ex)
        {
            _errors = new ErrorInfoCollection();
        }

        protected ValidationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _errors = (ErrorInfoCollection)info.GetValue("_errors", typeof(ErrorInfoCollection));
            _innerStackTrace = info.GetString("_innerStackTrace");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_errors", _errors);
            info.AddValue("_innerStackTrace", _innerStackTrace);
        }

    }
}
