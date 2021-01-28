using System.Collections.Generic;
using System.Data;

namespace Comfy.Data.Core
{
    public interface IExpression
    {
        string Sql { get; set; }
        Dictionary<string, KeyValuePair<DbType, object>> Parameters { get; }
    }
}
