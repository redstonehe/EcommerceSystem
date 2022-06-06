using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;

using Entity.Base;
using Utility;
using VMall.Core;

namespace DAL.Base
{
    public abstract class BaseAccess<T> where T : class,new()
    {
        #region 属性
        /// <summary>
        /// 构造函数
        /// </summary>
        public BaseAccess() { }

        private string _PrimaryKey = null;
        /// <summary>
        /// 主键字段名
        /// </summary>
        public string PrimaryKey
        {
            get
            {
                if (_PrimaryKey == null)
                {
                    foreach (PropertyInfo pi in listProperty)
                    {
                        foreach (object obj in pi.GetCustomAttributes(false))
                        {
                            if (obj is SqlField)
                            {
                                if (((SqlField)obj).IsPrimaryKey)
                                {
                                    _PrimaryKey = pi.Name;
                                    return _PrimaryKey;
                                }
                            }
                        }
                    }
                }
                return _PrimaryKey;
            }
            set { _PrimaryKey = value; }
        }

        private T _t = default(T);
        /// <summary>
        /// Model层变量Ｔ的实例
        /// </summary>
        protected T t
        {
            get
            {
                if (_t == null)
                    _t = new T();
                return _t;
            }
        }

        private string _connName = "";
        /// <summary>
        /// 要连接的数据库名
        /// </summary> 
        public string connName
        {
            get
            {
                if (string.IsNullOrEmpty(_connName))
                {
                    _connName = attrTable.ConnName;
                }
                return _connName;
            }
            set { _connName = value; }
        }

        private string _TableName = "";
        /// <summary>
        /// 数据表名
        /// </summary>
        public virtual string TableName
        {
            get
            {
                if (_TableName == "")
                {
                    _TableName = typeof(T).Name;
                }
                //return BMAConfig.RDBSConfig.RDBSTablePre + _TableName.Replace("Info", "");
                return GetTableName();
            }
            set { _TableName = value; }
        }

        private long _classGuid = 0;
        public long classGuid
        {
            get
            {
                if (_classGuid == 0)
                {
                    _classGuid = CheckSum.ComputeCheckSum(string.Format("{0}_{1}", TableName, typeof(T).GUID));
                }
                return _classGuid;
            }
        }

        private int _ExpireTime = 0;
        /// <summary>
        /// memcache 缓存过期分钟数，默认为0不缓存
        /// </summary>
        public int ExpireTime
        {
            get { return _ExpireTime; }
            set { _ExpireTime = value; }
        }

        private bool _IsCacheModelWhere = false;
        /// <summary>
        /// 是否缓存GetModel(string strWhere)方法。慎用，可能会造成GetModel数据不准
        /// </summary>
        public bool IsCacheModelWhere
        {
            get { return _IsCacheModelWhere; }
            set { _IsCacheModelWhere = value; }
        }
        #endregion 属性

        private static string GetTableName()
        {
            var type = typeof(T);

            var attribute = type.GetCustomAttributes(typeof(SqlTable), false).FirstOrDefault();

            if (attribute == null)
            {
                return null;
            }

            return ((SqlTable)attribute).TableName;
        }

        #region 辅助函数
        protected virtual string GetAddUpdateSql(bool IsAdd)
        {
            StringBuilder strSql = new StringBuilder();
            if (IsAdd)
            {
                StringBuilder strParameter = new StringBuilder();
                strSql.Append(string.Format("insert into {0}(", TableName));
                PropertyInfo[] pis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                for (int i = 0; i < pis.Length; i++)
                {
                    if (attrPrimary.IsAutoId)
                    {
                        if (PrimaryKey == pis[i].Name)
                            continue;
                    }
                    strSql.Append(pis[i].Name + ","); //构造SQL语句前半部份 
                    strParameter.Append("@" + pis[i].Name + ","); //构造参数SQL语句
                }
                strSql = strSql.Replace(",", ")", strSql.Length - 1, 1);
                strParameter = strParameter.Replace(",", ")", strParameter.Length - 1, 1);
                strSql.Append(" values (");
                strSql.Append(strParameter.ToString());
            }
            else
            {
                strSql.Append("update  " + TableName + " set ");
                PropertyInfo[] pis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public);
                for (int i = 0; i < pis.Length; i++)
                {
                    if (pis[i].Name != PrimaryKey)
                    {
                        strSql.Append(pis[i].Name + "=" + "@" + pis[i].Name + ",");
                    }
                }
                strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
                strSql.Append(" where " + PrimaryKey + "=@" + PrimaryKey);
            }
            return strSql.ToString();
        }
        #endregion 辅助函数

