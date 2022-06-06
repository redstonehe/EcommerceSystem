using System;
using System.Web;
using System.Collections.Generic;
using System.Text;
using VMall.Core;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace VMall.Services
{
    /// <summary>
    /// 区域操作管理类
    /// </summary>
    public partial class Regions
    {
        /// <summary>
        /// 获得全部区域
        /// </summary>
        /// <returns></returns>
        public static List<RegionInfo> GetAllRegion()
        {
            return VMall.Data.Regions.GetAllRegion();
        }

        /// <summary>
        /// 获得省列表
        /// </summary>
        /// <returns></returns>
        public static List<RegionInfo> GetProvinceList()
        {
            List<RegionInfo> provinceList = VMall.Core.BMACache.Get(CacheKeys.MALL_REGION_CHILDLIST + 0) as List<RegionInfo>;
            if (provinceList == null)
            {
                provinceList = GetRegionList(0);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_REGION_CHILDLIST + 0, provinceList);
            }
            return provinceList.OrderBy(x => x.RegionId).ToList();
        }

        /// <summary>
        /// 获得市列表
        /// </summary>
        /// <param name="provinceId">省id</param>
        /// <returns></returns>
        public static List<RegionInfo> GetCityList(int provinceId)
        {
            List<RegionInfo> cityList = VMall.Core.BMACache.Get(CacheKeys.MALL_REGION_CHILDLIST + provinceId) as List<RegionInfo>;
            if (cityList == null)
            {
                cityList = GetRegionList(provinceId);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_REGION_CHILDLIST + provinceId, cityList);
            }
            return cityList.OrderBy(x => x.RegionId).ToList();
        }

        /// <summary>
        /// 获得县或区列表
        /// </summary>
        /// <param name="cityId">市id</param>
        /// <returns></returns>
        public static List<RegionInfo> GetCountyList(int cityId)
        {
            List<RegionInfo> countyList = VMall.Core.BMACache.Get(CacheKeys.MALL_REGION_CHILDLIST + cityId) as List<RegionInfo>;
            if (countyList == null)
            {
                countyList = GetRegionList(cityId);
                VMall.Core.BMACache.Insert(CacheKeys.MALL_REGION_CHILDLIST + cityId, countyList);
            }
            return countyList.OrderBy(x => x.RegionId).ToList();
        }

        /// <summary>
        /// 获得区域列表
        /// </summary>
        /// <param name="parentId">父id</param>
        /// <returns></returns>
        public static List<RegionInfo> GetRegionList(int parentId)
        {
            return VMall.Data.Regions.GetRegionList(parentId);
        }

        /// <summary>
        /// 获得区域
        /// </summary>
        /// <param name="regionId">区域id</param>
        /// <returns></returns>
        public static RegionInfo GetRegionById(int regionId)
        {
            if (regionId > 0)
            {
                RegionInfo regionInfo = VMall.Core.BMACache.Get(CacheKeys.MALL_REGION_INFOBYID + regionId) as RegionInfo;
                if (regionInfo == null)
                {
                    regionInfo = VMall.Data.Regions.GetRegionById(regionId);
                    VMall.Core.BMACache.Insert(CacheKeys.MALL_REGION_INFOBYID + regionId, regionInfo);
                }
                return regionInfo;
            }
            return null;
        }

        /// <summary>
        /// 获得区域
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="layer">级别</param>
        /// <returns></returns>
        public static RegionInfo GetRegionByNameAndLayer(string name, int layer)
        {
            RegionInfo regionInfo = VMall.Core.BMACache.Get(string.Format(CacheKeys.MALL_REGION_INFOBYNAMEANDLAYER, name, layer)) as RegionInfo;
            if (regionInfo == null)
            {
                regionInfo = VMall.Data.Regions.GetRegionByNameAndLayer(name, layer);
                VMall.Core.BMACache.Insert(string.Format(CacheKeys.MALL_REGION_INFOBYNAMEANDLAYER, name, layer), regionInfo);
            }
            return regionInfo;
        }

        /// <summary>
        /// 获取IP对应区域
        /// </summary>
        /// <param name="ip">ip</param>
        /// <returns></returns>
        public static RegionInfo GetRegionByIP(string ip)
        {
            RegionInfo regionInfo = null;
            HttpCookie cookie = HttpContext.Current.Request.Cookies["position"];
            if (cookie != null)
            {
                if (cookie["ip"] == ip)
                {
                    regionInfo = GetRegionById(TypeHelper.StringToInt(cookie["regionid"]));
                }
                else
                {
                    cookie.Values["ip"] = ip;
                    regionInfo = IPSearch.SearchRegion(ip);
                    if (regionInfo != null)
                        cookie.Values["regionid"] = regionInfo.RegionId.ToString();
                    else
                        cookie.Values["regionid"] = "-1";
                    cookie.Expires = DateTime.Now.AddYears(1);

                    HttpContext.Current.Response.AppendCookie(cookie);
                }

            }
            else
            {
                cookie = new HttpCookie("position");
                cookie.Values["ip"] = ip;
                regionInfo = IPSearch.SearchRegion(ip);
                if (regionInfo != null)
                    cookie.Values["regionid"] = regionInfo.RegionId.ToString();
                else
                    cookie.Values["regionid"] = "-1";
                cookie.Expires = DateTime.Now.AddYears(1);

                HttpContext.Current.Response.AppendCookie(cookie);
            }

            if (regionInfo != null)
                return regionInfo;
            else
                return new RegionInfo() { RegionId = -1, Name = "未知区域" };
        }

        /// <summary>
        /// 根据regionid集合获得省列表
        /// </summary>
        /// <param name="regionIds">多个regionid集合</param>
        /// <returns></returns>
        public static string GetRegionNamesByregionids(string regionIds, string cityids)
        {
            List<string> regionNames = new List<string>();
            StringBuilder showName = new StringBuilder();
            List<RegionInfo> regionList = new List<RegionInfo>();
            string ids = regionIds + (string.IsNullOrEmpty(cityids) ? "" : "," + cityids);
            string commandText = string.Format("SELECT * FROM dbo.hlh_regions WHERE regionid IN ({0}) ", ids);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    RegionInfo info = VMall.Data.Regions.BuildRegionFromReader(reader);
                    regionList.Add(info);
                    regionNames.Add(reader[0].ToString());
                }
                reader.Close();
            }
            foreach (var item in regionList.FindAll(x => x.Layer == 1))
            {
                showName.Append(item.Name);
                List<RegionInfo> cityList = regionList.FindAll(x => x.Layer == 2 && x.ParentId == item.RegionId);
                if (cityList.Count > 0)
                {
                    showName.Append("(");
                    foreach (var info in cityList)
                    {
                        showName.Append(info.Name + ",");
                    }
                    showName.Remove(showName.Length - 1, 1);
                    showName.Append("),");
                }
                else
                {
                    showName.Append(",");
                }
            }
            showName.Remove(showName.Length - 1, 1);
            return showName.ToString();
            //return string.Join(",", regionNames);
        }

        /// <summary>
        /// 根据获得区域及所有父级区域
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="layer">级别</param>
        /// <returns></returns>
        public static List<RegionInfo> GetRegionAndParentRegionById(int regionId)
        {
            List<RegionInfo> regionList = new List<RegionInfo>();
            SqlParameter[] param = {
					new SqlParameter("@regionid", SqlDbType.Int,4)
			};
            param[0].Value = regionId;
            string commandText = string.Format(@"SELECT * FROM dbo.hlh_regions WHERE  regionid IN (
                                                    SELECT @regionid 
		                                            UNION
		                                            SELECT parentid FROM dbo.hlh_regions WHERE regionid=@regionid
		                                            UNION  SELECT parentid FROM dbo.hlh_regions WHERE regionid IN  (SELECT parentid FROM dbo.hlh_regions WHERE regionid=@regionid)
		                                        ) ",
                                                RDBSHelper.RDBSTablePre,
                                                RDBSFields.REGIONS);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, param))
            {
                while (reader.Read())
                {
                    RegionInfo regionInfo = VMall.Data.Regions.BuildRegionFromReader(reader);
                    regionList.Add(regionInfo);
                }
                reader.Close();
            }
            return regionList;
        }
    }
}
