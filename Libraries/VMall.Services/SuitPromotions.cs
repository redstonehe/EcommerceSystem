using System;
using System.Text;
using System.Data;
using System.Collections.Generic;
using System.Data.SqlClient;
using VMall.Core;
using System.Linq;
using Maticsoft.DBUtility;//Please add references
using DAL.Base;

namespace VMall.Services
{

    /// <summary>
    /// 频道管理类
    /// </summary>
   
    public partial class SuitPromotions : BaseDAL<SuitPromotionInfo>
    {
        public SuitPromotions()
        { }

        #region  BasicMethod
        
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string chidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_suitpromotions ");
            strSql.Append(" where pmid in (" + chidlist + ")  ");
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

        /// <summary>
        /// 获得数据列表
        /// </summary>
        //public DataSet GetList(string strWhere)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select * ");
        //    strSql.Append(" FROM hlh_channel ");
        //    if (strWhere.Trim() != "")
        //    {
        //        strSql.Append(" where " + strWhere);
        //    }
        //    return DbHelperSQL.Query(strSql.ToString());
        //}

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        //public DataSet GetList(int Top, string strWhere, string filedOrder)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select ");
        //    if (Top > 0)
        //    {
        //        strSql.Append(" top " + Top.ToString());
        //    }
        //    strSql.Append(" * ");
        //    strSql.Append(" FROM hlh_channel ");
        //    if (strWhere.Trim() != "")
        //    {
        //        strSql.Append(" where " + strWhere);
        //    }
        //    strSql.Append(" order by " + filedOrder);
        //    return DbHelperSQL.Query(strSql.ToString());
        //}

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_channel ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            object obj = DbHelperSQL.GetSingle(strSql.ToString());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.chid desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_channel T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            return DbHelperSQL.Query(strSql.ToString());
        }

        #endregion  BasicMethod

        #region  ExtensionMethod

        #region 秒杀

        /// <summary>
        /// 获得秒杀产品数量
        /// </summary>
        /// <returns></returns>
        public static int GetFlashSaleProductCount()
        {
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT COUNT(*) FROM hlh_singlepromotions a LEFT JOIN hlh_products b ON a.pid = b.pid WHERE a.state=1 AND a.discounttype=3 AND b.state=0");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText.ToString()));
        }

        /// <summary>
        /// 获得秒杀商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static DataTable GetFlashSaleProductList(int pageSize, int pageNumber, int sortColumn, int sortDirection)
        {
            List<StoreProductInfo> storeProductList = new List<StoreProductInfo>();
            DataTable reader = FlashSaleProductList(pageSize, pageNumber, sortColumn, sortDirection);
            return reader;
        }

        /// <summary>
        /// 获得秒杀商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static DataTable FlashSaleProductList(int pageSize, int pageNumber, int sortColumn, int sortDirection)
        {
            StringBuilder commandText = new StringBuilder();
            //SELECT * FROM dbo.hlh_singlepromotions a LEFT JOIN dbo.hlh_products b ON a.pid = b.pid WHERE a.state=1 AND a.discounttype=3 AND b.state=0

            if (pageNumber == 1)
            {
                commandText.AppendFormat("SELECT TOP {0}  b.name AS pname, * FROM hlh_singlepromotions a LEFT JOIN hlh_products b ON a.pid = b.pid WHERE a.state=1 AND a.discounttype=3 AND b.state=0 and a.isshow=1 and a.endtime1>=getdate()", pageSize);
                commandText.Append(" ORDER BY b.showorder asc,a.starttime1 asc,");

                switch (sortColumn)
                {
                    case 0:
                        commandText.Append("[b].[salecount]");
                        break;
                    case 1:
                        commandText.Append("[b].[shopprice]");
                        break;
                    case 2:
                        commandText.Append("[b].[reviewcount]");
                        break;
                    case 3:
                        commandText.Append("[b].[addtime]");
                        break;
                    case 4:
                        commandText.Append("[b].[visitcount]");
                        break;
                    default:
                        commandText.Append("[b].[salecount]");
                        break;
                }
                switch (sortDirection)
                {
                    case 0:
                        commandText.Append(" DESC");
                        break;
                    case 1:
                        commandText.Append(" ASC");
                        break;
                    default:
                        commandText.Append(" DESC");
                        break;
                }
            }
            else
            {
                commandText.Append("SELECT * FROM");
                commandText.Append(" (SELECT ROW_NUMBER() OVER (ORDER BY ");
                commandText.Append(" b.showorder asc, a.starttime1 asc,");
                switch (sortColumn)
                {
                    case 0:
                        commandText.Append("[b].[salecount]");
                        break;
                    case 1:
                        commandText.Append("[b].[shopprice]");
                        break;
                    case 2:
                        commandText.Append("[b].[reviewcount]");
                        break;
                    case 3:
                        commandText.Append("[b].[addtime]");
                        break;
                    case 4:
                        commandText.Append("[b].[visitcount]");
                        break;
                    default:
                        commandText.Append("[b].[salecount]");
                        break;
                }
                switch (sortDirection)
                {
                    case 0:
                        commandText.Append(" DESC");
                        break;
                    case 1:
                        commandText.Append(" ASC");
                        break;
                    default:
                        commandText.Append(" DESC");
                        break;
                }
                commandText.Append(") AS [rowid], b.name AS pname,b.showimg,b.shopprice, a.* FROM hlh_singlepromotions a LEFT JOIN hlh_products b ON a.pid = b.pid WHERE a.state=1 AND a.discounttype=3 AND b.state=0 and isshow=1 ");

                commandText.Append(") AS [temp]");
                commandText.AppendFormat(" WHERE [rowid] BETWEEN {0} AND {1}", pageSize * (pageNumber - 1) + 1, pageSize * pageNumber);
            }

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText.ToString()).Tables[0];
        }
        #endregion

        #endregion  ExtensionMethod


    }
}