        #region 参数缓存
        protected static System.Web.Caching.Cache cacheManager = System.Web.HttpRuntime.Cache;
        protected List<PropertyInfo> listProperty
        {
            get
            {
                string strKey = string.Format("dal_listProperty_{0}", classGuid);
                List<PropertyInfo> _list = cacheManager.Get(strKey) as List<PropertyInfo>;
                if (_list == null)
                {
                    PropertyInfo[] pis = typeof(T).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                    for (int i = 0; i < pis.Length; i++)
                    {
                        PropertyInfo pi = pis[i];
                        foreach (object obj in pi.GetCustomAttributes(false))
                        {
                            if (obj is SqlField)
                            {
                                if (((SqlField)obj).IsPrimaryKey)
                                {
                                    if (i == 0)
                                    {
                                        _PrimaryKey = pi.Name;
                                        _list = pis.ToList();
                                        cacheManager.Insert(strKey, _list);
                                        return _list;
                                    }
                                    else
                                    {   // 交换
                                        PropertyInfo pi0 = pis[0];
                                        pis[0] = pis[i];
                                        pis[i] = pi0;
                                        _PrimaryKey = pi.Name;
                                        _list = pis.ToList();
                                        cacheManager.Insert(strKey, _list);
                                        return _list;
                                    }
                                }
                            }
                        }
                    }
                    _list = pis.ToList();
                    cacheManager.Insert(strKey, _list);
                }
                return _list;
            }
        }

        public PropertyInfo[] arrProperty
        {
            get
            {
                string strKey = string.Format("dal_arrProperty_{0}", classGuid);
                PropertyInfo[] _arrProperty = cacheManager.Get(strKey) as PropertyInfo[];
                if (_arrProperty == null)
                {
                    List<PropertyInfo> _listPropertyInfo = new List<PropertyInfo>();

                    foreach (PropertyInfo pi in listProperty)
                    {
                        foreach (object obj in pi.GetCustomAttributes(false))
                        {
                            if (obj is SqlField)
                            {
                                _listPropertyInfo.Add(pi);
                                break;
                            }
                        }
                    }
                    _arrProperty = _listPropertyInfo.ToArray();
                    cacheManager.Insert(strKey, _arrProperty);
                }
                return _arrProperty;
            }
        }

        protected List<SqlField> attrListSqlAttribute
        {
            get
            {
                string strKey = string.Format("dal_listSqlAttribute_{0}", classGuid);
                List<SqlField> _listAttr = cacheManager.Get(strKey) as List<SqlField>;
                if (_listAttr == null)
                {
                    _listAttr = new List<SqlField>();

                    foreach (PropertyInfo pi in listProperty)
                    {
                        foreach (object obj in pi.GetCustomAttributes(false))
                        {
                            if (obj is SqlField)
                            {
                                _listAttr.Add((SqlField)obj);
                            }
                        }
                    }

                    if (_listAttr.Count == 0)
                    {
                        throw new Exception(string.Format("请在类{0}添加 SqlField定义", typeof(T).FullName));
                    }
                    _listAttr = _listAttr.OrderBy(p => p.Rank).ToList();
                    cacheManager.Insert(strKey, _listAttr);
                }
                return _listAttr;
            }
        }

        public bool IsAutoId
        {
            get
            {
                return attrPrimary.IsAutoId;
            }
            set
            {
                attrPrimary.IsAutoId = value;
            }
        }

        protected SqlField attrPrimary
        {
            get
            {
                string strKey = string.Format("dal_sqlPrimaryAttribute_{0}", classGuid);
                SqlField _attrPrimary = cacheManager.Get(strKey) as SqlField;
                if (_attrPrimary == null)
                {
                    try
                    {
                        _attrPrimary = attrListSqlAttribute.Single(p => p.IsPrimaryKey);
                    }
                    catch
                    {
                        throw new Exception(string.Format("请在类{0}添加 SqlField (IsPrimaryKey = true)定义", typeof(T).FullName));
                    }
                    cacheManager.Insert(strKey, _attrPrimary);
                }
                return _attrPrimary;
            }
        }

        protected SqlTable attrTable
        {
            get
            {
                string strKey = string.Format("dal_sqlConnAttribute_{0}", classGuid);
                SqlTable _attrTable = cacheManager.Get(strKey) as SqlTable;
                if (_attrTable == null)
                {
                    try
                    {
                        foreach (Attribute att in System.Attribute.GetCustomAttributes(typeof(T)))
                        {
                            if (att is SqlTable)
                            {
                                _attrTable = att as SqlTable;
                                break;
                            }
                        }
                        if (_attrTable == null)
                        {
                            throw new Exception();
                        }
                    }
                    catch
                    {
                        throw new Exception(string.Format("请在类{0}添加 SqlTable定义", typeof(T).FullName));
                    }
                    cacheManager.Insert(strKey, _attrTable);
                }
                return _attrTable;
            }
        }

        public Dictionary<string, PropertyInfo> dicProperty
        {
            get
            {
                string strKey = string.Format("dal_dicProperty_{0}", classGuid);
                Dictionary<string, PropertyInfo> _dic = cacheManager.Get(strKey) as Dictionary<string, PropertyInfo>;
                if (_dic == null)
                {
                    _dic = arrProperty.ToDictionary(p => p.Name.ToLower());
                    cacheManager.Insert(strKey, _dic);
                }
                return _dic;
            }
        }
        #endregion 参数缓存
    }
}
