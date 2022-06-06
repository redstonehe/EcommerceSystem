using VMall.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace Entity.Base
{
    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = false)]
    public class SqlTable : System.Attribute
    {
        private static string _ConnName = BMAConfig.RDBSConfig.RDBSDBName;
        private string _TableName = "";

        public SqlTable(string tableName)
        {
            //this.ConnName = _ConnName;
            this._TableName = tableName;
        }
        //public SqlTable(string connName)
        //{
        //    this.ConnName = connName;
        //}
        public string ConnName
        {
            get { return _ConnName; }
            set { _ConnName = value; }
        }

        public string TableName
        {
            get { return _TableName; }
            set { _TableName = value; }
        } 
    }
}
