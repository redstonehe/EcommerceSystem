using System;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using Maticsoft.DBUtility;
using DAL.Base;
using VMall.Core;
namespace VMall.Services
{
    //��Ϣ���ͱ�
    public partial class Informtype : BaseDAL<InformtypeInfo>
    {
        /// <summary>
        /// ����ɾ��һ������
        /// </summary>
        public bool DeleteList(string typeidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_informtype ");
            strSql.Append(" where typeid in (" + typeidlist + ")  ");
            int rows = DbHelperSQL.ExecuteSql(strSql.ToString());
            if (rows > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}