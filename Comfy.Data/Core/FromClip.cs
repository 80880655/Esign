using System.Collections.Generic;
using System.Text;

namespace Comfy.Data.Core
{
    public class FromClip
    {
        #region Protected Members

        protected readonly string tableOrViewName;
        protected readonly string aliasName;
        protected readonly Dictionary<string, KeyValuePair<string, WhereClip>> joins = new Dictionary<string, KeyValuePair<string, WhereClip>>();
        protected readonly List<JoinType> joinTypes = new List<JoinType>();

        #endregion

        #region Properties

        public string TableOrViewName
        {
            get
            {
                return tableOrViewName;
            }
        }

        public string AliasName
        {
            get
            {
                return aliasName;
            }
        }

        public Dictionary<string, KeyValuePair<string, WhereClip>> Joins
        {
            get
            {
                return joins;
            }
        }

        public List<JoinType> JoinTypes
        {
            get
            {
                return joinTypes;
            }
        }

        #endregion

        #region Constructors

        public FromClip(string tableOrViewName, string aliasName)
        {
            //Check.Require(!string.IsNullOrEmpty(tableOrViewName), "tableName could not be null or empty!");
            //Check.Require(!string.IsNullOrEmpty(aliasName), "aliasName could not be null or empty!");

            this.tableOrViewName = tableOrViewName;
            this.aliasName = aliasName;
        }

        public FromClip(string tableOrViewName)
            : this(tableOrViewName, tableOrViewName)
        {
        }

        #endregion

        #region Public Members

        public FromClip Join(string tableOrViewName, WhereClip onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip Join(string tableOrViewName, string aliasName, WhereClip onWhere)
        {
            //Check.Require(!string.IsNullOrEmpty(tableOrViewName), "tableName could not be null or empty!");
            //Check.Require(!string.IsNullOrEmpty(aliasName), "aliasName could not be null or empty!");
            //Check.Require(((object)onWhere) != null && onWhere.From == null, "onWhere could not be null, onWhere.From must be null in Join!");

            if (joins.ContainsKey(aliasName))
            {
                throw new NameDuplicatedException("In joins list: aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, WhereClip>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Inner);

            return this;
        }

        public FromClip LeftJoin(string tableOrViewName, WhereClip onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip LeftJoin(string tableOrViewName, string aliasName, WhereClip onWhere)
        {
            //Check.Require(!string.IsNullOrEmpty(tableOrViewName), "tableName could not be null or empty!");
            //Check.Require(!string.IsNullOrEmpty(aliasName), "aliasName could not be null or empty!");
            //Check.Require(((object)onWhere) != null && onWhere.From == null, "onWhere could not be null, onWhere.From must be null in Join!");

            if (joins.ContainsKey(aliasName))
            {
                throw new NameDuplicatedException("In joins list: duplicated aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, WhereClip>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Left);

            return this;
        }

        public FromClip RightJoin(string tableOrViewName, WhereClip onWhere)
        {
            return Join(tableOrViewName, tableOrViewName, onWhere);
        }

        public FromClip RightJoin(string tableOrViewName, string aliasName, WhereClip onWhere)
        {
            //Check.Require(!string.IsNullOrEmpty(tableOrViewName), "tableName could not be null or empty!");
            //Check.Require(!string.IsNullOrEmpty(aliasName), "aliasName could not be null or empty!");
            //Check.Require(((object)onWhere) != null && onWhere.From == null, "onWhere could not be null, onWhere.From must be null in Join!");

            if (joins.ContainsKey(aliasName))
            {
                throw new NameDuplicatedException("In joins list: duplicated aliasName - " + aliasName);
            }

            joins.Add(aliasName, new KeyValuePair<string, WhereClip>(tableOrViewName, onWhere));
            joinTypes.Add(JoinType.Right);

            return this;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            if (!tableOrViewName.Contains("["))
            {
                sb.Append('[');
                sb.Append(tableOrViewName);
                sb.Append(']');
            }
            else
                sb.Append(tableOrViewName);

            if (aliasName != tableOrViewName)
            {
                sb.Append(' ');
                sb.Append('[');
                sb.Append(aliasName.TrimStart('[').TrimEnd(']'));
                sb.Append(']');
            }

            int i = 0;
            foreach (string joinAliasName in joins.Keys)
            {
                string joinTypeStr;
                if (joinTypes[i] == JoinType.Inner)
                    joinTypeStr = "INNER JOIN";
                else if (joinTypes[i] == JoinType.Left)
                    joinTypeStr = "LEFT OUTER JOIN";
                else if (joinTypes[i] == JoinType.Right)
                    joinTypeStr = "RIGHT OUTER JOIN";
                else
                    joinTypeStr = "INNER JOIN";

                if (sb.ToString().Contains(joinTypeStr))
                {
                    sb = new StringBuilder('(' + sb.ToString() + ')');
                }

                KeyValuePair<string, WhereClip> keyWhere = joins[joinAliasName];
                sb.Append(' ');
                sb.Append(joinTypeStr);
                sb.Append(' ');
                if (!keyWhere.Key.Contains("["))
                {
                    sb.Append('[');
                    sb.Append(keyWhere.Key);
                    sb.Append(']');
                }
                else
                    sb.Append(keyWhere.Key);

                if (joinAliasName != keyWhere.Key)
                {
                    sb.Append(' ');
                    if (!joinAliasName.Contains("["))
                    {
                        sb.Append('[');
                        sb.Append(joinAliasName);
                        sb.Append(']');
                    }
                    else
                        sb.Append(joinAliasName);
                }
                sb.Append(" ON ");
                sb.Append(keyWhere.Value.ToString());

                ++i;
            }

            return sb.ToString();
        }

        #endregion
    }
}
