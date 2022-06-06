using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Web.MallAdmin.bll
{
    using VMall.Core;
    using VMall.Services;
    using System.Data;
    using System.Data.SqlClient;
    public class ChannelBLL
    {
        /// <summary>
        /// 后台获得频道数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public int AdminGetChannelCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(chid) FROM [{0}channel]  WHERE [state]=1  AND mallsource=0", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(chid) FROM [{0}channel] WHERE {1} AND [state]=1  AND mallsource=0", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 后台获得频道选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public DataTable AdminGetChannelSelectList(int pageSize, int pageNumber, string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [state]=1  AND mallsource=0  ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE {2} AND [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC) ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize);
                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE {3} AND [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC) AND {3} ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                condition);
            }

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }

        /// <summary>
        /// 后台获得频道选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public DataTable AdminGetChannelList(int pageSize, int pageNumber, string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [state]=1  AND mallsource=0  ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE {2} AND [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC) ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize);
                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE {3} AND [state]=1  AND mallsource=0 ORDER BY [displayorder] DESC) AND {3} ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                condition);
            }

            return RDBSHelper.ExecuteDataset(CommandType.Text, commandText).Tables[0];
        }

        /// <summary>
        /// 创建频道商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <returns>商品id</returns>
        public int CreateChannelProduct(ChannelProductInfo channelProductInfo)
        {
            //SqlParameter[] parameters = new SqlParameter[] { new SqlParameter("@uid", uid) };
            //SqlParameter param = new SqlParameter(paramName, sqlDbType, size);
            SqlParameter[] parms = {
									 new SqlParameter("@creationdate",channelProductInfo.CreationDate),
									 new SqlParameter("@chid",channelProductInfo.ChId),
									 new SqlParameter("@pid",channelProductInfo.Pid),
									 new SqlParameter("@state",channelProductInfo.State)
                                   };

            string commandText = string.Format(@"INSERT INTO [{0}channelproducts]([creationdate],[chid],[pid],[state]) VALUES(@creationdate,@chid,@pid,@state);SELECT SCOPE_IDENTITY();",RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms), -1);
        }

    }
}
