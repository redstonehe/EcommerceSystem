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
   
    public partial class Channel : BaseDAL<ChannelInfo>
    {
        public Channel()
        { }

        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int chid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_channel");
            strSql.Append(" where chid=@chid");
            SqlParameter[] parameters = {
					new SqlParameter("@chid", SqlDbType.SmallInt)
			};
            parameters[0].Value = chid;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int AddModel(ChannelInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_channel(");
            strSql.Append("creationdate,displayorder,name,state,description,mallsource,linktype,linkurl)");
            strSql.Append(" values (");
            strSql.Append("@creationdate,@displayorder,@name,@state,@description,@mallsource,@linktype,@linkurl)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@creationdate", SqlDbType.DateTime),
					new SqlParameter("@displayorder", SqlDbType.Int,4),
					new SqlParameter("@name", SqlDbType.NChar,100),
					new SqlParameter("@state", SqlDbType.TinyInt,1),
					new SqlParameter("@description", SqlDbType.NText),
					new SqlParameter("@mallsource", SqlDbType.Int,4),
                    new SqlParameter("@linktype", SqlDbType.TinyInt),
                    new SqlParameter("@linkurl", SqlDbType.VarChar,100)};
            parameters[0].Value = model.CreationDate;
            parameters[1].Value = model.DisplayOrder;
            parameters[2].Value = model.Name;
            parameters[3].Value = model.State;
            parameters[4].Value = model.Description;
            parameters[5].Value = model.MallSource;
            parameters[6].Value = model.LinkType;
            parameters[7].Value = model.LinkUrl;
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), parameters);
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
        /// 更新一条数据
        /// </summary>
        //public bool Update(ChannelInfo model)
        //{
        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("update hlh_channel set ");
        //    strSql.Append("creationdate=@creationdate,");
        //    strSql.Append("displayorder=@displayorder,");
        //    strSql.Append("name=@name,");
        //    strSql.Append("state=@state,");
        //    strSql.Append("description=@description,");
        //    strSql.Append("mallsource=@mallsource,");
        //    strSql.Append("linktype=@linktype,");
        //    strSql.Append("linkurl=@linkurl");
        //    strSql.Append(" where chid=@chid");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@creationdate", SqlDbType.DateTime),
        //            new SqlParameter("@displayorder", SqlDbType.Int,4),
        //            new SqlParameter("@name", SqlDbType.NChar,100),
        //            new SqlParameter("@state", SqlDbType.TinyInt,1),
        //            new SqlParameter("@description", SqlDbType.NText),
        //            new SqlParameter("@mallsource", SqlDbType.Int,4),
        //            new SqlParameter("@linktype", SqlDbType.TinyInt),
        //            new SqlParameter("@linkurl", SqlDbType.VarChar,100),
        //            new SqlParameter("@chid", SqlDbType.SmallInt,2)};
        //    parameters[0].Value = model.CreationDate;
        //    parameters[1].Value = model.DisplayOrder;
        //    parameters[2].Value = model.Name;
        //    parameters[3].Value = model.State;
        //    parameters[4].Value = model.Description;
        //    parameters[5].Value = model.MallSource;
        //    parameters[6].Value = model.LinkType;
        //    parameters[7].Value = model.LinkUrl;
        //    parameters[8].Value = model.ChId;

        //    int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
        //    if (rows > 0)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int chid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_channel ");
            strSql.Append(" where chid=@chid");
            SqlParameter[] parameters = {
					new SqlParameter("@chid", SqlDbType.SmallInt)
			};
            parameters[0].Value = chid;

            int rows = DbHelperSQL.ExecuteSql(strSql.ToString(), parameters);
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
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string chidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_channel ");
            strSql.Append(" where chid in (" + chidlist + ")  ");
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


        ///// <summary>
        ///// 得到一个对象实体
        ///// </summary>
        //public ChannelInfo GetModel(int chid)
        //{

        //    StringBuilder strSql = new StringBuilder();
        //    strSql.Append("select  top 1 * from hlh_channel ");
        //    strSql.Append(" where chid=@chid");
        //    SqlParameter[] parameters = {
        //            new SqlParameter("@chid", SqlDbType.SmallInt)
        //    };
        //    parameters[0].Value = chid;

        //    ChannelInfo model = new ChannelInfo();
        //    DataSet ds = DbHelperSQL.Query(strSql.ToString(), parameters);
        //    if (ds.Tables[0].Rows.Count > 0)
        //    {
        //        return DataRowToModel(ds.Tables[0].Rows[0]);
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public ChannelInfo DataRowToModel(DataRow row)
        {
            ChannelInfo model = new ChannelInfo();
            if (row != null)
            {
                if (row["chid"] != null && row["chid"].ToString() != "")
                {
                    model.ChId = int.Parse(row["chid"].ToString());
                }
                if (row["creationdate"] != null && row["creationdate"].ToString() != "")
                {
                    model.CreationDate = DateTime.Parse(row["creationdate"].ToString());
                }
                if (row["displayorder"] != null && row["displayorder"].ToString() != "")
                {
                    model.DisplayOrder = int.Parse(row["displayorder"].ToString());
                }
                if (row["name"] != null)
                {
                    model.Name = row["name"].ToString();
                }
                if (row["state"] != null && row["state"].ToString() != "")
                {
                    model.State = int.Parse(row["state"].ToString());
                }
                if (row["description"] != null)
                {
                    model.Description = row["description"].ToString();
                }
                if (row["mallsource"] != null && row["mallsource"].ToString() != "")
                {
                    model.MallSource = int.Parse(row["mallsource"].ToString());
                }
                if (row["linktype"] != null && row["linktype"].ToString() != "")
                {
                    model.LinkType = int.Parse(row["linktype"].ToString());
                }
                if (row["linkurl"] != null)
                {
                    model.LinkUrl = row["linkurl"].ToString();
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public DataSet GetList(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_channel ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_channel ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            return DbHelperSQL.Query(strSql.ToString());
        }

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

        /*
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public DataSet GetList(int PageSize,int PageIndex,string strWhere)
        {
            SqlParameter[] parameters = {
                    new SqlParameter("@tblName", SqlDbType.VarChar, 255),
                    new SqlParameter("@fldName", SqlDbType.VarChar, 255),
                    new SqlParameter("@PageSize", SqlDbType.Int),
                    new SqlParameter("@PageIndex", SqlDbType.Int),
                    new SqlParameter("@IsReCount", SqlDbType.Bit),
                    new SqlParameter("@OrderType", SqlDbType.Bit),
                    new SqlParameter("@strWhere", SqlDbType.VarChar,1000),
                    };
            parameters[0].Value = "hlh_channel";
            parameters[1].Value = "chid";
            parameters[2].Value = PageSize;
            parameters[3].Value = PageIndex;
            parameters[4].Value = 0;
            parameters[5].Value = 0;
            parameters[6].Value = strWhere;	
            return DbHelperSQL.RunProcedure("UP_GetRecordByPage",parameters,"ds");
        }*/

        #endregion  BasicMethod

        #region  ExtensionMethod


        /// <summary>
        /// 获得频道列表
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<ChannelInfo> GetChannelList(int pageNumber = 1, int pageSize = 100)
        {
            List<ChannelInfo> ChannelList = VMall.Core.BMACache.Get("/Mall/ChannelList") as List<ChannelInfo>;
            if (ChannelList == null)
            {
                string condition = "  mallsource=0 ";
                ChannelList = new List<ChannelInfo>();
                DataTable ChannelSelectList = AdminChannel.AdminGetChannelSelectList(pageSize, pageNumber, condition);
                foreach (DataRow row in ChannelSelectList.Rows)
                {
                    ChannelInfo channelInfo = new ChannelInfo();
                    channelInfo.ChId = TypeHelper.ObjectToInt(row["chid"]);
                    channelInfo.CreationDate = TypeHelper.ObjectToDateTime(row["creationdate"]);
                    channelInfo.DisplayOrder = TypeHelper.ObjectToInt(row["displayorder"]);
                    channelInfo.Name = row["name"].ToString().Trim();
                    channelInfo.State = TypeHelper.ObjectToInt(row["state"]);
                    channelInfo.Description = row["description"].ToString().Trim();
                    channelInfo.LinkType = TypeHelper.ObjectToInt(row["linktype"]);
                    channelInfo.LinkUrl = row["linkurl"].ToString();
                    ChannelList.Add(channelInfo);
                }
                VMall.Core.BMACache.Insert("/Mall/ChannelList", ChannelList, 3600*24);
            }
            return ChannelList;
        }

        /// <summary>
        /// 获得频道列表
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<ChannelInfo> GetChannelListByWhere(string strWhere, int pageNumber = 1, int pageSize = 100)
        {
            List<ChannelInfo> ChannelList = null;
            if (ChannelList == null)
            {
                string condition = "";
                if (!string.IsNullOrEmpty(strWhere))
                {
                    condition = "  " + strWhere;
                }
                ChannelList = new List<ChannelInfo>();
                DataTable ChannelSelectList = AdminChannel.AdminGetChannelSelectList(pageSize, pageNumber, condition);
                foreach (DataRow row in ChannelSelectList.Rows)
                {
                    ChannelInfo channelInfo = new ChannelInfo();
                    channelInfo.ChId = TypeHelper.ObjectToInt(row["chid"]);
                    channelInfo.CreationDate = TypeHelper.ObjectToDateTime(row["creationdate"]);
                    channelInfo.DisplayOrder = TypeHelper.ObjectToInt(row["displayorder"]);
                    channelInfo.Name = row["name"].ToString().Trim();
                    channelInfo.State = TypeHelper.ObjectToInt(row["state"]);
                    channelInfo.Description = row["description"].ToString().Trim();
                    channelInfo.LinkType = TypeHelper.ObjectToInt(row["linktype"]);
                    channelInfo.LinkUrl = row["linkurl"].ToString();
                    ChannelList.Add(channelInfo);
                }
                //VMall.Core.BMACache.Insert("/Mall/ChannelList", ChannelList);
            }
            return ChannelList;
        }

        /// <summary>
        /// 获得频道
        /// </summary>
        /// <param name="chId">分类id</param>
        /// <returns></returns>
        public static ChannelInfo GetChannelById(int chId)
        {
            foreach (ChannelInfo channelInfo in GetChannelList())
            {
                if (channelInfo.ChId == chId)
                    return channelInfo;
            }
            return null;
        }
        /// <summary>
        /// 获得频道根据名称
        /// </summary>
        /// <param name="chId">分类id</param>
        /// <returns></returns>
        public static ChannelInfo GetChannelByName(string name)
        {
            foreach (ChannelInfo channelInfo in GetChannelList())
            {
                if (channelInfo.Name.Trim() == name.Trim())
                    return channelInfo;
            }
            return null;
        }

        /// <summary>
        /// 创建频道商品
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <returns>商品id</returns>
        public static int CreateChannel(ChannelInfo info)
        {
            SqlParameter[] parms = {
									 new SqlParameter("@creationdate",info.CreationDate),
                                     new SqlParameter("@displayorder",info.DisplayOrder),
									 new SqlParameter("@name",info.Name),
									 new SqlParameter("@state",info.State),
									 new SqlParameter("@description",info.Description),
									 new SqlParameter("@mallsource",0)	
                                   };

            string commandText = string.Format(@"INSERT INTO [{0}channel] ([creationdate],[displayorder],[name],[state],[description],[mallsource]) VALUES (@creationdate,@displayorder,@name,@state,@description,@mallsource) ", RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms), -1);
        }
        /// <summary>
        /// 修改频道信息
        /// </summary>
        /// <param name="productInfo">商品信息</param>
        /// <returns>商品id</returns>
        public static int UpdateChannel(ChannelInfo channelInfo)
        {
            SqlParameter[] parms = {
									 new SqlParameter("@displayorder",channelInfo.DisplayOrder),
									 new SqlParameter("@name",channelInfo.Name),
									 new SqlParameter("@state",channelInfo.State),
									 new SqlParameter("@description",channelInfo.Description),
                                      new SqlParameter("@chid",channelInfo.ChId)
                                   };
            string commandText = string.Format(@"update  [{0}channel] set [displayorder]=@displayorder,[name]=@name,[state]=@state,[description]=@description  where [chid]=@chid", RDBSHelper.RDBSTablePre);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText, parms), -1);
        }

        #region 前台频道产品列表相关

        /// <summary>
        ///  获得频道关联的一级分类
        /// </summary>
        /// <param name="storeSTid">店铺模板id</param>
        /// <param name="provinceId">省id</param>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static List<CategoryInfo> GetChannelFristCateoryList(int chId)
        {
            List<CategoryInfo> list = new List<CategoryInfo>();

            string commandText = @"SELECT  * FROM [hlh_categories] WHERE   CHARINDEX('," + chId.ToString() + ",' ,','+[channelid]+',')>0   AND layer=1 AND parentid=0";
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    CategoryInfo orderInfo = VMall.Data.Categories.BuildCategoryFromReader(reader);
                    list.Add(orderInfo);
                }
                reader.Close();
            }
            return list;



        }

        /// <summary>
        /// 获得频道关联的品牌
        /// </summary>
        /// <param name="chId">频道id</param>
        /// <returns></returns>
        public static List<BrandInfo> GetChannelBrandList(int chId, int cateId = 0)
        {
            List<BrandInfo> brandList = VMall.Core.BMACache.Get("/Mall/ChannelBrandList/" + chId + "_" + cateId) as List<BrandInfo>;
            if (brandList == null)
            {
                //brandList = VMall.Data.Categories.GetCategoryBrandList(chId);
                brandList = new List<BrandInfo>();
                IDataReader reader = GetBrandList(chId, cateId);
                while (reader.Read())
                {
                    BrandInfo brandInfo = VMall.Data.Brands.BuildBrandFromReader(reader);
                    brandList.Add(brandInfo);
                }
                reader.Close();

                if (brandList != null)
                    VMall.Core.BMACache.Insert("/Mall/ChannelBrandList/" + chId + "_" + cateId, brandList);
            }

            return brandList;
        }

        /// <summary>
        /// 获得关联的品牌
        /// </summary>
        /// <param name="catchIdeId">频道id</param>
        /// <returns></returns>
        public static IDataReader GetBrandList(int chId, int cateId = 0)
        {
            SqlParameter[] parms = {
                                        new SqlParameter("@chId", chId)    
                                    };
            string strWhere = string.Empty;
            if (cateId > 0)
                strWhere = string.Format(" AND a.[cateid]={0} ", cateId);
            string commandText = string.Format(@"SELECT [brandid],[displayorder],[name],[logo] FROM [{0}brands] 
    WHERE [brandid] IN (
		SELECT DISTINCT [brandid] FROM [{0}products] a LEFT JOIN [{0}channelproducts] b ON a.pid=b.pid
		WHERE [chid]=@chId AND a.[state]=0 {1}
    ) ORDER BY [displayorder] DESC", RDBSHelper.RDBSTablePre, strWhere);

            return RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 获得频道产品数量
        /// </summary>
        /// <param name="chId"></param>
        /// <param name="brandId"></param>
        /// <param name="filterPrice"></param>
        /// <param name="catePriceRangeList"></param>
        /// <param name="onlyStock"></param>
        /// <returns></returns>
        public static int GetChannelProductCount(int chId, string cateIds, int brandId, string[] catePriceRangeList, int onlyStock, int filterPrice = 0, int gid = 0)
        {
            StringBuilder commandText = new StringBuilder();

            commandText.AppendFormat("SELECT COUNT([p].[pid]) FROM [{0}products] AS [p]", RDBSHelper.RDBSTablePre);

            if (onlyStock == 1)
                commandText.AppendFormat(" LEFT JOIN [{0}productstocks] AS [ps] ON [p].[pid]=[ps].[pid]", RDBSHelper.RDBSTablePre);

            commandText.AppendFormat(" LEFT JOIN [{0}stores] AS [s] ON [p].[storeid]=[s].[storeid]", RDBSHelper.RDBSTablePre);
            if (gid > 0)
                commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [hlh_channelproducts] c,[hlh_GroupProduct] gp where charindex(','+ltrim(c.pid)+',',','+  gp.Products+',' )>0 AND groupid={0} )", gid);
            else
                commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [{0}channelproducts] c WHERE c.chid={1})", RDBSHelper.RDBSTablePre, chId);

            if (!string.IsNullOrEmpty(cateIds))
                commandText.AppendFormat(" AND [p].[cateid] IN ({0}) ", cateIds);
            if (brandId > 0)
                commandText.AppendFormat(" AND [p].[brandid]={0}", brandId);

            if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            {
                string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
                if (priceRange.Length == 1)
                    if (priceRange[0].Contains("以上"))
                        commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0].Replace("以上", ""));
                    else
                        commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
                else if (priceRange.Length == 2)
                    commandText.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
            }

            commandText.Append(" AND [p].[state]=0");
            commandText.Append(" AND [p].[displayorder]=1");
            //if (attrValueIdList.Count > 0)
            //{
            //    commandText.Append(" AND [p].[pid] IN (SELECT [pa1].[pid] FROM");
            //    for (int i = 0; i < attrValueIdList.Count; i++)
            //    {
            //        if (i == 0)
            //            commandText.AppendFormat(" (SELECT [pid] FROM [{0}productattributes] WHERE [attrvalueid]={1}) AS [pa1]", RDBSHelper.RDBSTablePre, attrValueIdList[i]);
            //        else
            //            commandText.AppendFormat(" INNER JOIN (SELECT [pid] FROM [{0}productattributes] WHERE [attrvalueid]={1}) AS [pa{2}] ON [pa{2}].[pid]=[pa{3}].[pid]", RDBSHelper.RDBSTablePre, attrValueIdList[i], i + 1, i);
            //    }
            //    commandText.Append(")");
            //}

            if (onlyStock == 1)
                commandText.Append(" AND [ps].[number]>0");

            commandText.Append(" AND [s].[state]=0");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText.ToString()));
        }

        /// <summary>
        /// 获得频道商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="chId">频道id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="filterPrice">筛选价格</param>
        /// <param name="catePriceRangeList">分类价格范围列表</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static List<StoreProductInfo> GetChannelProductList(int pageSize, int pageNumber, int chId, string cateIds, int brandId, int filterPrice, string[] catePriceRangeList, int onlyStock, int sortColumn, int sortDirection, string productName = "", int gid = 0)
        {
            List<StoreProductInfo> storeProductList = new List<StoreProductInfo>();
            IDataReader reader = ChannelProductList(pageSize, pageNumber, chId, cateIds, brandId, filterPrice, catePriceRangeList, onlyStock, sortColumn, sortDirection, productName, gid);
            while (reader.Read())
            {
                StoreProductInfo storeProductInfo = VMall.Data.Products.BuildStoreProductFromReader(reader);
                storeProductList.Add(storeProductInfo);
            }

            reader.Close();
            return storeProductList;
        }


        /// <summary>
        /// 获得频道商品列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <param name="chId">频道id</param>
        /// <param name="brandId">品牌id</param>
        /// <param name="filterPrice">筛选价格</param>
        /// <param name="catePriceRangeList">分类价格范围列表</param>
        /// <param name="attrValueIdList">属性值id列表</param>
        /// <param name="onlyStock">是否只显示有货</param>
        /// <param name="sortColumn">排序列</param>
        /// <param name="sortDirection">排序方向</param>
        /// <returns></returns>
        public static IDataReader ChannelProductList(int pageSize, int pageNumber, int chId, string cateIds, int brandId, int filterPrice, string[] catePriceRangeList, int onlyStock, int sortColumn, int sortDirection, string productName = "", int gid = 0)
        {
            StringBuilder commandText = new StringBuilder();

            //if (pageNumber == 1)
            //{
            //    commandText.AppendFormat("SELECT TOP {1} [p].*,[s].[name] AS [storename] FROM [{0}products] AS [p]", RDBSHelper.RDBSTablePre, pageSize);

            //    if (onlyStock == 1)
            //        commandText.AppendFormat(" LEFT JOIN [{0}productstocks] AS [ps] ON [p].[pid]=[ps].[pid]", RDBSHelper.RDBSTablePre);

            //    commandText.AppendFormat(" LEFT JOIN [{0}stores] AS [s] ON [p].[storeid]=[s].[storeid]", RDBSHelper.RDBSTablePre);
            //    if (gid > 0)
            //        commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [hlh_channelproducts] c,[hlh_GroupProduct] gp where charindex(','+ltrim(c.pid)+',',','+  gp.Products+',' )>0 AND groupid={0} )", gid);
            //    else
            //        commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [{0}channelproducts] c WHERE c.chid={1})", RDBSHelper.RDBSTablePre, chId);
            //    if (!string.IsNullOrEmpty(cateIds))
            //        commandText.AppendFormat(" AND [p].[cateid] IN ({0}) ", cateIds);
            //    if (brandId > 0)
            //        commandText.AppendFormat(" AND [p].[brandid]={0}", brandId);
            //    if (!string.IsNullOrEmpty(productName))
            //        commandText.AppendFormat(" AND [p].[name] like '%{0}%'", productName);
            //    if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            //    {
            //        string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
            //        if (priceRange.Length == 1)
            //            if (priceRange[0].Contains("以上"))
            //                commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0].Replace("以上", ""));
            //            else
            //                commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
            //        else if (priceRange.Length == 2)
            //            commandText.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
            //    }

            //    commandText.Append(" AND [p].[state]=0");

            //    commandText.Append(" AND [p].[displayorder]=1");

            //    if (onlyStock == 1)
            //        commandText.Append(" AND [ps].[number]>0");

            //    commandText.Append(" AND [s].[state]=0");

            //    commandText.Append(" ORDER BY ");
            //    if (sortColumn > 0)
            //    {

            //        commandText.Append(" [p].[isbest] DESC, ");
            //        switch (sortColumn)
            //        {
            //            case 1:
            //                commandText.Append("[p].[salecount]");
            //                break;
            //            case 2:
            //                commandText.Append("[p].[shopprice]");
            //                break;
            //            case 3:
            //                commandText.Append("[p].[reviewcount]");
            //                break;
            //            case 4:
            //                commandText.Append("[p].[addtime]");
            //                break;
            //            case 5:
            //                commandText.Append("[p].[visitcount]");
            //                break;
            //            default:
            //                commandText.Append("[p].[salecount]");
            //                break;
            //        }
            //        switch (sortDirection)
            //        {
            //            case 0:
            //                commandText.Append(" DESC");
            //                break;
            //            case 1:
            //                commandText.Append(" ASC");
            //                break;
            //            default:
            //                commandText.Append(" DESC");
            //                break;
            //        }
            //    }
            //    else
            //    {
            //        if (chId == 1 || chId == 2)
            //            commandText.Append(" [p].[showorder]  ASC");
            //        else
            //            commandText.Append(" [p].[isbest] DESC, [p].[salecount]  DESC");
            //    }

            //}
            //else
            //{
            commandText.Append("SELECT * FROM");
            commandText.Append(" (SELECT ROW_NUMBER() OVER (ORDER BY ");
            if (sortColumn > 0)
            {
                commandText.Append(" [p].[showorder] ASC, ");
                commandText.Append(" [p].[isbest] DESC, ");
                switch (sortColumn)
                {
                    case 1:
                        commandText.Append("[p].[salecount]");
                        break;
                    case 2:
                        commandText.Append("[p].[shopprice]");
                        break;
                    case 3:
                        commandText.Append("[p].[reviewcount]");
                        break;
                    case 4:
                        commandText.Append("[p].[addtime]");
                        break;
                    case 5:
                        commandText.Append("[p].[visitcount]");
                        break;
                    default:
                        commandText.Append("[p].[salecount]");
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
                if (chId == 1 || chId == 2 || chId == 4 || chId == 6 || chId == 7 || chId == 9 )
                    commandText.Append(" [p].[showorder]  ASC");
                else
                    commandText.Append(" [p].[isbest] DESC, [p].[salecount]  DESC");
            }

            commandText.AppendFormat(") AS [rowid],[p].*,[s].[name] AS [storename] FROM [{0}products] AS [p]", RDBSHelper.RDBSTablePre);
            if (onlyStock == 1)
                commandText.AppendFormat(" LEFT JOIN [{0}productstocks] AS [ps] ON [p].[pid]=[ps].[pid]", RDBSHelper.RDBSTablePre);

            commandText.AppendFormat(" LEFT JOIN [{0}stores] AS [s] ON [p].[storeid]=[s].[storeid]", RDBSHelper.RDBSTablePre);
            if (gid > 0)
                commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [hlh_channelproducts] c,[hlh_GroupProduct] gp where charindex(','+ltrim(c.pid)+',',','+  gp.Products+',' )>0 AND groupid={0} )", gid);
            else
                commandText.AppendFormat(" WHERE [p].[pid] IN (SELECT c.pid from [{0}channelproducts] c WHERE c.chid={1})", RDBSHelper.RDBSTablePre, chId);
            if (!string.IsNullOrEmpty(cateIds))
                commandText.AppendFormat(" AND [p].[cateid] IN ({0}) ", cateIds);
            if (brandId > 0)
                commandText.AppendFormat(" AND [p].[brandid]={0}", brandId);
            if (!string.IsNullOrEmpty(productName))
                commandText.AppendFormat(" AND [p].[name] like '%{0}%'", productName);
            if (filterPrice > 0 && filterPrice <= catePriceRangeList.Length)
            {
                string[] priceRange = StringHelper.SplitString(catePriceRangeList[filterPrice - 1], "-");
                if (priceRange.Length == 1)
                    if (priceRange[0].Contains("以上"))
                        commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0].Replace("以上", ""));
                    else
                        commandText.AppendFormat(" AND [p].[shopprice]>='{0}'", priceRange[0]);
                else if (priceRange.Length == 2)
                    commandText.AppendFormat(" AND [p].[shopprice]>='{0}' AND [p].[shopprice]<'{1}'", priceRange[0], priceRange[1]);
            }

            commandText.Append(" AND [p].[state]=0");

            commandText.Append(" AND [p].[displayorder]=1");

            if (onlyStock == 1)
                commandText.Append(" AND [ps].[number]>0");

            commandText.Append(" AND [s].[state]=0");

            commandText.Append(") AS [temp]");
            commandText.AppendFormat(" WHERE [rowid] BETWEEN {0} AND {1}", pageSize * (pageNumber - 1) + 1, pageSize * pageNumber);

            //}

            return RDBSHelper.ExecuteReader(CommandType.Text, commandText.ToString());
        }
        #endregion

        #region  频道购物车相关

        /// <summary>
        /// 获得购物车内产品所属频道列表
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static List<int> GetCartProductChanId(int uid, string selectedCartItemKeyList)
        {
            StringBuilder keyStr = new StringBuilder();
            if (!string.IsNullOrEmpty(selectedCartItemKeyList))
            {
                string[] keyList = StringHelper.SplitString(selectedCartItemKeyList);

                foreach (var item in keyList)
                {
                    if (item.IndexOf("_") >= 0)
                    {
                        keyStr.Append(item.Split('_')[1]);
                        keyStr.Append(",");
                    }
                }
                keyStr.Remove(keyStr.Length - 1, 1);
            }
            List<int> chidList = new List<int>();
            SqlParameter[] parms = {
                                        new SqlParameter("@uid", uid)    
                                    };
            string commandText = string.Empty;
            if (string.IsNullOrEmpty(selectedCartItemKeyList))
                commandText = "SELECT b.chid FROM [hlh_orderproducts] a LEFT JOIN dbo.hlh_channelproducts b ON a.pid = b.pid   WHERE [uid]=@uid AND [oid]=0";
            else
                commandText = string.Format("SELECT b.chid FROM [hlh_orderproducts] a LEFT JOIN dbo.hlh_channelproducts b ON a.pid = b.pid   WHERE [uid]=@uid AND [oid]=0  AND a.[pid] in ({0})", keyStr);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    int info = TypeHelper.ObjectToInt(reader[0]);
                    chidList.Add(info);
                }
                reader.Close();
            }

            return chidList;

        }

        /// <summary>
        /// 获得专区频道内的商品总计
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="chId"></param>
        /// <param name="selectedCartItemKeyList">type为1是格式为type_pid,如：0_44,0_2，type为2时格式为pid集合，如55，66</param>
        /// <param name="type">1为购物车使用 2为提交订单页面</param>
        /// <returns></returns>
        public static decimal GetChannelProductAmount(int uid, int chId, string selectedCartItemKeyList, int type = 1)
        {
            //SELECT sum(discountprice*buycount) FROM [hlh_orderproducts] a LEFT JOIN dbo.hlh_channelproducts b ON a.pid = b.pid WHERE  chid=1 AND uid=1 AND oid=0
            //  AND a.pid IN (581,530,535)
            decimal Sum = 0M;
            StringBuilder keyStr = new StringBuilder();
            if (!string.IsNullOrEmpty(selectedCartItemKeyList))
            {
                if (type == 1)
                {
                    string[] keyList = StringHelper.SplitString(selectedCartItemKeyList);

                    foreach (var item in keyList)
                    {
                        if (item.IndexOf("_") >= 0)
                        {
                            keyStr.Append(item.Split('_')[1]);
                            keyStr.Append(",");
                        }
                    }
                    keyStr.Remove(keyStr.Length - 1, 1);
                }
                else if (type == 2)
                    keyStr.Append(selectedCartItemKeyList);
            }


            SqlParameter[] parms = {
                                        new SqlParameter("@chid", chId),
                                        new SqlParameter("@uid", uid)        
                                    };
            string commandText = string.Empty;
            if (string.IsNullOrEmpty(selectedCartItemKeyList))
                commandText = "SELECT sum(discountprice * buycount) FROM [hlh_orderproducts] a LEFT JOIN [hlh_channelproducts] b ON a.pid = b.pid WHERE chid=@chid AND uid=@uid AND oid=0";
            else
                commandText = string.Format("SELECT sum(discountprice * buycount) FROM [hlh_orderproducts] a LEFT JOIN [hlh_channelproducts] b ON a.pid = b.pid WHERE chid=@chid AND uid=@uid AND oid=0  AND a.[pid] in ({0})", keyStr);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    Sum = TypeHelper.ObjectToDecimal(reader[0]);
                }
                reader.Close();
            }

            return Sum;

        }

        /// <summary>
        /// 获得产品所属的频道集合
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<int> GetProductChannels(int pid)
        {
            List<int> chidlist = new List<int>();
            SqlParameter[] parms = {
                                        new SqlParameter("@pid", pid)    
                                    };
            string commandText = string.Format(@"SELECT [chid] FROM [{0}channelproducts]  WHERE [pid]=@pid ", RDBSHelper.RDBSTablePre);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms))
            {
                while (reader.Read())
                {
                    chidlist.Add(TypeHelper.ObjectToInt(reader[0]));
                }
                reader.Close();
            }
            return chidlist;
        }

        /// <summary>
        /// 获得产品列表所属的频道集合
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static List<int> GetProductChannelsForPids(string pids)
        {
            List<int> chidlist = new List<int>();
            
            string commandText = string.Format(@"SELECT [chid] FROM [{0}channelproducts]  WHERE [pid] in ({1}) ", RDBSHelper.RDBSTablePre,pids);

            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    chidlist.Add(TypeHelper.ObjectToInt(reader[0]));
                }
                reader.Close();
            }
            return chidlist;
        }

        #endregion

        public static string GetAllChildrenCateory(int cateId, int chId)
        {
            List<CategoryInfo> AllCateory = Categories.GetCategoryList();
            List<CategoryInfo> frist = Channel.GetChannelFristCateoryList(chId);
            List<CategoryInfo> second = new List<CategoryInfo>();
            List<CategoryInfo> third = new List<CategoryInfo>();
            List<CategoryInfo> checkCateory = new List<CategoryInfo>();
            CategoryInfo currentCateory = new CategoryInfo();
            StringBuilder CatePath = new StringBuilder();
            if (cateId > 0)
            {
                currentCateory = Categories.GetCategoryById(cateId);

                if (currentCateory != null)
                {
                    string[] cateNav = StringHelper.SplitString(currentCateory.Path);
                    foreach (var item in cateNav)
                    {
                        CategoryInfo itemInfo = AllCateory.Find(x => x.CateId == TypeHelper.StringToInt(item));

                        checkCateory.Add(itemInfo);
                    }
                    if (currentCateory.ParentId == 0 && currentCateory.Layer == 1)
                    {
                        second = AllCateory.FindAll(x => x.ParentId == cateId);
                        List<CategoryInfo> tmpCate = new List<CategoryInfo>();
                        second.ForEach(x =>
                        {
                            tmpCate.AddRange(AllCateory.FindAll(k => k.ParentId == x.CateId));
                        });
                        CatePath.Append(currentCateory.Path);
                        if (second.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", second.Select(x => x.Path)));
                        }
                        if (tmpCate.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", tmpCate.Select(x => x.Path)));
                        }
                    }
                    if (currentCateory.Layer == 2)
                    {
                        CategoryInfo pCateory = Categories.GetCategoryById(currentCateory.ParentId);
                        second = AllCateory.FindAll(x => x.ParentId == pCateory.CateId);
                        third = AllCateory.FindAll(x => x.ParentId == currentCateory.CateId);
                        CatePath.Append(currentCateory.CateId);
                        if (third.Count > 0)
                        {
                            CatePath.Append(",");
                            CatePath.Append(string.Join(",", third.Select(x => x.Path)));
                        }
                    }
                    if (currentCateory.Layer == 3)
                    {
                        string[] path = currentCateory.Path.Split(',');
                        second = AllCateory.FindAll(x => x.ParentId == TypeHelper.StringToInt(path[0]));
                        third = third = AllCateory.FindAll(x => x.ParentId == currentCateory.ParentId);
                        CatePath.Append(currentCateory.CateId);
                    }
                }
            }
            return CatePath.ToString();
        }

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


        /// <summary>
        /// 统计根据条件获取的记录数量
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public int GetChannelCount(string where)
        {
            return GetCount(where);
        }
    }
}
