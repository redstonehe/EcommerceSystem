using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;

namespace DAL.Base
{
    public static class TypeConvert
    {
        #region db类型转换到C#类型
        public static int ToInt32(object dbValue)
        {
            if (dbValue == null || dbValue == DBNull.Value)
            {
                return int.MinValue;
            }
            return Convert.ToInt32(dbValue);
        }
        #endregion db类型转换到C#类型

        #region 将Type类型转换为SqlDbType
        public static System.Data.SqlDbType GetSqlDbType(System.Type sysType)
        {
            if (sysType.IsEnum)
            {
                return SqlDbType.Int;
            }
            else
            {
                return dicSqlDbType[sysType.Name];
            }
        }
        public static System.Data.SqlDbType GetSqlDbType(string sysType)
        {
            return dicSqlDbType[sysType];
        }
        #endregion 将Type类型转换为SqlDbType

        #region 将Type类型转换为DbType
        public static System.Data.DbType GetDbType(System.Type sysType)
        {
            return dicDbType[sysType.Name];
        }
        #endregion 将Type类型转换为DbType

        #region Dictionr缓存
        public static Dictionary<string, System.Data.DbType> dicDbType
        {
            get
            {
                Dictionary<string, DbType> _dic = System.Web.HttpRuntime.Cache.Get("ref_dicDbType") as Dictionary<string, DbType>;
                if (_dic == null)
                {
                    _dic = new Dictionary<string, DbType>();
                    _dic.Add("String", DbType.String);
                    _dic.Add("Int16", DbType.Int16);
                    _dic.Add("Int32", DbType.Int32);
                    _dic.Add("Int64", DbType.Int64);
                    _dic.Add("UInt64", DbType.UInt64);                   
                    _dic.Add("DateTime", DbType.DateTime);
                    _dic.Add("Double", DbType.Double);
                    _dic.Add("Boolean", DbType.Boolean);
                    _dic.Add("Guid", DbType.Guid);
                    _dic.Add("Byte", DbType.Byte);
                    _dic.Add("Decimal", DbType.Decimal);
                    System.Web.HttpRuntime.Cache.Insert("ref_dicDbType", _dic);
                }
                return _dic;
            }
        }

        public static Dictionary<string, System.Data.SqlDbType> dicSqlDbType
        {
            get
            {
                Dictionary<string, SqlDbType> _dic = System.Web.HttpRuntime.Cache.Get("ref_dicSqlDbType") as Dictionary<string, SqlDbType>;
                if (_dic == null)
                {
                    _dic = new Dictionary<string, SqlDbType>();
                    _dic.Add("String", SqlDbType.VarChar);
                    _dic.Add("Int16", SqlDbType.SmallInt);
                    _dic.Add("Int32", SqlDbType.Int);
                    _dic.Add("Int64", SqlDbType.BigInt);
                    _dic.Add("UInt64", SqlDbType.BigInt);                
                    _dic.Add("DateTime", SqlDbType.DateTime);
                    _dic.Add("Float", SqlDbType.Float);
                    _dic.Add("Double", SqlDbType.Float);
                    _dic.Add("Boolean", SqlDbType.Bit);
                    _dic.Add("Guid", SqlDbType.UniqueIdentifier);
                    _dic.Add("Byte", SqlDbType.TinyInt);
                    _dic.Add("Decimal", SqlDbType.Decimal);
                    System.Web.HttpRuntime.Cache.Insert("ref_dicSqlDbType", _dic);
                }
                return _dic;
            }
        }
        #endregion Diction缓存

        public static string GetPropValueField(string piName, string strValue)
        {
            string returnValue = "";
            switch (piName)
            {
                case "String":
                    returnValue = string.Format("'{0}'", strValue.Replace("'", "''"));
                    break;

                case "DateTime":
                case "Boolean":
                case "Guid":
                    returnValue = string.Format("'{0}'", strValue);
                    break;

                default:
                    returnValue = strValue;
                    break;
            }
            return returnValue;
        }
    }
}