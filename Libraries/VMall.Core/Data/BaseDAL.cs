using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Data.Common;
using System.Transactions;

//using Entity.Base;
using Utility;
using Utility.Text;

namespace DAL.Base
{
    public class BaseDAL<T> : BaseAccess<T> where T : class ,new()
    {
        public SqlTransaction Transaction { get; set; }

	      private string _currentExcutingSql = string.Empty;

        public string CurrentExcutingSql
        {
            get
            {
                string temp = _currentExcutingSql;
                _currentExcutingSql = string.Empty;
                return temp;
            }
            set
            {
                _currentExcutingSql = value;
            }
        }

        protected string fldListDefault
        {
            get
            {
                string strKey = string.Format("dal_fldListDefault_{0}", classGuid);
                string _fldList = cacheManager.Get(strKey) as string;
                if (string.IsNullOrEmpty(_fldList))
                {
                    _fldList = "";
                    foreach (PropertyInfo pi in arrProperty)
                    {
                        _fldList += "[" + pi.Name + "],";
                    }
                    _fldList = _fldList.TrimEnd(new char[] { ',' }).ToLower();
                    cacheManager.Insert(strKey, _fldList);
                }
                return _fldList;
            }
        }

        #region ����
        /// <summary>
        /// ���캯��
        /// </summary>
        public BaseDAL() { }

        #endregion ����

        #region BaseMethod�жϼ�¼�Ƿ����
        /// <summary>
        /// �жϼ�¼�Ƿ����
        /// </summary>
        /// <param name="id">��¼ID</param>
        public virtual bool Exists(int Id)
        {
            return Exists(string.Format("{0}=@{0}", PrimaryKey),
                          new SqlParameter[] { new SqlParameter() { ParameterName = PrimaryKey, SqlDbType = System.Data.SqlDbType.Int, SqlValue = Id } });
        }
        public virtual bool Exists(long Id)
        {
            return Exists(string.Format("{0}=@{0}", PrimaryKey),
                          new SqlParameter[] { new SqlParameter { ParameterName = PrimaryKey, SqlDbType = System.Data.SqlDbType.BigInt, SqlValue = Id } });
        }
        /// <summary>
        /// �жϼ�¼�Ƿ����
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        public virtual bool Exists(string strWhere)
        {
            return Exists(strWhere, null);
        }
        public virtual bool Exists(string strWhere, SqlParameter[] sqlParms)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("select count(1) from [{0}]", TableName));
            if (strWhere != "")
                strSql.Append(" where " + strWhere);

