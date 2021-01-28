using System;
using Comfy.Data.Core;

namespace Comfy.Data
{
    [Serializable]
    public class SqlException : Exception
    {
        string _Sql;
        string _innerStackTrace;

        public string Sql
        {
            get { return _Sql; }
            set { _Sql = value; }
        }

        public override string StackTrace
        {
            get
            {
                return String.Format("{0}{1}{2}",
                    _innerStackTrace, Environment.NewLine, base.StackTrace);
            }
        }

        public SqlException(string message, Exception ex, ISqlSection sql)
            : base(message, ex)
        {
            _innerStackTrace = ex.StackTrace;
            if (sql != null)
                _Sql = sql.ToDbCommandText();
        }

        public SqlException(string message, Exception ex, string sqlText)
            : base(message, ex)
        {
            _innerStackTrace = ex.StackTrace;
            _Sql = sqlText;
        }

        public SqlException(Exception ex, string sqlText)
            : base(ex.Message, ex)
        {
            _innerStackTrace = ex.StackTrace;
            _Sql = sqlText;
        }

        public SqlException(Exception ex) : this(ex.Message, ex, "") { }

        public SqlException(string message, Exception ex) : this(message, ex, "") { }

        public SqlException(Exception ex, ISqlSection sql) : this(ex.Message, ex, sql) { }

        protected SqlException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            _Sql = info.GetString("_Sql");
            _innerStackTrace = info.GetString("_innerStackTrace");
        }

        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.SerializationFormatter)]
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("_Sql", _Sql);
            info.AddValue("_innerStackTrace", _innerStackTrace);
        }
    }
}
