using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entity.Base
{
    [System.AttributeUsage(System.AttributeTargets.Property, AllowMultiple = false)]
    public class SqlField : System.Attribute
    {
        private bool _IsPrimaryKey = false;
        private bool _IsAutoId = false;
        private string _FieldName = "";
        private int _Rank = 10;
        private bool _IsNeedN = false;

        public SqlField()
        {

        }

        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }

        public bool IsAutoId
        {
            get { return _IsAutoId; }
            set { _IsAutoId = value; }
        }
        public string FieldName
        {
            get { return _FieldName; }
            set { _FieldName = value; }
        }
        public int Rank
        {
            get { return _Rank; }
            set { _Rank = value; }
        }
        public bool IsNeedN
        {
            get { return _IsNeedN; }
            set { _IsNeedN = value; }
        }
    }
}