            object obj = SqlHelper.ExecuteScalar(connName, CommandType.Text, strSql.ToString(), sqlParms);
            int cmdresult;
            if ((Equals(obj, null)) || (Equals(obj, DBNull.Value)))
            {
                cmdresult = 0;
            }
            else
            {
                cmdresult = int.Parse(obj.ToString());
            }
            if (cmdresult == 0)
            {
                return false;
            }
            return true;
        }


        #endregion BaseMethod

        #region Deleteɾ����¼
        /// <summary>
        /// ɾ����¼
        /// </summary>
        /// <param name="id">��¼ID</param>
        public virtual int Delete(int Id)
        {
            return DeleteByWhere(string.Format("{0}={1}", PrimaryKey, Id));
        }
        public virtual int Delete(long Id)
        {
            return DeleteByWhere(string.Format("{0}={1}", PrimaryKey, Id));
        }
        public virtual int Delete(Guid Id)
        {
            return DeleteByWhere(string.Format("{0}='{1}'", PrimaryKey, Id));
        }

        /// <summary>
        /// ɾ����¼
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param> 
        public virtual int DeleteByWhere(string strWhere)
        {
            return DeleteByWhere(strWhere, null);
        }
        public virtual int DeleteByWhere(string strWhere, SqlParameter[] sqlParms)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("delete [{0}]", TableName));
            if (strWhere != "")
                strSql.Append(" where " + strWhere);
            int iReturn = 0;
            if (Transaction == null)
                iReturn = SqlHelper.ExecuteNonQuery(connName, CommandType.Text, strSql.ToString(), sqlParms);
            else
                iReturn = SqlHelper.ExecuteNonQuery(Transaction, CommandType.Text, strSql.ToString(), sqlParms);
            if (ExpireTime > 0)
            {
                // ������ʵ����ܰ汾�ż�1
                MemCacheHander.SetVersion(string.Format(BaseMemCacheKey.Vision_Model_All, MemCacheHander.GetValue_Model_All_Vision(classGuid)));
            }
            return iReturn;
        }
        #endregion

        #region GetCount��ü�¼��
        /// <summary>
        /// ��ü�¼��
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param> 
        public virtual int GetCount(string strWhere, params  SqlParameter[] sqlParms)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(string.Format("select count(0) from [{0}](nolock)", TableName));
            if (!string.IsNullOrEmpty(strWhere))
                strSql.Append(" where " + strWhere);
            return int.Parse(SqlHelper.ExecuteScalar(connName, CommandType.Text, strSql.ToString(), sqlParms).ToString());
        }
        #endregion

        #region Add
        /// <summary>
        /// ���ʵ��
        /// </summary>
        /// <param name="model">ʵ��</param>
        public virtual object Add(T model)
        {
            return Add(model, null, false);
        }
        /// <summary>
        /// ��Ӽ�¼�����ص�ǰ�����������¼��ID
        /// </summary>
        /// <param name="model">ʵ���ĳʵ���ʵ��</param>
        /// <param name="field">�����ֶ�</param>
        /// <returns></returns>
        public virtual object Add(T model, string field)
        {
            return Add(model, field, false);
        }
        /// <summary>
        /// ���ʵ��
        /// </summary>
        /// <param name="model">ʵ��</param>
        /// <param name="identity_insert">�Ƿ������Զ����</param>
        /// <returns></returns>
        public virtual object Add(T model, bool identity_insert)
        {
            return Add(model, null, identity_insert);
        }
        /// <summary>
        /// ���ʵ��
        /// </summary>
        /// <param name="model">ʵ��</param>
        /// <param name="field">�����ֶ�</param>
        /// <param name="identity_insert">�Ƿ������Զ����</param>
        /// <returns></returns>
        public virtual object Add(T model, string field, bool identity_insert)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append(string.Format("insert into [{0}](", TableName));
            PropertyInfo[] pis = arrProperty;
            SqlParameter[] listParam = null;

            List<PropertyInfo> listProperty = new List<PropertyInfo>();
            for (int i = 0; i < pis.Length; i++)
            {
                if (!string.IsNullOrEmpty(field))
                {
                    if (pis[i].Name == field)
                    {
                        continue;
                    }
                }
                listProperty.Add(pis[i]);
            }

            if (attrPrimary.IsAutoId)
            {
                listParam = new SqlParameter[pis.Length - 1];
                for (int i = 1; i < pis.Length; i++)
                {
                    strSql.Append(string.Format("[{0}],", pis[i].Name)); //����SQL���ǰ�벿�� 
                    strParameter.Append("@" + pis[i].Name + ","); //�������SQL���

                    listParam[i - 1] = new SqlParameter();
                    listParam[i - 1].ParameterName = pis[i].Name;
                    // listParam[i - 1].SqlDbType = TypeConvert.GetSqlDbType(pis[i].PropertyType);
                    listParam[i - 1].Value = pis[i].GetValue(model, null);
                }
            }
            else
            {
                listParam = new SqlParameter[listProperty.Count];
                int j = 0;
                foreach (PropertyInfo pi in listProperty)
                {
                    strSql.Append(string.Format("[{0}],", pi.Name)); //����SQL���ǰ�벿�� 
                    strParameter.Append("@" + pi.Name + ","); //�������SQL���

                    listParam[j] = new SqlParameter();
                    listParam[j].ParameterName = pi.Name;
                    //listParam[j].SqlDbType = TypeConvert.GetSqlDbType(pi.PropertyType);

                    //�ж��Ƿ�Ϊ������guidֵ�Ƿ�Ϊ��
                    if (j == 0)
                    {
                        if (pi.GetValue(model, null).ToString() == Guid.Empty.ToString())//Ϊ������һ��guid
                        {
                            listParam[j].Value = Guid.NewGuid();
                        }
                        else
                        {
                            listParam[j].Value = pi.GetValue(model, null);
                        }
                    }
                    else
                    {
                        listParam[j].Value = pi.GetValue(model, null);
                    }
                    j++;
                }
            }
            strSql = strSql.Replace(",", ")", strSql.Length - 1, 1);
            strParameter = strParameter.Replace(",", ")", strParameter.Length - 1, 1);
            strSql.Append(" values (");
            strSql.Append(strParameter + ";");
            if (attrPrimary.IsAutoId)
            {
                switch (pis[0].PropertyType.Name)
                {
                    case "Int16":
                    case "Int32":
                    case "Int64":
                        strSql.Append("select SCOPE_IDENTITY();");
                        break;
                }
            }
            if (identity_insert)
            {
                strSql = new StringBuilder().Append(string.Format("set identity_insert {0} on ", TableName))
                    .AppendLine(strSql.ToString())
                    .AppendLine(string.Format(" set identity_insert {0} off", TableName));
            }
            if (Transaction == null)
                return SqlHelper.ExecuteScalar(connName, CommandType.Text, strSql.ToString(), listParam);
            else
                return SqlHelper.ExecuteScalar(Transaction, CommandType.Text, strSql.ToString(), listParam);
        }
        #endregion Add

        #region Update
        /// <summary>
        /// ���¼�¼
        /// </summary>
        /// <param name="model">��Ҫ���µ����ݿ��ʵ����</param> 
        public virtual bool Update(T model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;
            SqlParameter[] listParam = new SqlParameter[pis.Length];
            for (int i = 1; i < pis.Length; i++)
            {
                strSql.Append(string.Format("[{0}]=@{0},", pis[i].Name));
                listParam[i - 1] = new SqlParameter();
                listParam[i - 1].ParameterName = pis[i].Name;
                listParam[i - 1].SqlDbType = TypeConvert.GetSqlDbType(pis[i].PropertyType);
                listParam[i - 1].Value = pis[i].GetValue(model, null);
                if (listParam[i - 1].SqlDbType == SqlDbType.VarChar)
                {
                    listParam[i - 1].Size = -1;
                    if (listParam[i - 1].Value == null) ��// �ַ���Ϊnullʱ�ᱨδ�ṩ�����Ĵ���
                    {
                        listParam[i - 1].Value = "";
                    }
                }
            }

            listParam[pis.Length - 1] = new SqlParameter();
            listParam[pis.Length - 1].ParameterName = pis[0].Name;
            listParam[pis.Length - 1].SqlDbType = TypeConvert.GetSqlDbType(pis[0].PropertyType);
            listParam[pis.Length - 1].Value = pis[0].GetValue(model, null);

            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            strSql.Append(" where [" + PrimaryKey + "]=@" + PrimaryKey);
            bool blReturn = SqlHelper.ExecuteNonQuery(connName, CommandType.Text, strSql.ToString(), listParam) > 0 ? true : false;

            if (ExpireTime > 0)
            {
                string strkey = string.Format(BaseMemCacheKey.Vision_Model_Id, MemCacheHander.GetValue_Model_All_Vision(classGuid), pis[0].GetValue(model, null));
                MemCacheHander.SetVersion(strkey);
            }
            return blReturn;
        }



        /// <summary>
        /// ���¼�¼
        /// </summary>
        /// <param name="model">��Ҫ���µ����ݿ��ʵ����</param> 
        /// <param name="fldList">��Ҫ���µ��ֶ�</param>
        /// <returns></returns>
        public virtual bool Update(T model, string fldList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;
            List<SqlParameter> listParam = new List<SqlParameter>();


            if (string.IsNullOrEmpty(fldList))
            {
                throw new Exception(string.Format("Ҫ���µ��ֶ�Ϊ��", fldList));
            }
            List<string> listField = fldList.Replace(" ", "").Replace("\r\n", "").ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 1; i < pis.Length; i++)
            {
                if (!listField.Any(p => p == pis[i].Name.ToLower()))
                {
                    continue;
                }

                strSql.Append(string.Format("[{0}]=@{0},", pis[i].Name));
                SqlParameter par = new SqlParameter();
                par.ParameterName = pis[i].Name;
                //par.SqlDbType = TypeConvert.GetSqlDbType(pis[i].PropertyType);
                par.Value = pis[i].GetValue(model, null);
                //if (par.SqlDbType == SqlDbType.VarChar)
                //{
                //    par.Size = -1;
                //    if (par.Value == null) ��// �ַ���Ϊnullʱ�ᱨδ�ṩ�����Ĵ���
                //    {
                //        par.Value = "";
                //    }
                //}
                listParam.Add(par);
            }

            SqlParameter primaryKeyPar = new SqlParameter();
            primaryKeyPar.ParameterName = pis[0].Name;
            //primaryKeyPar.SqlDbType = TypeConvert.GetSqlDbType(pis[0].PropertyType);
            primaryKeyPar.Value = pis[0].GetValue(model, null);
            if (CString.ProcessSqlStr(primaryKeyPar.Value.ToString()))
            {
                throw new Exception(string.Format("Ҫ���µ��ֶ�:{0}, ����ע�����", fldList));
            }


            listParam.Add(primaryKeyPar);

            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            strSql.Append(" where [" + PrimaryKey + "]=@" + PrimaryKey);
            CurrentExcutingSql = strSql.ToString();
            bool blReturn = SqlHelper.ExecuteNonQuery(connName, CommandType.Text, strSql.ToString(), listParam.ToArray()) > 0 ? true : false;

            if (ExpireTime > 0)
            {
                string strkey = string.Format(BaseMemCacheKey.Vision_Model_Id, MemCacheHander.GetValue_Model_All_Vision(classGuid), pis[0].GetValue(model, null));
                MemCacheHander.SetVersion(strkey);
            }
            return blReturn;
        }
        /// <summary>
        /// ���¼�¼
        /// </summary>
        /// <param name="model">��Ҫ���µ����ݿ��ʵ����</param> 
        /// <param name="fldList">��Ҫ���µ��ֶ�</param>
        /// <param name="Condition">��Ҫ���µ�����</param>
        /// <returns></returns>
        public virtual bool Update(T model, string fldList, string Condition)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;
            List<SqlParameter> listParam = new List<SqlParameter>();

            if (string.IsNullOrEmpty(fldList))
            {
                throw new Exception(string.Format("Ҫ���µ��ֶ�Ϊ��", fldList));
            }
            List<string> listField = fldList.Replace(" ", "").Replace("\r\n", "").ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            for (int i = 1; i < pis.Length; i++)
            {
                if (!listField.Any(p => p == pis[i].Name.ToLower()))
                {
                    continue;
                }

                strSql.Append(string.Format("[{0}]=@{0},", pis[i].Name));
                SqlParameter par = new SqlParameter();
                par.ParameterName = pis[i].Name;
                //par.SqlDbType = TypeConvert.GetSqlDbType(pis[i].PropertyType);
                par.Value = pis[i].GetValue(model, null);

                if (CString.ProcessSqlStr(par.Value.ToString()))
                {
                    throw new Exception(string.Format("Ҫ���µ��ֶ�:{0}, ����ע�����", fldList));
                }

                //if (par.SqlDbType == SqlDbType.VarChar)
                //{
                //    par.Size = -1;
                //    if (par.Value == null) ��// �ַ���Ϊnullʱ�ᱨδ�ṩ�����Ĵ���
                //    {
                //        par.Value = "";
                //    }
                //}
                listParam.Add(par);
            }
            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            if (string.IsNullOrEmpty(Condition))
            {
                SqlParameter primaryKeyPar = new SqlParameter();
                primaryKeyPar.ParameterName = pis[0].Name;
                //primaryKeyPar.SqlDbType = TypeConvert.GetSqlDbType(pis[0].PropertyType);
                primaryKeyPar.Value = pis[0].GetValue(model, null);
                listParam.Add(primaryKeyPar);

                strSql.Append(" where [" + PrimaryKey + "]=@" + PrimaryKey);
            }
            else
                strSql.Append(" where " + Condition + "");
            bool blReturn = false;
            CurrentExcutingSql = strSql.ToString();
            // bool blReturn = SqlHelper.ExecuteNonQuery(connName, strSql.ToString(), CommandType.Text, listParam.ToArray()) > 0 ? true : false;
            if (Transaction == null)
                blReturn = SqlHelper.ExecuteNonQuery(connName, CommandType.Text, strSql.ToString(), listParam.ToArray()) > 0 ? true : false;
            else
                blReturn = SqlHelper.ExecuteNonQuery(Transaction, CommandType.Text, strSql.ToString(), listParam.ToArray()) > 0 ? true : false;
            if (ExpireTime > 0)
            {
                string strkey = string.Format(BaseMemCacheKey.Vision_Model_Id, MemCacheHander.GetValue_Model_All_Vision(classGuid), pis[0].GetValue(model, null));
                MemCacheHander.SetVersion(strkey);
            }
            return blReturn;
        }

        /// <summary>
        /// ���¼�¼
        /// </summary>
        /// <param name="model">��Ҫ���µ����ݿ��ʵ����</param> 
        /// <param name="field">����Ҫ���µ��ֶ�,����ö��Ÿ���</param>
        /// <returns></returns>
        public virtual bool UpdatePassField(T model, string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                return Update(model);
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;
            List<SqlParameter> listParam = new List<SqlParameter>();

            if (CString.ProcessSqlStr(field))
            {
                throw new Exception(string.Format("Ҫ���µ��ֶ�:{0}, ����ע�����", field));
            }
            List<string> listField = field.Replace(" ", "").Replace("\r\n", "").ToLower().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int i = 1; i < pis.Length; i++)
            {
                if (!listField.Any(p => p != pis[i].Name.ToLower()))
                {
                    continue;
                }

                strSql.Append(string.Format("[{0}]=@{0},", pis[i].Name));
                SqlParameter par = new SqlParameter();
                par.ParameterName = pis[i].Name;
                //par.SqlDbType = TypeConvert.GetSqlDbType(pis[i].PropertyType);
                par.Value = pis[i].GetValue(model, null);
                //if (par.SqlDbType == SqlDbType.VarChar)
                //{
                //    par.Size = -1;
                //    if (par.Value == null) ��// �ַ���Ϊnullʱ�ᱨδ�ṩ�����Ĵ���
                //    {
                //        par.Value = "";
                //    }
                //}
                listParam.Add(par);
            }

            SqlParameter primaryKeyPar = new SqlParameter();
            primaryKeyPar.ParameterName = pis[0].Name;
            //primaryKeyPar.SqlDbType = TypeConvert.GetSqlDbType(pis[0].PropertyType);
            primaryKeyPar.Value = pis[0].GetValue(model, null);
            listParam.Add(primaryKeyPar);

            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            strSql.Append(" where [" + PrimaryKey + "]=@" + PrimaryKey);
            bool blReturn = SqlHelper.ExecuteNonQuery(connName, CommandType.Text, strSql.ToString(), listParam.ToArray()) > 0 ? true : false;
            CurrentExcutingSql = strSql.ToString();
            if (ExpireTime > 0)
            {
                string strkey = string.Format(BaseMemCacheKey.Vision_Model_Id, MemCacheHander.GetValue_Model_All_Vision(classGuid), pis[0].GetValue(model, null));
                MemCacheHander.SetVersion(strkey);
            }
            return blReturn;
        }

        #endregion Update

        #region Transaction
        //public virtual bool ExecuteTransaction(string cmdText)
        //{
        //    return SqlHelper.ExecuteTransaction(connName, cmdText);
        //}

        //public virtual bool ExecuteProc(string cmdText)
        //{
        //    return SqlHelper.ExecuteProc(connName, cmdText);
        //}

        //public virtual bool ExecuteTransaction(string[] cmdTexts)
        //{
        //    return SqlHelper.ExecuteTransaction(connName, cmdTexts, null, null);
        //}
        #endregion

        #region GetModel
        /// <summary>
        /// ���һ��Model����ʵ��
        /// </summary>
        /// <param name="id">����ID��ֵ</param>
        public virtual T GetModel(int Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id);
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                     , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@" + PrimaryKey, Value = Id } }
                                     , null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }

        public virtual T GetModel(long Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id.ToString());
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                     , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@" + PrimaryKey, Value = Id } }
                                     , null, null);

                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }

        public virtual T GetModel(Guid Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id.ToString());
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                    , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@" + PrimaryKey, Value = Id } }
                                    , null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }

        /// <summary>
        /// ���ڻ�ȡ�ַ���Ϊ������ʵ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual T GetModelByStringId(string Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id);
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                     , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@" + PrimaryKey, Value = Id } }
                                     , null, null);

                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModel(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.VarChar, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }

        /// <summary>
        /// ���һ��Model����ʵ��
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        public virtual T GetModel(string strWhere)
        {
            if (ExpireTime > 0 && IsCacheModelWhere)
            {
                string strkey = MemCacheHander.GetKey_Model_Where(classGuid, CheckSum.ComputeCheckSum(strWhere));
                object objId = MemCacheHander.GetValue(strkey);
                if (objId == null)
                {
                    T model = GetModel(strWhere, null, null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, arrProperty[0].GetValue(model, null).ToString(), DateTime.Now.AddMinutes(ExpireTime));
                        strkey = MemCacheHander.GetKey_Model_Value(classGuid, arrProperty[0].GetValue(model, null).ToString());
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                    return model;
                }
                else
                {
                    return GetModel(int.Parse(objId.ToString()));
                }
            }
            else
            {
                return GetModel(strWhere, null, null, null);
            }
        }

        public virtual T GetModel(string strWhere, SqlParameter[] listPm)
        {
            if (ExpireTime > 0 && IsCacheModelWhere)
            {
                string _strTemp = strWhere;
                if (listPm != null && listPm.Length > 0)
                {
                    foreach (SqlParameter pm in listPm)
                    {
                        _strTemp += string.Format("{0}={1}", pm.ParameterName, pm.Value.ToString());
                    }
                }
                string strkey = MemCacheHander.GetKey_Model_Where(classGuid, CheckSum.ComputeCheckSum(_strTemp));
                object objId = MemCacheHander.GetValue(strkey);
                if (objId == null)
                {
                    T model = GetModel(strWhere, listPm, null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, arrProperty[0].GetValue(model, null).ToString(), DateTime.Now.AddMinutes(ExpireTime));
                        strkey = MemCacheHander.GetKey_Model_Value(classGuid, arrProperty[0].GetValue(model, null).ToString());
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                    return model;
                }
                else
                {
                    return GetModel(int.Parse(objId.ToString()));
                }
            }
            else
            {
                return GetModel(strWhere, listPm, null, PrimaryKey + " asc");
            }
        }

        public virtual T GetModel(string strWhere, SqlParameter[] listPm, string colField, string sortField)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(string.Format("select top 1 {0} from [{1}] WITH (NOLOCK)", GetSqlFieldList(colField), TableName));

            //if (!NotInject(strWhere))
            //{
            //    return null;
            //} 

            if (!string.IsNullOrEmpty(strWhere))
                strSql.Append("  where " + strWhere);
            if (string.IsNullOrEmpty(sortField))
                sortField = PrimaryKey + " asc";

            strSql.Append(string.Format(" order by {0}", sortField));
            CurrentExcutingSql = strSql.ToString();
            T model = default(T);
            using (SqlDataReader dr = SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql.ToString(), listPm))
            {
                if (dr.Read())
                    model = GetModel(dr);
            }
            return model;
        }

        /// <summary>
        /// ���һ��Model����ʵ��
        /// </summary>
        public virtual T GetModel(SqlDataReader dr)
        {
            T model = new T();

            int fieldCount = dr.FieldCount;
            string fieldName;

            Dictionary<string, PropertyInfo> dic = dicProperty;
            for (int i = 0; i < fieldCount; i++)
            {
                if (dr.IsDBNull(i))
                    continue;
                fieldName = dr.GetName(i);
                if (dic.ContainsKey(fieldName))
                {
                    dic[fieldName].SetValue(model, dr.GetValue(i), null);
                }
                else
                {
                    fieldName = fieldName.ToLower();
                    if (dic.ContainsKey(fieldName))
                    {
                        dic[fieldName].SetValue(model, dr.GetValue(i), null);
                    }
                }
            }
            return GetModel(model);
        }

        public virtual T GetModelBySql(string strSql)
        {
            T model = new T();
            using (SqlDataReader dr = SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, null))
            {
                if (dr.Read())
                    model = GetModel(dr);
            }
            return model;
        }

        public virtual T GetModel(T model)
        {
            return model;
        }
        #endregion GetModel

        #region GetModelLock
        /// <summary>
        /// ���һ��Model����ʵ��--����
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        public virtual T GetModelLock(int Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id);
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                     , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@" + PrimaryKey, Value = Id } }
                                     , null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.Int, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }
        public virtual T GetModelLock(long Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id.ToString());
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                     , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@" + PrimaryKey, Value = Id } }
                                     , null, null);

                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.BigInt, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }
        public virtual T GetModelLock(Guid Id)
        {
            if (ExpireTime > 0)
            {
                string strkey = MemCacheHander.GetKey_Model_Value(classGuid, Id.ToString());
                T model = MemCacheHander.GetValue(strkey) as T;
                if (model == null)
                {
                    model = GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                    , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@" + PrimaryKey, Value = Id } }
                                    , null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                }
                return model;
            }
            else
            {
                return GetModelLock(string.Format("[{0}]=@{0}", PrimaryKey)
                                , new SqlParameter[] { new SqlParameter() { SqlDbType = SqlDbType.UniqueIdentifier, ParameterName = "@" + PrimaryKey, Value = Id } }
                                , null, null);
            }
        }
        public virtual T GetModelLock(string strWhere)
        {
            if (ExpireTime > 0 && IsCacheModelWhere)
            {
                string strkey = MemCacheHander.GetKey_Model_Where(classGuid, CheckSum.ComputeCheckSum(strWhere));
                object objId = MemCacheHander.GetValue(strkey);
                if (objId == null)
                {
                    T model = GetModelLock(strWhere, null, null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, arrProperty[0].GetValue(model, null).ToString(), DateTime.Now.AddMinutes(ExpireTime));
                        strkey = MemCacheHander.GetKey_Model_Value(classGuid, arrProperty[0].GetValue(model, null).ToString());
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                    return model;
                }
                else
                {
                    return GetModelLock(int.Parse(objId.ToString()));
                }
            }
            else
            {
                return GetModelLock(strWhere, null, null, null);
            }
        }
        public virtual T GetModelLock(string strWhere, SqlParameter[] listPm)
        {
            if (ExpireTime > 0 && IsCacheModelWhere)
            {
                string _strTemp = strWhere;
                if (listPm != null && listPm.Length > 0)
                {
                    foreach (SqlParameter pm in listPm)
                    {
                        _strTemp += string.Format("{0}={1}", pm.ParameterName, pm.Value.ToString());
                    }
                }
                string strkey = MemCacheHander.GetKey_Model_Where(classGuid, CheckSum.ComputeCheckSum(_strTemp));
                object objId = MemCacheHander.GetValue(strkey);
                if (objId == null)
                {
                    T model = GetModelLock(strWhere, listPm, null, null);
                    if (model != null)
                    {
                        MemCacheHander.SetValue(strkey, arrProperty[0].GetValue(model, null).ToString(), DateTime.Now.AddMinutes(ExpireTime));
                        strkey = MemCacheHander.GetKey_Model_Value(classGuid, arrProperty[0].GetValue(model, null).ToString());
                        MemCacheHander.SetValue(strkey, model, DateTime.Now.AddMinutes(ExpireTime));
                    }
                    return model;
                }
                else
                {
                    return GetModelLock(int.Parse(objId.ToString()));
                }
            }
            else
            {
                return GetModelLock(strWhere, listPm, null, PrimaryKey + " asc");
            }
        }
        public virtual T GetModelLock(string strWhere, SqlParameter[] listPm, string colField, string sortField)
        {
            StringBuilder strSql = new StringBuilder();

            strSql.Append(string.Format("select top 1 {0} from [{1}] ", GetSqlFieldList(colField), TableName));
            if (!string.IsNullOrEmpty(strWhere))
                strSql.Append("  where " + strWhere);
            if (string.IsNullOrEmpty(sortField))
                sortField = PrimaryKey + " asc";

            strSql.Append(string.Format(" order by {0}", sortField));

            T model = default(T);
            using (SqlDataReader dr = SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql.ToString(), listPm))
            {
                if (dr.Read())
                    model = GetModel(dr);
            }
            return model;
        }
        #endregion GetModelLock

        #region  GetList

        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <returns></returns>
        public virtual List<T> GetList( params SqlParameter[] arrParam)
        {
            return GetListByParam(null, arrParam, -1, 1, "*", PrimaryKey, TableName);
        }

        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        public virtual List<T> GetList(string strWhere, params SqlParameter[] arrParam)
        {
            return GetListByParam(strWhere, arrParam, -1, 1, "*", PrimaryKey + " asc", TableName);
        }
       
        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        public virtual List<T> GetList(string strWhere, int pageSize, int pageIndex, params SqlParameter[] arrParam)
        {
            return GetListByParam(strWhere, arrParam, pageSize, pageIndex, "*", PrimaryKey, TableName);
        }


        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        /// <param name="bOrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        public virtual List<T> GetList(string strWhere, int pageSize, int pageIndex, string fldList, params SqlParameter[] arrParam)
        {
            return GetListByParam(strWhere, arrParam, pageSize, pageIndex, fldList, PrimaryKey, TableName);
        }

        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        /// <param name="bOrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�</param>
        public virtual List<T> GetList(string strWhere, int pageSize, int pageIndex, string fldList, string fldOrder, params SqlParameter[] arrParam)
        {
            return GetListByParam(strWhere, arrParam, pageSize, pageIndex, fldList, fldOrder, TableName);
        } 
        

        /// <summary>
        /// ���List����
        /// </summary>
        /// <param name="dr">��DataReader���ʵ��ת��List</param>
        public virtual List<T> GetList(SqlDataReader dr )
        {
            List<T> list = new List<T>();
            while (dr.Read())
            {
                list.Add(GetModel(dr));
            }
            return list;
        }

        public virtual List<T> GetListBySql(string strSql, params SqlParameter[] parm)
        {
            //SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, null);
            using (SqlDataReader dr = SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, parm))
            {
                return GetList(dr);
            }
        } 
        #endregion  GetList

        #region  GetListByParam 
        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        /// <param name="bOrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�</param>
        /// <param name="sTableName">����</param>
        private List<T> GetListByParam(string strWhere, SqlParameter[] parm, int pageSize, int pageIndex, string fldList, string fldOrder, string sTableName)
        {
            using (SqlDataReader dr = GetDataReaderByPage(strWhere, parm, pageSize, pageIndex, fldList, fldOrder, TableName))
            {
                return GetList(dr);
            }
        }
        #endregion  GetListByParam

        #region  GetReader
        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        /// <param name="bOrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�</param>
        public virtual SqlDataReader GetReader(string strWhere, int pageSize, int pageIndex, string fldList, string fldOrder)
        {
            return GetReader(strWhere, pageSize, pageIndex, fldList, fldOrder, TableName);
        }

        internal SqlDataReader GetReader(string strWhere, int pageSize, int pageIndex, string fldList, string fldOrder, string sTableName)
        {
            return GetDataReaderByPage(strWhere, null, pageSize, pageIndex, fldList, fldOrder, TableName);
        }

        /// <summary>
        /// ��DataReaderת����DataTable
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        public DataTable ConvertDataReaderToDataTable(IDataReader reader)
        {
            return SqlHelper.ConvertDataReaderToDataTable(reader);
        }

        #endregion  GetReader

        #region GetTop
        public virtual List<T> GetTop(int topNum)
        {
            return GetTopByParam(topNum, null, null, "*", null, TableName);
        }
        public virtual List<T> GetTop(int topNum, params SqlParameter[] arrParam)
        {
            return GetTopByParam(topNum, null, arrParam, "*", null, TableName);
        }
        public virtual List<T> GetTop(int topNum, string strWhere, params SqlParameter[] arrParam)
        {
            return GetTopByParam(topNum, strWhere, arrParam, "*", null, TableName);
        }
        public virtual List<T> GetTop(int topNum, string strWhere, string fldList, params SqlParameter[] arrParam)
        {
            return GetTopByParam(topNum, strWhere, arrParam, fldList, null, TableName);
        }
        public virtual List<T> GetTop(int topNum, string strWhere, string fldList, string fldOrder, params SqlParameter[] arrParam)
        {
            return GetTopByParam(topNum, strWhere, arrParam, fldList, fldOrder, TableName);
        }
        
        #endregion

        #region  GetTopByParam
        /// <summary>
        /// ������ݼ�
        /// </summary>
        /// <param name="topNum">ȡǰtopNum������ </param>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="bOrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�</param>
        /// <param name="sTableName">����</param>
        private List<T> GetTopByParam(int topNum,string strWhere, SqlParameter[] parm, string fldList, string fldOrder, string sTableName)
        {
            using (SqlDataReader dr = GetTopDataReader(topNum, strWhere, parm, fldList, fldOrder, TableName))
            {
                return GetList(dr);
            }
        }
        #endregion  GetTopByParam

        #region top��ѯ
        /// <param name="topNum">ȡǰtopNum������ </param>
        /// <param name="strWhere">���SQL����Where�Ӿ�</param>
        /// <param name="parm">��ѯ����</param>
        /// <param name="fldList">���ŷָ����ֶ����ַ���</param>
        /// <param name="fldOrder">�����ֶ�</param>
        /// <param name="tblName">����</param>
        private SqlDataReader GetTopDataReader(int topNum, string strWhere, SqlParameter[] parm, string fldList, string fldOrder, string tblName)
        {
            string strSql = BuildGetTopSql(topNum, strWhere, fldList, fldOrder, tblName);
            CurrentExcutingSql = strSql.ToString();
            return SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, parm);
        }
        /// <param name="topNum">ȡǰtopNum������ </param>
        /// <param name="strWhere">���SQL����Where�Ӿ�</param>
        /// <param name="fldList">���ŷָ����ֶ����ַ���</param>
        /// <param name="fldOrder">�����ֶ�</param>
        /// <param name="tblName">����</param>
        public virtual string BuildGetTopSql(int topNum, string strWhere, string fldList, string fldOrder, string tblName)
        {
            string strKey = string.Format("dal_GetListSql_{0}", CheckSum.ComputeCheckSum(string.Format("{0}_{1}_{2}_{3}_{4}",
               classGuid, strWhere, fldList, fldOrder, tblName)));

            string strSql = cacheManager.Get(strKey) as string;
            if (string.IsNullOrEmpty(strSql))
            {
                string sColList = GetSqlFieldList(fldList);
                StringBuilder sbSql = new StringBuilder();
                string strOrder; // -- ��������
                if (string.IsNullOrEmpty(fldOrder))//û�������оͲ���������
                {
                    strOrder = string.Empty;
                }
                else
                {
                    strOrder = string.Format(" order by {0}", fldOrder);
                }

                if (string.IsNullOrEmpty(strWhere))
                {
                    sbSql.Append(string.Format("select top {0} {1} from [{2}] WITH (NOLOCK) ", topNum, sColList, tblName));
                    sbSql.AppendLine(strOrder);
                }
                else
                {
                    sbSql.Append(string.Format("select top {0} {1} from [{2}] WITH (NOLOCK) ", topNum, sColList, tblName));
                    sbSql.Append(string.Format(" where {0}", strWhere));
                    sbSql.AppendLine(strOrder);

                }
                strSql = sbSql.ToString();
                cacheManager.Insert(strKey, strSql);
            }
            return strSql;
        }
        #endregion

        #region ��ҳ��ѯ

        /// <summary>
        /// ���ݷ�ҳ��Ϣ��ȡ��¼��DataReader
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��¼��</param>
        /// <param name="pageIndex">��ǰҳ��</param>
        /// <param name="OrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ������</param>
        /// <param name="fldOrder">�����ֶ�����</param>
        /// <param name="tblName">����</param>
        private SqlDataReader GetDataReaderByPage(string strWhere, SqlParameter[] parm, int pageSize, int pageIndex, string fldList, string fldOrder, string tblName)
        {
            string strSql = BuildGetListSql(strWhere, pageSize, pageIndex, fldList, fldOrder, tblName);
						CurrentExcutingSql = strSql.ToString();
            return SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, parm);
        }

        /// <param name="strWhere">���SQL����Where�Ӿ�</param>
        /// <param name="pageSize">ÿҳ��ʾ��¼��</param>
        /// <param name="pageIndex">��ǰ��Ҫ��ȡ��ҳ��</param>
        /// <param name="OrderType">�������(true-����flase-����)</param>
        /// <param name="fldList">���ŷָ����ֶ����ַ���</param>
        /// <param name="fldOrder">�����ֶ�</param>
        /// <param name="tblName">����</param>
        public virtual string BuildGetListSql(string strWhere, int pageSize, int pageIndex, string fldList, string fldOrder, string tblName)
        {
            //if (!NotInject(strWhere))
            //{
            //    return null;
            //}

            if (pageIndex < 1)
            {
                pageIndex = 1;
            }
            if (pageSize < 1)
            {
                pageSize = 2000;	//Ĭ��ֵ��2000
            }
            string strKey = string.Format("dal_GetListSql_{0}", CheckSum.ComputeCheckSum(string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}",
               classGuid, strWhere, pageSize, pageIndex, fldList, fldOrder, tblName)));

            string strSql = cacheManager.Get(strKey) as string;
            if (string.IsNullOrEmpty(strSql))
            {
                string sColList = GetSqlFieldList(fldList);
                StringBuilder sbSql = new StringBuilder();
                string strOrder; // -- ��������
                if (string.IsNullOrEmpty(fldOrder))
                {
                    fldOrder = PrimaryKey;
                }
                strOrder = string.Format(" order by {0}", fldOrder);
                if (string.IsNullOrEmpty(strWhere))
                {
                    sbSql.Append(string.Format("select {0} from(select {1}, row_number() over({2}) as row from [{3}] WITH (NOLOCK) ", sColList, sColList, strOrder, tblName));
                    sbSql.Append(string.Format(") a  where row between {0} and {1}", (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));
                }
                else
                {
                    sbSql.Append(string.Format("select {0} from(select {1}, row_number() over({2}) as row from [{3}] WITH (NOLOCK) ", sColList, sColList, strOrder, tblName));
                    sbSql.Append(string.Format(" where {0}", strWhere));
                    sbSql.Append(string.Format(") a  where row between {0} and {1}", (pageIndex - 1) * pageSize + 1, pageIndex * pageSize));
                }
                strSql = sbSql.ToString();
                cacheManager.Insert(strKey, strSql);
            }
            return strSql;
        }        
          
        
        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string BuildAddSql(T model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strParameter = new StringBuilder();
            strSql.Append(string.Format("insert into [{0}](", TableName));
            PropertyInfo[] pis = arrProperty;

            if (!attrPrimary.IsAutoId)
            {
                strSql.Append(string.Format("[{0}],", pis[0].Name)); //����SQL���ǰ�벿��  
                //�ж��Ƿ�Ϊ������guidֵ�Ƿ�Ϊ��
                if (pis[0].GetValue(model, null).ToString() == Guid.Empty.ToString())//Ϊ������һ��guid
                {
                    Guid stnewid = Guid.NewGuid();
                    strParameter.Append(TypeConvert.GetPropValueField(pis[0].PropertyType.Name, stnewid.ToString()) + ","); //�������SQL���
                }
                else
                {
                    strParameter.Append(TypeConvert.GetPropValueField(pis[0].PropertyType.Name, pis[0].GetValue(model, null).ToString()) + ","); //�������SQL���
                }
            }

            for (int i = 1; i < pis.Length; i++)
            {
                string strValue = pis[i].GetValue(model, null).ToString();
                if (TypeConvert.GetSqlDbType(pis[i].PropertyType).Equals(SqlDbType.DateTime))
                {
                    strValue = Convert.ToDateTime(pis[i].GetValue(model, null)).ToString("yyyy-MM-dd HH:mm:ss.fff");
                }
                strSql.Append(string.Format("[{0}],", pis[i].Name)); //����SQL���ǰ�벿�� 
                strParameter.Append(TypeConvert.GetPropValueField(pis[i].PropertyType.Name,strValue) + ","); //�������SQL���
            }

            strSql = strSql.Replace(",", ")", strSql.Length - 1, 1);
            strParameter = strParameter.Replace(",", ")", strParameter.Length - 1, 1);
            strSql.Append(" values (");
            strSql.Append(strParameter + ";");
            return strSql.ToString();
        }

        /// <summary>
        /// �����������
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string BuildUpdateSql(T model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;

            for (int i = 1; i < pis.Length; i++)
            {
                strSql.Append(string.Format("[{0}]={1},", pis[i].Name, TypeConvert.GetPropValueField(pis[i].PropertyType.Name, pis[i].GetValue(model, null).ToString())));
            }
            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            strSql.Append(string.Format(" where [{0}]={1}", PrimaryKey, TypeConvert.GetPropValueField(pis[0].PropertyType.Name, pis[0].GetValue(model, null).ToString())));
            return strSql.ToString();
        }

        /// <summary>
        /// ���ݴ����ֶδ����������
        /// </summary>
        /// <param name="model"></param>
        /// <param name="fldList">�Զ��ŷָ��ĸ����ֶ�����</param>
        /// <returns></returns>
        public virtual string BuildUpdateSql(T model, string fldList)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update  [" + TableName + "] WITH (ROWLOCK)  set ");
            PropertyInfo[] pis = arrProperty;

            if (string.IsNullOrWhiteSpace(fldList))
            {
                for (int i = 1; i < pis.Length; i++)
                {
                    strSql.Append(string.Format("[{0}]={1},", pis[i].Name, TypeConvert.GetPropValueField(pis[i].PropertyType.Name, pis[i].GetValue(model, null).ToString())));
                }
            }
            else
            {
                string[] udpFld = fldList.Split(",".ToArray());
                for (int i = 1; i < pis.Length; i++)
                {
                    if (udpFld.Contains(pis[i].Name))
                        strSql.Append(string.Format("[{0}]={1},", pis[i].Name, TypeConvert.GetPropValueField(pis[i].PropertyType.Name, pis[i].GetValue(model, null).ToString())));
                }
            }
            strSql = strSql.Replace(",", " ", strSql.Length - 1, 1);
            strSql.Append(string.Format(" where [{0}]={1}", PrimaryKey, TypeConvert.GetPropValueField(pis[0].PropertyType.Name, pis[0].GetValue(model, null).ToString())));
            return strSql.ToString();
        }

        #endregion ��ҳ��ѯ
        #region �޷�ҳ��ѯ
        /// <summary>
        /// ������ݼ��޷�ҳ
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="arrParam">��ѯ����</param>
        public virtual List<T> GetListNoPage(string strWhere, params SqlParameter[] arrParam)
        {
            return GetListNoPage(strWhere,"*", string.Empty, arrParam);
        }

        /// <summary>
        /// ������ݼ��޷�ҳ
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="arrParam">��ѯ����</param>
        public virtual List<T> GetListNoPage(string strWhere, string fldList, params SqlParameter[] arrParam)
        {
            return GetListNoPage(strWhere, fldList, string.Empty, arrParam);
        }
        /// <summary>
        /// ������ݼ��޷�ҳ
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�+�������(desc-����asc-����)</param>
        /// <param name="arrParam">��ѯ����</param>
        public virtual List<T> GetListNoPage(string strWhere, string fldList, string fldOrder, params SqlParameter[] arrParam)
        {
            return GetListNoPageByParam(strWhere, arrParam, fldList, fldOrder, TableName);
        }
        /// <summary>
        /// ������ݼ��޷�ҳ
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ�ֶ�����</param>
        /// <param name="fldOrder">�����ֶ�+�������(desc-����asc-����)</param></param>
        /// <param name="sTableName">����</param>
        private List<T> GetListNoPageByParam(string strWhere, SqlParameter[] parm, string fldList, string fldOrder, string sTableName)
        {
            using (SqlDataReader dr = GetDataReaderNoPage(strWhere, parm, fldList, fldOrder, sTableName))
            {
                return GetList(dr);
            }
        }

        /// <summary>
        /// ���ݲ�ѯ��Ϣ��ȡ��¼��DataReaderû�з�ҳ
        /// </summary>
        /// <param name="strWhere">Where�Ӿ�</param>
        /// <param name="sqlParm">��ѯ�����Ӿ�</param>
        /// <param name="fldList">�Զ��ŷָ��Ĳ�ѯ������</param>
        /// <param name="fldOrder">�����ֶ�����+�������(true-����flase-����)</param>
        /// <param name="tblName">����</param>
        private SqlDataReader GetDataReaderNoPage(string strWhere, SqlParameter[] sqlParm, string fldList, string fldOrder, string tblName)
        {
            string strSql = BuildGetListSqlNoPage(strWhere, fldList, fldOrder, tblName);
            return SqlHelper.ExecuteDataReader(connName, CommandType.Text, strSql, sqlParm);
        }
        
        /// <summary>
        /// ����������Ϣ��ȡ��¼��DataReader�޷�ҳ
        /// </summary>
        /// <param name="strWhere">���SQL����Where�Ӿ�</param>
        /// <param name="fldList">���ŷָ����ֶ����ַ���</param>
        /// <param name="fldOrder">�����ֶ� �������(true-����flase-����)</param>
        /// <param name="tblName">����</param>
        public virtual string BuildGetListSqlNoPage(string strWhere, string fldList, string fldOrder, string tblName)
        {

            string strKey = string.Format("dal_GetListSql_{0}", CheckSum.ComputeCheckSum(string.Format("{0}_{1}_{2}_{3}_{4}",
               classGuid, strWhere, fldList, fldOrder, tblName)));

            string strSql = cacheManager.Get(strKey) as string;
            if (string.IsNullOrEmpty(strSql))
            {
                string sColList = GetSqlFieldList(fldList);
                StringBuilder sbSql = new StringBuilder();
                string strOrder; // -- ��������
                if (string.IsNullOrEmpty(fldOrder))//û�������оͲ���������
                {
                    strOrder = string.Empty;
                }
                else
                {
                    strOrder = string.Format(" order by {0}", fldOrder);
                }

                if (string.IsNullOrEmpty(strWhere))
                {
                    sbSql.Append(string.Format("select {0} from [{1}] WITH (NOLOCK) ", sColList, tblName));
                    sbSql.AppendLine(strOrder);
                }
                else
                {
                    sbSql.Append(string.Format("select {0} from [{1}] WITH (NOLOCK) ", sColList, tblName));
                    sbSql.Append(string.Format(" where {0}", strWhere));
                    sbSql.AppendLine(strOrder);

                }
                strSql = sbSql.ToString();
                cacheManager.Insert(strKey, strSql);
            }
            return strSql;
        }
        
        #endregion


        #region ��������
        private string GetSqlFieldList(string fldList)
        {
            if (string.IsNullOrEmpty(fldList) || fldList.Trim() == "*")
            {
                return fldListDefault;
            }
            else
            {
                string strKey = string.Format("dal_fldList_{0}", CheckSum.ComputeCheckSum(fldList));
                string _fldList = cacheManager.Get(strKey) as string;
                if (string.IsNullOrEmpty(_fldList))
                {
                    //��ȥ���ո�[]���� 
                    _fldList = fldList.Replace("[", "").Replace("]", "");
                    //Ϊ��ֹʹ�ñ����֣��������ֶμ���[]
                    //_fldList = "[" + _fldList + "]";
                    _fldList = _fldList.Replace('��', ',');
                    //_fldList = _fldList.Replace(",", "],[");
                    _fldList = _fldList.TrimEnd(new char[] { ',' }).ToLower();
                    cacheManager.Insert(strKey, _fldList);
                }
                return _fldList;
            }
        }


        //private bool NotInject(string strWhere)
        //{
        //    if (!string.IsNullOrWhiteSpace(strWhere))
        //    {
        //        if (CRegex.IsMatch(strWhere, @"=\s*[^@][\w\[]+"))
        //        {
        //            throw new Exception(strWhere + " ��sqlע����գ���ʹ�ô��η�ʽ");
        //        }
        //    }
        //    return true;
        //}
        #endregion ��������

        #region ������뼶��ʹ������ʱֱ�ӵ��ã�
        protected TransactionOptions TransactionOptions
        {
            get
            {
                TransactionOptions transactionOptions = new TransactionOptions();
                transactionOptions.IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted;
                transactionOptions.Timeout = new TimeSpan(0, 0, 60);

                return transactionOptions;
            }
        }
        #endregion

    }
}
