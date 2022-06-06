using System;
using System.Collections.Generic;

using VMall.Core;
using System.Data.SqlClient;
using System.Text;
using System.Data;

namespace VMall.Services
{
    /// <summary>
    /// 配送公司操作管理类
    /// </summary>
    public partial class ShipCompanies
    {
        /// <summary>
        /// 获得配送公司列表
        /// </summary>
        /// <param name="pageSize">每页数</param>
        /// <param name="pageNumber">当前页数</param>
        /// <returns></returns>
        public static List<ShipCompanyInfo> GetShipCompanyList(int pageSize, int pageNumber)
        {
            return VMall.Data.ShipCompanies.GetShipCompanyList(pageSize, pageNumber);
        }

        /// <summary>
        /// 获得配送公司数量
        /// </summary>
        /// <returns></returns>
        public static int GetShipCompanyCount()
        {
            return VMall.Data.ShipCompanies.GetShipCompanyCount();
        }

        /// <summary>
        /// 获得配送公司
        /// </summary>
        /// <param name="shipCoId">配送公司id</param>
        /// <returns></returns>
        public static ShipCompanyInfo GetShipCompanyById(int shipCoId)
        {
            ShipCompanyInfo shipCompanyInfo = VMall.Core.BMACache.Get(CacheKeys.MALL_SHIPCOMPANY_INFO + shipCoId) as ShipCompanyInfo;
            if (shipCompanyInfo == null)
            {
                shipCompanyInfo = VMall.Data.ShipCompanies.GetShipCompanyById(shipCoId);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_SHIPCOMPANY_INFO + shipCoId, shipCompanyInfo);
            }

            return shipCompanyInfo;
        }
        /// <summary>
        /// 获得配送公司
        /// </summary>
        /// <param name="shipCoId">配送公司id</param>
        /// <returns></returns>
        public static ShipCompanyInfo GetShipCompanyByName(string condition)
        {
            ShipCompanyInfo Info = null;
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(condition))
            {
                sb.Append(" where ");
                sb.Append(condition);
            }
            string commandText = "select top 1 * from hlh_shipcompanies " + sb.ToString();
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    Info = VMall.Data.ShipCompanies.BuildShipCompanyFromReader(reader);
                }
                reader.Close();
            }
            return Info;
        }
    }
}
