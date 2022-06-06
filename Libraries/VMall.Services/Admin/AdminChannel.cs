using System;
using System.IO;
using System.Text;
using System.Data;
using System.Collections.Generic;

using VMall.Core;

namespace VMall.Services
{
    using System.Data.SqlClient;
    /// <summary>
    /// 后台频道操作管理类
    /// </summary>
    public class AdminChannel
    {
        /// <summary>
        /// 后台获得频道数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int AdminGetChannelCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(chid) FROM [{0}channel] WHERE mallsource=0 ", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(chid) FROM [{0}channel] WHERE {1}  AND mallsource=0 ", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText));
        }

        /// <summary>
        /// 后台获得频道选择列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static DataTable AdminGetChannelSelectList(int pageSize, int pageNumber, string condition)
        {
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE mallsource=0  ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE {2} ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE mallsource=0 ORDER BY [displayorder] DESC) ORDER BY [displayorder] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize);
                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}channel] WHERE [chid] NOT IN (SELECT TOP {2} [chid] FROM [{1}channel] WHERE {3} ORDER BY [displayorder] DESC) AND {3} ORDER BY [displayorder] DESC",
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
        public static int CreateChannelProduct(ChannelProductInfo channelProductInfo)
        {
            SqlParameter[] parms = {
									 new SqlParameter("@creationdate",channelProductInfo.CreationDate),
									 new SqlParameter("@chid",channelProductInfo.ChId),
									 new SqlParameter("@pid",channelProductInfo.Pid),
									 new SqlParameter("@state",channelProductInfo.State)
                                   };

            string commandText = string.Format(@"INSERT INTO [{0}channelproducts]([creationdate],[chid],[pid],[state]) VALUES(@creationdate,@chid,@pid,@state);SELECT SCOPE_IDENTITY();", RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms), -1);
        }

        /// <summary>
        /// 根据产品id获取频道信息
        /// </summary>
        /// <param name="pid">商品id</param>
        /// <returns>频道信息</returns>
        public static List<ChannelInfo> GetChanneListlByPid(int pid)
        {
            List<ChannelInfo> infoList = new List<ChannelInfo>();
            SqlParameter[] parms = {
									 new SqlParameter("@pid",pid)
                                   };
            //ChannelInfo channelInfo = new ChannelInfo();
            string commandText = string.Format(@"SELECT b.chid,b.name FROM [{0}channelproducts] a LEFT JOIN [{0}channel] b on a.chid=b.chid WHERE a.pid=@pid", RDBSHelper.RDBSTablePre);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    ChannelInfo info = new ChannelInfo();
                    info.ChId = TypeHelper.ObjectToInt(reader["chid"]);
                    info.Name = reader["name"].ToString();
                    infoList.Add(info);
                }
                reader.Close();
            }

            return infoList;
        }

        /// <summary>
        /// 根据条件获取频道信息
        /// </summary>
        /// <param name="strWhere">条件</param>
        /// <returns>频道信息</returns>
        public static List<ChannelInfo> GetChanneListlByWhere(string strWhere)
        {
            List<ChannelInfo> infoList = new List<ChannelInfo>();

            string commandText = string.Format(@"SELECT * FROM  [{0}channel]  WHERE {1}", RDBSHelper.RDBSTablePre, strWhere);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    ChannelInfo info = new ChannelInfo();
                    info.ChId = TypeHelper.ObjectToInt(reader["chid"]);
                    info.Name = reader["name"].ToString();
                    infoList.Add(info);
                }
                reader.Close();
            }

            return infoList;
        }
    }
}
