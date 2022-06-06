using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VMall.Core;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace VMall.Services
{
    public class OrderReturn
    {
        /// <summary>
        /// 创建退货订单
        /// </summary>
        /// <param name="orderChange">换货model</param>
        public static void CreateOrderReturn(OrderReturnInfo orderReturn)
        {
            DbParameter[] parms = new DbParameter[] {
                                            SqlHelper.CreateInParam("@creationdate", SqlDbType.DateTime, 8, orderReturn.CreationDate),
                                            SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, orderReturn.LastModifity),
                                            SqlHelper.CreateInParam("@storeid", SqlDbType.Int, 4, orderReturn.StoreId),
                                            SqlHelper.CreateInParam("@storename", SqlDbType.NVarChar, 60, orderReturn.StoreName),
                                            SqlHelper.CreateInParam("@oid", SqlDbType.Int, 4, orderReturn.Oid),
                                            SqlHelper.CreateInParam("@osn", SqlDbType.VarChar, 30, orderReturn.OSN),
                                            SqlHelper.CreateInParam("@uid", SqlDbType.Int, 4, orderReturn.Uid),
                                            SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, orderReturn.State),
                                            SqlHelper.CreateInParam("@returndesc", SqlDbType.NVarChar, 300, orderReturn.ReturnDesc),
                                            SqlHelper.CreateInParam("@returnshipfee", SqlDbType.Decimal, 8, orderReturn.ReturnShipFee),
                                            SqlHelper.CreateInParam("@returnshipdesc", SqlDbType.NVarChar, 300, orderReturn.ReturnShipDesc)
            };
            string sqlStr = @"INSERT INTO [hlh_orderreturn] ([creationdate],[lastmodifity],[storeid],[storename],[oid],[osn],[uid],[state],[returndesc],[returnshipfee],[returnshipdesc])  VALUES (@creationdate,@lastmodifity,@storeid,@storename,@oid,@osn,@uid,@state,@returndesc,@returnshipfee,@returnshipdesc) ";
            RDBSHelper.ExecuteScalar(CommandType.Text, sqlStr, parms);
        }

        #region 列表
        /// <summary>
        /// 获取退货订单列表
        /// </summary>
        /// <param name="state">处理状态</param>
        /// <returns></returns>
        public static List<OrderReturnInfo> GetOrderReturnList(int pageSize, int pageNumber, string condition)
        {
            List<OrderReturnInfo> returnInfoList = new List<OrderReturnInfo>();
            bool noCondition = string.IsNullOrWhiteSpace(condition);
            string commandText;
            if (pageNumber == 1)
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderreturn] ORDER BY [returnid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre);

                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderreturn] WHERE {2} ORDER BY [returnid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                condition);
            }
            else
            {
                if (noCondition)
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderreturn] WHERE [returnid] < (SELECT MIN([returnid]) FROM (SELECT TOP {2} [returnid] FROM [{1}orderreturn] ORDER BY [returnid] DESC) AS [temp]) ORDER BY [returnid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize);
                else
                    commandText = string.Format("SELECT TOP {0} * FROM [{1}orderreturn] WHERE [returnid] < (SELECT MIN([returnid]) FROM (SELECT TOP {2} [returnid] FROM [{1}orderreturn] WHERE {3} ORDER BY [returnid] DESC) AS [temp]) AND {3} ORDER BY [returnid] DESC",
                                                pageSize,
                                                RDBSHelper.RDBSTablePre,
                                                (pageNumber - 1) * pageSize,
                                                condition);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText))
            {
                while (reader.Read())
                {
                    OrderReturnInfo returnInfo = BuildOrderReturnInfoFromReader(reader);
                    returnInfoList.Add(returnInfo);
                }
                reader.Close();
            }
            return returnInfoList;
        }


        /// <summary>
        /// 获得订单退货列表条件
        /// </summary>
        /// <param name="storeId">店铺id</param>
        /// <param name="osn">订单编号</param>
        /// <returns></returns>
        public static string GetOrderReturnListCondition(int storeId, string osn, int state)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            condition.AppendFormat(" AND [state] = {0} ", state);
            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }

        public static string GetOrderReturnApplyListCondition(int storeId, string osn, int uid, string consignee, int orderState, DateTime? startDate, DateTime? endDate)
        {
            StringBuilder condition = new StringBuilder();

            if (storeId > 0)
                condition.AppendFormat(" AND [storeid] = {0} ", storeId);
            if (!string.IsNullOrWhiteSpace(osn))
                condition.AppendFormat(" AND [osn] like '{0}%' ", osn);
            if (uid > 0)
                condition.AppendFormat(" AND [uid] = {0} ", uid);
            if (!string.IsNullOrWhiteSpace(consignee))
                condition.AppendFormat(" AND [consignee] like '{0}%' ", consignee);
            if (orderState > 0)
                condition.AppendFormat(" AND [orderstate] = {0} AND [returntype]=1 ", orderState);
            if (startDate.HasValue)
                condition.AppendFormat(" AND [addtime] >= '{0}' ", startDate.Value.Date.ToString("yyyy-MM-dd HH:mm:ss"));
            if (endDate.HasValue)
                condition.AppendFormat(" AND [addtime] <= '{0}' ", endDate.Value.Date.AddDays(1).AddSeconds(-1).ToString("yyyy-MM-dd HH:mm:ss"));

            return condition.Length > 0 ? condition.Remove(0, 4).ToString() : "";
        }
        /// <summary>
        /// 获得订单退货数量
        /// </summary>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        public static int GetOrderReturnCount(string condition)
        {
            string commandText;
            if (string.IsNullOrWhiteSpace(condition))
                commandText = string.Format("SELECT COUNT(returnid) FROM [{0}orderreturn]", RDBSHelper.RDBSTablePre);
            else
                commandText = string.Format("SELECT COUNT(returnid) FROM [{0}orderreturn] WHERE {1}", RDBSHelper.RDBSTablePre, condition);

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, commandText), 0);
        }

        #endregion

        /// <summary>
        /// 根据oid取得换货记录
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <returns></returns>
        public static OrderReturnInfo GetOrderReturnByOid(int oid)
        {
            OrderReturnInfo returnInfo = null;
            DbParameter[] parms =  { 
                                         SqlHelper.CreateInParam("@oid", SqlDbType.Int, 4, oid)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderreturn] WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);

            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
            if (reader.Read())
            {
                returnInfo = BuildOrderReturnInfoFromReader(reader);
            }
            reader.Close();
            return returnInfo;
        }

        /// <summary>
        /// 根据returnid取得退货记录
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <returns></returns>
        public static OrderReturnInfo GetMergeOrderBySubOid(int returnid)
        {
            OrderReturnInfo returnInfo = null;
            DbParameter[] parms =  { 
                                         SqlHelper.CreateInParam("@returnid", SqlDbType.Int, 4, returnid)
                                    };
            string commandText = string.Format("SELECT * FROM [{0}orderreturn] WHERE [returnid]=@returnid", RDBSHelper.RDBSTablePre);

            IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, commandText, parms);
            if (reader.Read())
            {
                returnInfo = BuildOrderReturnInfoFromReader(reader);
            }
            reader.Close();
            return returnInfo;
        }

        /// <summary>
        /// 更新退货记录--由未处理更新为已处理
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void UpdateOrderReturn(int returnid, int state, DateTime lastModifity, string returnDesc = "")
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@returnid", SqlDbType.Int, 4, returnid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity),
                                    SqlHelper.CreateInParam("@returndesc", SqlDbType.NVarChar, 300, returnDesc)
                                   };
            string commandText = string.Format("UPDATE [{0}orderreturn] SET [state]=@state,[lastmodifity]=@lastmodifity,[returndesc]=@returndesc WHERE [returnid]=@returnid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 更新退货记录--由未处理更新为已处理
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void UpdateOrderReturnByOid(int oid, int state, DateTime lastModifity)
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@oid", SqlDbType.Int, 4, oid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity)
                                   };
            string commandText = string.Format("UPDATE [{0}orderreturn] SET [state]=@state,[lastmodifity]=@lastmodifity WHERE [oid]=@oid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }
        /// <summary>
        /// 确认退货记录--该为审核通过2 并增加承担运费与运费说明
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void ConfirmOrderReturn(int returnid, int state, DateTime lastModifity, decimal returnShipfee, string shipfeeDesc)
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@returnid", SqlDbType.Int, 4, returnid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity),
                                    SqlHelper.CreateInParam("@returnshipfee", SqlDbType.Decimal, 8, returnShipfee), 
                                    SqlHelper.CreateInParam("@returnshipdesc", SqlDbType.NVarChar, 300, shipfeeDesc)

                                   };
            string commandText = string.Format("UPDATE [{0}orderreturn] SET [state]=@state,[lastmodifity]=@lastmodifity,[returnshipfee]=@returnshipfee,[returnshipdesc]=@returnshipdesc WHERE [returnid]=@returnid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }

        /// <summary>
        /// 确认退货记录2--该为审核通过2 并追加承担运费与运费说明
        /// </summary>
        /// <param name="returnid">记录id</param>
        /// <param name="state">状态</param>

        /// <param name="lastModifity">最后修改时间</param>
        public static void ConfirmOrderReturn2(int returnid, int state, DateTime lastModifity, decimal returnShipfee, string shipfeeDesc, decimal newReturnShipfee, string newShipfeeDesc)
        {
            DbParameter[] parms = {
                                    SqlHelper.CreateInParam("@returnid", SqlDbType.Int, 4, returnid),
                                    SqlHelper.CreateInParam("@state", SqlDbType.TinyInt, 1, state), 
                                    SqlHelper.CreateInParam("@lastmodifity", SqlDbType.DateTime, 8, lastModifity),
                                    SqlHelper.CreateInParam("@returnshipfee", SqlDbType.Decimal, 8, returnShipfee+newReturnShipfee), 
                                    SqlHelper.CreateInParam("@returnshipdesc", SqlDbType.NVarChar, 300, shipfeeDesc+newShipfeeDesc)

                                   };
            string commandText = string.Format("UPDATE [{0}orderreturn] SET [state]=@state,[lastmodifity]=@lastmodifity,[returnshipfee]=@returnshipfee,[returnshipdesc]=@returnshipdesc WHERE [returnid]=@returnid",
                                                RDBSHelper.RDBSTablePre);
            RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
        }


        #region 后台申请单个退款

        /// <summary>
        /// 创建单个退款订单
        /// </summary>
        /// <param name="orderInfo">订单信息</param>
        /// <param name="type">类型，1-为多个同样产品退一件(需要更改数量，并插入一条新记录)，2-不同产品退一件（更改订单产品记录）</param>
        /// <param name="orderProductList">订单商品列表</param>
        /// <returns>订单id</returns>
        public int CreateAdminReturnOrder(OrderInfo orderInfo, int type, OrderProductInfo orderProductInfo, decimal returnAmount, int returnCount)
        {
            #region

            DbParameter[] parms = {
	                                    GenerateInParam("@osn", SqlDbType.Char,30,orderInfo.OSN),
	                                    GenerateInParam("@uid", SqlDbType.Int,4 ,orderInfo.Uid),
	                                    GenerateInParam("@orderstate", SqlDbType.TinyInt,1 ,orderInfo.OrderState),
                                        GenerateInParam("@productamount", SqlDbType.Decimal,8 ,orderInfo.ProductAmount),
                                        GenerateInParam("@orderamount", SqlDbType.Decimal,8 ,orderInfo.OrderAmount),
                                        GenerateInParam("@surplusmoney", SqlDbType.Decimal,8 ,orderInfo.SurplusMoney),
                                        GenerateInParam("@parentid", SqlDbType.Int,4 ,orderInfo.ParentId),
                                        GenerateInParam("@isreview", SqlDbType.TinyInt,1 ,orderInfo.IsReview),
                                        GenerateInParam("@addtime", SqlDbType.DateTime, 8,orderInfo.AddTime),
                                        GenerateInParam("@storeid", SqlDbType.Int,4 ,orderInfo.StoreId),
                                        GenerateInParam("@storename", SqlDbType.NChar,60 ,orderInfo.StoreName),
                                        GenerateInParam("@shipsn", SqlDbType.Char,30 ,orderInfo.ShipSN),
                                        GenerateInParam("@shipcoid", SqlDbType.SmallInt,2 ,orderInfo.ShipCoId),
                                        GenerateInParam("@shipconame", SqlDbType.NChar,30 ,orderInfo.ShipCoName),
                                        GenerateInParam("@shiptime", SqlDbType.DateTime, 8,orderInfo.ShipTime),
                                        GenerateInParam("@paysn", SqlDbType.Char,30 ,orderInfo.PaySN),
                                        GenerateInParam("@paysystemname", SqlDbType.Char,20 ,orderInfo.PaySystemName),
                                        GenerateInParam("@payfriendname", SqlDbType.NChar,30 ,orderInfo.PayFriendName),
                                        GenerateInParam("@paymode", SqlDbType.TinyInt,1 ,orderInfo.PayMode),
                                        GenerateInParam("@paytime", SqlDbType.DateTime, 8,orderInfo.PayTime),
                                        GenerateInParam("@regionid", SqlDbType.SmallInt,2 ,orderInfo.RegionId),
                                        GenerateInParam("@consignee", SqlDbType.NVarChar,30 ,orderInfo.Consignee),
                                        GenerateInParam("@mobile", SqlDbType.VarChar,15 ,orderInfo.Mobile),
                                        GenerateInParam("@phone", SqlDbType.VarChar,12 ,orderInfo.Phone),
                                        GenerateInParam("@email", SqlDbType.VarChar,50 ,orderInfo.Email),
                                        GenerateInParam("@zipcode", SqlDbType.Char,6 ,orderInfo.ZipCode),
	                                    GenerateInParam("@address", SqlDbType.NVarChar,150 ,orderInfo.Address),
                                        GenerateInParam("@besttime", SqlDbType.DateTime,8 ,orderInfo.BestTime),
	                                    GenerateInParam("@shipfee", SqlDbType.Decimal,8 ,orderInfo.ShipFee),
                                        GenerateInParam("@payfee", SqlDbType.Decimal,8 ,orderInfo.PayFee),
                                        GenerateInParam("@fullcut", SqlDbType.Int,4 ,orderInfo.FullCut),
	                                    GenerateInParam("@discount", SqlDbType.Decimal,8 ,orderInfo.Discount),
	                                    GenerateInParam("@paycreditcount", SqlDbType.Int,4 ,orderInfo.PayCreditCount),
	                                    GenerateInParam("@paycreditmoney", SqlDbType.Decimal,8 ,orderInfo.PayCreditMoney),
                                        GenerateInParam("@couponmoney", SqlDbType.Decimal,8 ,orderInfo.CouponMoney),
	                                    GenerateInParam("@weight", SqlDbType.Int,4 ,orderInfo.Weight),
                                        GenerateInParam("@buyerremark", SqlDbType.NVarChar,250 ,orderInfo.BuyerRemark),
                                        GenerateInParam("@ip", SqlDbType.VarChar,15 ,orderInfo.IP),
                                        GenerateInParam("@ordersource", SqlDbType.VarChar,15 ,orderInfo.OrderSource),
                                        GenerateInParam("@taxamount",SqlDbType.Decimal,8 ,orderInfo.TaxAmount),
                                        GenerateInParam("@invoice",SqlDbType.NVarChar,60 ,orderInfo.Invoice),
                                        GenerateInParam("@haimidiscount",SqlDbType.Decimal,8 ,orderInfo.HaiMiDiscount),
                                        GenerateInParam("@hongbaodiscount",SqlDbType.Decimal,8 ,orderInfo.HongBaoDiscount),
                                        GenerateInParam("@cashdiscount",SqlDbType.Decimal,8 ,orderInfo.CashDiscount),
                                        GenerateInParam("@agentdiscount",SqlDbType.Decimal,8 ,orderInfo.AgentDiscount),
                                        GenerateInParam("@commisiondiscount",SqlDbType.Decimal,8 ,orderInfo.CommisionDiscount),
                                        GenerateInParam("@mallsource",SqlDbType.Int,4 ,orderInfo.MallSource),
                                        GenerateInParam("@actualuid",SqlDbType.Int,4 ,orderInfo.ActualUid),
                                        GenerateInParam("@rewarddiscount",SqlDbType.Decimal,8 ,0M)
                                    };
            #endregion

            int oid = TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.StoredProcedure, string.Format("{0}createorder", RDBSHelper.RDBSTablePre), parms), -1);

            if (oid > 0)
            {
                //更新原来订单金额
                UpdateOrderAmountForAdminSingleReturn(orderInfo.ParentId, returnAmount);
                if (type == 2)
                {
                    parms = new DbParameter[] {
                                                GenerateInParam("@recordid", SqlDbType.Int, 4, orderProductInfo.RecordId),
                                                GenerateInParam("@oid", SqlDbType.Int, 4, oid),
                                              };
                    string commandText = "UPDATE [hlh_orderproducts] SET [oid]=@oid,[productpv]=-[productpv] WHERE [recordid] =@recordid";
                    RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms);
                }
                else if (type == 1)
                {
                    UpdateOrderProCountForAdminSingleReturn(orderInfo.ParentId, returnCount);

                    orderProductInfo.Oid = oid;
                    parms = new DbParameter[] {
                                                GenerateInParam("@oid", SqlDbType.Int, 4, orderProductInfo.Oid),
                                                GenerateInParam("@uid", SqlDbType.Int, 4, orderProductInfo.Uid),
                                                GenerateInParam("@sid", SqlDbType.Char, 16, orderProductInfo.Sid),
                                                GenerateInParam("@pid", SqlDbType.Int, 4, orderProductInfo.Pid),
                                                GenerateInParam("@psn", SqlDbType.Char, 30, orderProductInfo.PSN),
                                                GenerateInParam("@cateid", SqlDbType.SmallInt, 2, orderProductInfo.CateId),
                                                GenerateInParam("@brandid", SqlDbType.Int, 4, orderProductInfo.BrandId),
                                                GenerateInParam("@storeid", SqlDbType.Int, 4, orderProductInfo.StoreId),
                                                GenerateInParam("@storecid", SqlDbType.Int, 4, orderProductInfo.StoreCid),
                                                GenerateInParam("@storestid", SqlDbType.Int, 4, orderProductInfo.StoreSTid),
                                                GenerateInParam("@name", SqlDbType.NVarChar, 200, orderProductInfo.Name),
                                                GenerateInParam("@showimg", SqlDbType.NVarChar, 100, orderProductInfo.ShowImg),
                                                GenerateInParam("@discountprice", SqlDbType.Decimal, 4, orderProductInfo.DiscountPrice),
                                                GenerateInParam("@costprice", SqlDbType.Decimal, 4, orderProductInfo.CostPrice),
                                                GenerateInParam("@shopprice", SqlDbType.Decimal, 4, orderProductInfo.ShopPrice),
                                                GenerateInParam("@marketprice", SqlDbType.Decimal, 4, orderProductInfo.MarketPrice),
                                                GenerateInParam("@weight", SqlDbType.Int, 4, orderProductInfo.Weight),
                                                GenerateInParam("@isreview", SqlDbType.TinyInt, 1, orderProductInfo.IsReview),
                                                GenerateInParam("@realcount", SqlDbType.Int, 4, returnCount),
                                                GenerateInParam("@buycount", SqlDbType.Int, 4,returnCount),
                                                GenerateInParam("@sendcount", SqlDbType.Int, 4, orderProductInfo.SendCount),
                                                GenerateInParam("@type", SqlDbType.TinyInt, 1, orderProductInfo.Type),
                                                GenerateInParam("@paycredits", SqlDbType.Int, 4, orderProductInfo.PayCredits),
                                                GenerateInParam("@coupontypeid", SqlDbType.Int, 4, orderProductInfo.CouponTypeId),
                                                GenerateInParam("@extcode1", SqlDbType.Int, 4, orderProductInfo.ExtCode1),
                                                GenerateInParam("@extcode2", SqlDbType.Int, 4, orderProductInfo.ExtCode2),
                                                GenerateInParam("@extcode3", SqlDbType.Int, 4, orderProductInfo.ExtCode3),
                                                GenerateInParam("@extcode4", SqlDbType.Int, 4, orderProductInfo.ExtCode4),
                                                GenerateInParam("@extcode5", SqlDbType.Int, 4, orderProductInfo.ExtCode5),
                                                GenerateInParam("@addtime", SqlDbType.DateTime, 8, orderProductInfo.AddTime),
                                                GenerateInParam("@producthaimi", SqlDbType.Decimal, 8, orderProductInfo.ProductHaiMi),
                                                GenerateInParam("@productpv", SqlDbType.Decimal, 8, -orderProductInfo.ProductPV),
                                                GenerateInParam("@producthbcut", SqlDbType.Decimal, 8, orderProductInfo.ProductHBCut),
                                                GenerateInParam("@mallsource", SqlDbType.Int, 4, 0)
                                            };
                    RDBSHelper.ExecuteNonQuery(CommandType.StoredProcedure,
                                               string.Format("{0}addorderproduct", RDBSHelper.RDBSTablePre),
                                               parms);

                }

            }

            return oid;
        }

        /// <summary>
        /// 修改订单价格-单个产品退货
        /// </summary>
        public static bool UpdateOrderAmountForAdminSingleReturn(int oid, decimal returnAmount)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@returnAmount",returnAmount)
                                    };
            string commandText = string.Format("UPDATE [{0}orders] SET [productamount]=[productamount]-@returnAmount,[orderamount]=[orderamount]-@returnAmount,[surplusmoney]=[surplusmoney]-@returnAmount WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        /// <summary>
        /// 修改订单产品数量-单个产品退货
        /// </summary>
        public static bool UpdateOrderProCountForAdminSingleReturn(int oid, decimal returnCount)
        {
            SqlParameter[] parms =  {
                                       new SqlParameter("@oid",oid),
                                       new SqlParameter("@returnCount",returnCount)
                                    };
            string commandText = string.Format("UPDATE [{0}orderproducts] SET [buycount]=[buycount]-@returnCount,[realcount]=[realcount]-@returnCount WHERE [oid]=@oid", RDBSHelper.RDBSTablePre);
            return RDBSHelper.ExecuteNonQuery(CommandType.Text, commandText, parms) > 0;
        }
        #region  辅助方法

        /// <summary>
        /// 生成输入参数
        /// </summary>
        /// <param name="paramName">参数名</param>
        /// <param name="sqlDbType">参数类型</param>
        /// <param name="size">类型大小</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        private DbParameter GenerateInParam(string paramName, SqlDbType sqlDbType, int size, object value)
        {
            SqlParameter param = new SqlParameter(paramName, sqlDbType, size);
            param.Direction = ParameterDirection.Input;
            if (value != null)
                param.Value = value;
            return param;
        }

        #endregion

        #endregion


        #region 辅助方法
        /// <summary>
        /// 从IDataReader创建ProductInfo
        /// </summary>
        public static OrderReturnInfo BuildOrderReturnInfoFromReader(IDataReader reader)
        {
            OrderReturnInfo returnInfo = new OrderReturnInfo();
            returnInfo.ReturnId = TypeHelper.ObjectToInt(reader["returnid"]);
            returnInfo.CreationDate = TypeHelper.ObjectToDateTime(reader["creationdate"]);
            returnInfo.LastModifity = TypeHelper.ObjectToDateTime(reader["lastmodifity"]);
            returnInfo.StoreId = TypeHelper.ObjectToInt(reader["storeid"]);
            returnInfo.StoreName = reader["storename"].ToString();
            returnInfo.Oid = TypeHelper.ObjectToInt(reader["oid"]);
            returnInfo.OSN = reader["osn"].ToString();
            returnInfo.Uid = TypeHelper.ObjectToInt(reader["uid"]);
            returnInfo.State = TypeHelper.ObjectToInt(reader["state"]);
            returnInfo.ReturnDesc = reader["returndesc"].ToString();
            returnInfo.ReturnShipFee = TypeHelper.ObjectToDecimal(reader["returnshipfee"]);
            returnInfo.ReturnShipDesc = reader["returnshipdesc"].ToString();

            return returnInfo;
        }

        #endregion


    }
}
