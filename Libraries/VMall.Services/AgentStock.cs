using System;
using System.Data;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Generic;
using VMall.Core;
using System.Linq;
namespace VMall.Services
{
    /// <summary>
    /// 数据访问类:hlh_agentstock
    /// </summary>
    public partial class AgentStock
    {
        AgentStockDetail AgentStockDetailBLL = new AgentStockDetail();
        public AgentStock()
        { }
        #region  BasicMethod
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int asid)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from hlh_agentstock");
            strSql.Append(" where asid=@asid");
            SqlParameter[] parameters = {
					new SqlParameter("@asid", SqlDbType.Int,4)
			};
            parameters[0].Value = asid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }


        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(AgentStockInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("insert into hlh_agentstock(");
            strSql.Append("uid,pid,balance,agentamount)");
            strSql.Append(" values (");
            strSql.Append("@uid,@pid,@balance,@agentamount)");
            strSql.Append(";select @@IDENTITY");
            SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@balance", SqlDbType.Decimal,10),
                    new SqlParameter("@agentamount", SqlDbType.Decimal,10)
                                        };
            parameters[0].Value = model.Uid;
            parameters[1].Value = model.Pid;
            parameters[2].Value = model.Balance;
            parameters[3].Value = model.AgentAmount;
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString(), parameters));
        }
        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(AgentStockInfo model)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update hlh_agentstock set ");
            strSql.Append("uid=@uid,");
            strSql.Append("pid=@pid,");
            strSql.Append("balance=@balance,");

            strSql.Append("agentamount=@agentamount");
            strSql.Append(" where asid=@asid");
            SqlParameter[] parameters = {
					new SqlParameter("@uid", SqlDbType.Int,4),
					new SqlParameter("@pid", SqlDbType.Int,4),
					new SqlParameter("@balance", SqlDbType.Decimal,10),
                    new SqlParameter("@agentamount", SqlDbType.Decimal,10),
					new SqlParameter("@asid", SqlDbType.Int,4)};
            parameters[0].Value = model.Uid;
            parameters[1].Value = model.Pid;
            parameters[2].Value = model.Balance;
            parameters[3].Value = model.AgentAmount;
            parameters[4].Value = model.Asid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int asid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_agentstock ");
            strSql.Append(" where asid=@asid");
            SqlParameter[] parameters = {
					new SqlParameter("@asid", SqlDbType.Int,4)
			};
            parameters[0].Value = asid;

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString(), parameters)) > 0;
        }
        /// <summary>
        /// 批量删除数据
        /// </summary>
        public bool DeleteList(string asidlist)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from hlh_agentstock ");
            strSql.Append(" where asid in (" + asidlist + ")  ");

            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteNonQuery(CommandType.Text, strSql.ToString())) > 0;
        }


        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentStockInfo GetModel(int asid)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_agentstock ");
            strSql.Append(" where asid=@asid");
            SqlParameter[] parameters = {
					new SqlParameter("@asid", SqlDbType.Int,4)
			};
            parameters[0].Value = asid;

            AgentStockInfo model = new AgentStockInfo();
            DataSet ds = RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString(), parameters);
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentStockInfo GetModel(string strWhere)
        {

            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  top 1 * from hlh_agentstock ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }

            AgentStockInfo model = new AgentStockInfo();
            DataSet ds = RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString());
            if (ds.Tables[0].Rows.Count > 0)
            {
                return DataRowToModel(ds.Tables[0].Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentStockInfo DataRowToModel(DataRow row)
        {
            AgentStockInfo model = new AgentStockInfo();
            if (row != null)
            {
                if (row["asid"] != null && row["asid"].ToString() != "")
                {
                    model.Asid = int.Parse(row["asid"].ToString());
                }
                if (row["uid"] != null && row["uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["uid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.Pid = int.Parse(row["pid"].ToString());
                }
                if (row["balance"] != null && row["balance"].ToString() != "")
                {
                    model.Balance = decimal.Parse(row["balance"].ToString());
                }
                if (row["agentamount"] != null && row["agentamount"].ToString() != "")
                {
                    model.AgentAmount = decimal.Parse(row["agentamount"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public AgentStockInfo BuildAgentStockReader(IDataReader row)
        {
            AgentStockInfo model = new AgentStockInfo();
            if (row != null)
            {
                if (row["asid"] != null && row["asid"].ToString() != "")
                {
                    model.Asid = int.Parse(row["asid"].ToString());
                }
                if (row["uid"] != null && row["uid"].ToString() != "")
                {
                    model.Uid = int.Parse(row["uid"].ToString());
                }
                if (row["pid"] != null && row["pid"].ToString() != "")
                {
                    model.Pid = int.Parse(row["pid"].ToString());
                }
                if (row["balance"] != null && row["balance"].ToString() != "")
                {
                    model.Balance = decimal.Parse(row["balance"].ToString());
                }
                if (row["agentamount"] != null && row["agentamount"].ToString() != "")
                {
                    model.AgentAmount = decimal.Parse(row["agentamount"].ToString());
                }
            }
            return model;
        }

        /// <summary>
        /// 获得数据列表
        /// </summary>
        public List<AgentStockInfo> GetList(string strWhere)
        {
            List<AgentStockInfo> list = new List<AgentStockInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * ");
            strSql.Append(" FROM hlh_agentstock ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentStockInfo info = BuildAgentStockReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public List<AgentStockInfo> GetList(int Top, string strWhere, string filedOrder)
        {
            List<AgentStockInfo> list = new List<AgentStockInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" * ");
            strSql.Append(" FROM hlh_agentstock ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            strSql.Append(" order by " + filedOrder);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentStockInfo info = BuildAgentStockReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }

        /// <summary>
        /// 获取记录总数
        /// </summary>
        public int GetRecordCount(string strWhere)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) FROM hlh_agentstock ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            return TypeHelper.ObjectToInt(RDBSHelper.ExecuteScalar(CommandType.Text, strSql.ToString()));
        }
        /// <summary>
        /// 分页获取数据列表
        /// </summary>
        public List<AgentStockInfo> GetListByPage(string strWhere, string orderby, int startIndex, int endIndex)
        {
            List<AgentStockInfo> list = new List<AgentStockInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.asid desc");
            }
            strSql.Append(")AS Row, T.*  from hlh_agentstock T ");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);
            using (IDataReader reader = RDBSHelper.ExecuteReader(CommandType.Text, strSql.ToString()))
            {
                while (reader.Read())
                {
                    AgentStockInfo info = BuildAgentStockReader(reader);
                    list.Add(info);
                }
                reader.Close();
            }
            return list;
        }


        #endregion  BasicMethod

        #region  ExtensionMethod
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="orderby"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public DataTable GetAgentStockList(string strWhere, string orderby, int startIndex, int endIndex)
        {
            //List<CashCouponInfo> list = new List<CashCouponInfo>();
            StringBuilder strSql = new StringBuilder();
            strSql.Append("SELECT * FROM ( ");
            strSql.Append(" SELECT ROW_NUMBER() OVER (");
            if (!string.IsNullOrEmpty(orderby.Trim()))
            {
                strSql.Append("order by T." + orderby);
            }
            else
            {
                strSql.Append("order by T.asid desc");
            }
            strSql.Append(")AS Row, T.*,b.name,b.storeid,b.showimg  from hlh_agentstock T LEFT JOIN hlh_products b on T.pid=b.pid");
            if (!string.IsNullOrEmpty(strWhere.Trim()))
            {
                strSql.Append(" WHERE " + strWhere);
            }
            strSql.Append(" ) TT");
            strSql.AppendFormat(" WHERE TT.Row between {0} and {1}", startIndex, endIndex);

            return RDBSHelper.ExecuteDataset(CommandType.Text, strSql.ToString()).Tables[0];

        }
        /// <summary>
        /// 计算代理等级相应库存金额
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orderProduct"></param>
        /// <returns></returns>
        public decimal SumAgentLevel(PartUserInfo user, int pid, int count)
        {
            decimal amount = 0;
            List<string> agentPids_298 = WebHelper.GetConfigSettings("298_AgentPid").Split(',').ToList();
            PartProductInfo pro = Products.GetPartProductById(pid);
            if (user.AgentType == 0 && !agentPids_298.Exists(x => TypeHelper.StringToInt(x) == pid))
                amount = count * pro.ShopPrice;
            if (user.AgentType == 0 && agentPids_298.Exists(x => TypeHelper.StringToInt(x) == pid))
                if (count % 2 == 0)//双数，8折298
                    amount = count * 149;
                else//单数
                    amount = count * pro.ShopPrice;
            if (user.AgentType == 1)
                amount = (decimal)(count * ((double)pro.ShopPrice * 0.65));
            if (user.AgentType == 2)
                amount = (decimal)(count * ((double)pro.ShopPrice * 0.55));
            if (user.AgentType == 3)
                amount = (decimal)(count * ((double)pro.ShopPrice * 0.47));
            if (user.AgentType == 4)
            {
                if (user.Uid == WebHelper.GetConfigSettingsInt("SpecialAgentUid"))
                    amount = (decimal)(count * ((double)pro.ShopPrice * 0.4));
                else
                    amount = (decimal)(count * ((double)pro.ShopPrice * 0.42));
            }
            return amount;
        }
        /// <summary>
        /// 计算代理等级相应折扣单价金额
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orderProduct"></param>
        /// <returns></returns>
        public decimal SingleAgentPrice(PartUserInfo user, int pid)
        {
            decimal price = 0;
            PartProductInfo pro = AdminProducts.AdminGetProductById(pid);
            if (user.AgentType == 0)
                price = pro.ShopPrice;
            if (user.AgentType == 1)
                price = (decimal)(((double)pro.ShopPrice * 0.65));
            if (user.AgentType == 2)
                price = (decimal)(((double)pro.ShopPrice * 0.55));
            if (user.AgentType == 3)
                price = (decimal)(((double)pro.ShopPrice * 0.47));
            if (user.AgentType == 4)
            {
                if (user.Uid == WebHelper.GetConfigSettingsInt("SpecialAgentUid"))
                    price = (decimal)(((double)pro.ShopPrice * 0.4));
                else
                    price = (decimal)(((double)pro.ShopPrice * 0.42));
            }

            return price;
        }
        /// <summary>
        /// 计算代理等级相应折扣单价金额--大区70拿货有机胚芽
        /// </summary>
        /// <param name="user"></param>
        /// <param name="orderProduct"></param>
        /// <returns></returns>
        public decimal SingleAgentPriceFor70(PartUserInfo user, int pid)
        {
            decimal price = 0;
            PartProductInfo pro = Products.GetPartProductById(pid);
            if (user.AgentType == 0)
                price = pro.ShopPrice;
            if (user.AgentType == 1)
                price = (decimal)(((double)pro.ShopPrice * 0.65));
            if (user.AgentType == 2)
                price = (decimal)(((double)pro.ShopPrice * 0.55));
            if (user.AgentType == 3)
                price = (decimal)(((double)pro.ShopPrice * 0.47));
            if (user.AgentType == 4)
            {
                if (pid == WebHelper.GetConfigSettingsInt("SpecialAgentPid"))
                    price = 70;
                else
                    price = (decimal)(((double)pro.ShopPrice * 0.42));
            }

            return price;
        }
        /// <summary>
        /// Decimal类型截取保留N位小数并且不进行四舍五入操作
        /// </summary>
        /// <param name="d"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        public static decimal CutDecimalWithN(decimal d, int n)
        {
            string strDecimal = d.ToString();
            int index = strDecimal.IndexOf(".");
            if (index == -1 || strDecimal.Length < index + n + 1)
            {
                strDecimal = string.Format("{0:F" + n + "}", d);
            }
            else
            {
                int length = index;
                if (n != 0)
                {
                    length = index + n + 1;
                }
                strDecimal = strDecimal.Substring(0, length);
            }
            return Decimal.Parse(strDecimal);
        }
        /// <summary>
        /// 确认订单时的代理库存更新操作
        /// </summary>
        /// <param name="parentUser"></param>
        /// <param name="selfUser"></param>
        /// <param name="orderInfo"></param>
        /// <param name="orderProduct"></param>
        public void UpdateAgentStockForOrder(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct, bool isChangeSelfStock)
        {
            PartUserInfo currentuser = Users.GetPartUserById(selfUser.Uid);//取最新的当前用户信息
            int fromparentid1 = 0; //int 事业伙伴出货uid
            decimal fromparentamount1 = 0M;//'事业伙伴出货库存金额
            int fromparentid2 = 0; //int 星级出货uid
            decimal fromparentamount2 = 0M;//星级出货库存金额
            int fromparentid3 = 0; // int VIP出货id
            decimal fromparentamount3 = 0M;//VIP出货库存金额
            int fromparentid4 = 0;  //大区出货id 
            decimal fromparentamount4 = 0M;//大区出货库存金额
            decimal fromcompanyamount = 0M;//公司出货产品金额

            PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
            PartUserInfo Parent2 = new PartUserInfo();//星级
            PartUserInfo Parent3 = new PartUserInfo();//VIP
            PartUserInfo Parent4 = new PartUserInfo();//大区

            int stockFromParent1 = 0;
            int stockFromParent2 = 0;
            int stockFromParent3 = 0;
            int stockFromParent4 = 0;
            int stockFromCompany = 0;

            //上级库存处理,最大往上找3层，直到找到公司--数量为基础，
            //从大区开始往下判断等级
            if (currentuser.AgentType == 4)//大区往上无上级，直接到公司拿库存
            {
                fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, orderProduct.BuyCount);
                stockFromCompany = orderProduct.BuyCount;
                AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);

                Orders.UpdateOrderProductAgentCount(orderProduct.RecordId, 0, stockFromCompany, parentUser.Uid, fromparentid1, fromparentamount1, fromparentid2, fromparentamount2, fromparentid3, fromparentamount3, fromparentid4, fromparentamount4, fromcompanyamount);
            }
            if (currentuser.AgentType == 3) //VIP往上找大区，找不到大区或大区没货在公司拿库存
            {
                if (parentUser.AgentType == 4)
                    StockForAgent4(parentUser, selfUser, orderInfo, orderProduct);
            }
            if (currentuser.AgentType == 2) //星级往上找VIP，再找大区，找不到大区或大区没货在公司拿库存
            {
                if (parentUser.AgentType == 4)
                    StockForAgent4(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 3)
                    StockForAgent3(parentUser, selfUser, orderInfo, orderProduct);
            }
            if (currentuser.AgentType == 1) //事业伙伴往上找星级，再找VIP，再找大区，找不到大区或大区没货在公司拿库存
            {
                if (parentUser.AgentType == 4)
                    StockForAgent4(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 3)
                    StockForAgent3(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 2)
                    StockForAgent2(parentUser, selfUser, orderInfo, orderProduct);
            }
            if (currentuser.AgentType == 0)//个人零售 
            {
                if (parentUser.AgentType == 4)
                    StockForAgent4(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 3)
                    StockForAgent3(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 2)
                    StockForAgent2(parentUser, selfUser, orderInfo, orderProduct);
                if (parentUser.AgentType == 1)
                    StockForAgent1(parentUser, selfUser, orderInfo, orderProduct);
            }

            #region

            //找上级的对应产品的代理库存，找不到说明库存为0  公司拿货，库存小于购买，不足部分从公司补，并记录库存加减详情
            //AgentStockInfo parentStock = GetModel(string.Format(" uid={0} and pid={1} ", parentUser.Uid, orderProduct.Pid));

            ////如果上级库存不足或为库存0 再往上找上上级直到找到大区，大区没有再找到公司

            //if (parentStock == null || parentStock.Balance <= 0)//库存为0 或者库存不存在（直销会员默认VIP，无库存），从公司拿货
            //{
            //    stockFromCompany = orderProduct.BuyCount;
            //    AgentStockDetailBLL.Add(new AgentStockDetailInfo()
            //    {
            //        CreationDate = DateTime.Now,
            //        Uid = 0,
            //        Pid = orderProduct.Pid,
            //        DetailType = 2,
            //        OutAmount = stockFromCompany,
            //        OrderCode = orderInfo.OSN,
            //        DetailDesc = string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany),
            //        FromUser = 0,
            //        ToUser = selfUser.Uid
            //    });
            //}
            //else if (parentStock.Balance <= orderProduct.BuyCount)//库存小于购买，不足部分从公司补
            //{
            //    stockFromParent = parentStock.Balance;
            //    stockFromCompany = orderProduct.BuyCount - parentStock.Balance;
            //    parentStock.Balance = 0;
            //    Update(parentStock);
            //    AgentStockDetailBLL.Add(new AgentStockDetailInfo()
            //    {
            //        CreationDate = DateTime.Now,
            //        Uid = parentUser.Uid,
            //        Pid = orderProduct.Pid,
            //        DetailType = 2,
            //        OutAmount = stockFromParent,
            //        OrderCode = orderInfo.OSN,
            //        CurrentBalance = 0,
            //        DetailDesc = string.Format("下级会员{0}拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent),
            //        FromUser = 0,
            //        ToUser = selfUser.Uid
            //    });
            //    if (stockFromCompany > 0)
            //    {
            //        AgentStockDetailBLL.Add(new AgentStockDetailInfo()
            //        {
            //            CreationDate = DateTime.Now,
            //            Uid = 0,
            //            Pid = orderProduct.Pid,
            //            DetailType = 2,
            //            OutAmount = stockFromCompany,
            //            OrderCode = orderInfo.OSN,
            //            DetailDesc = string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany),
            //            FromUser = 0,
            //            ToUser = selfUser.Uid
            //        });
            //    }
            //}
            //else if (parentStock.Balance > orderProduct.BuyCount)
            //{
            //    stockFromParent = orderProduct.BuyCount;
            //    int stockRemain = parentStock.Balance - orderProduct.BuyCount;

            //    parentStock.Balance = stockRemain;
            //    Update(parentStock);
            //    AgentStockDetailBLL.Add(new AgentStockDetailInfo()
            //    {
            //        CreationDate = DateTime.Now,
            //        Uid = parentUser.Uid,
            //        Pid = orderProduct.Pid,
            //        DetailType = 2,
            //        OutAmount = stockFromParent,
            //        CurrentBalance = stockRemain,
            //        OrderCode = orderInfo.OSN,
            //        DetailDesc = string.Format("下级会员{0}拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent),
            //        FromUser = 0,
            //        ToUser = selfUser.Uid
            //    });
            //}
            #endregion

            //自身库存处理  --普通零售自身无库存--不需要增加库存


            //PartUserInfo current = Users.GetPartUserById(selfUser.Uid);
            if (currentuser.AgentType > 0 && isChangeSelfStock)
            {
                AgentStockInfo selfStock = GetModel(string.Format(" uid={0} and pid={1} ", selfUser.Uid, orderProduct.Pid));
                int selfCount = 0;
                decimal InAmount = SumAgentLevel(currentuser, orderProduct.Pid, orderProduct.BuyCount);
                decimal currentAmount = 0;
                if (selfStock == null)//记录不存在
                {
                    Add(new AgentStockInfo()
                    {
                        Uid = selfUser.Uid,
                        Pid = orderProduct.Pid,
                        Balance = orderProduct.BuyCount,
                        AgentAmount = InAmount
                    });
                    currentAmount = InAmount;
                }
                else
                {
                    selfCount = (int)Math.Floor(selfStock.Balance);
                    selfStock.Balance = selfStock.Balance + orderProduct.BuyCount;
                    selfStock.AgentAmount += InAmount;
                    this.Update(selfStock);
                    currentAmount = selfStock.AgentAmount;
                }

                //更新订单产品代理数量来源以及拿货人uid
                int stockFromParent = orderProduct.BuyCount - stockFromCompany;
                string selfDesc = "";
                if (stockFromParent > 0 && stockFromCompany <= 0)
                    selfDesc = string.Format("从上级拿货，产品：{0},数量：{1},金额：{2}", orderProduct.Name, orderProduct.BuyCount, orderProduct.BuyCount * orderProduct.DiscountPrice);
                if (stockFromParent <= 0 && stockFromCompany > 0)
                    selfDesc = string.Format("从公司拿货，产品：{0},数量：{1},金额：{2}", orderProduct.Name, orderProduct.BuyCount, orderProduct.BuyCount * orderProduct.DiscountPrice);
                if (stockFromParent > 0 && stockFromCompany > 0)
                    selfDesc = string.Format("混合拿货，产品：{0},从公司数量：{1}，从上级数量{2}", orderProduct.Name, stockFromCompany, stockFromParent);
                AgentStockDetailBLL.AddDetail(selfUser.Uid, orderProduct.Pid, 1, InAmount, 0, currentAmount, orderInfo.OSN, selfDesc, parentUser.Uid, 0);
            }
        }

        /// <summary>
        /// 取消订单时的代理库存更新操作
        /// </summary>
        /// <param name="parentUser"></param>
        /// <param name="selfUser"></param>
        /// <param name="orderInfo"></param>
        /// <param name="orderProduct"></param>
        public void UpdateAgentStockForCancel(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct)
        {
            //AgentStockInfo parentStock = GetModel(string.Format(" uid={0} and pid={1} ", parentUser.Uid, orderProduct.Pid));
            int stockFromCompany = orderProduct.FromCompany;
            int stockFromParent = orderProduct.BuyCount - stockFromCompany;
            if (stockFromCompany > 0)//退回公司，只记录详情，不增加公司库存
            {
                decimal InAmount = SumAgentLevel(selfUser, orderProduct.Pid, orderProduct.BuyCount);
                AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 3, InAmount, 0, 0, orderInfo.OSN, string.Format("会员{0}公司拿货后订单退回，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), selfUser.Uid, 0);
            }
            if (orderProduct.FromParentAmount4 > 0)//大区拿货退回
            {
                PartUserInfo Parent4 = Users.GetPartUserById(orderProduct.FromParentId4);
                AgentStockInfo parent4Stock = GetModel(string.Format(" uid={0} and pid={1} ", orderProduct.FromParentId4, orderProduct.Pid));
                if (parent4Stock != null)
                {
                    int stockCount = (int)(orderProduct.FromParentAmount4 / SingleAgentPrice(Parent4, orderProduct.Pid));
                    parent4Stock.Balance = parent4Stock.Balance + stockCount;
                    parent4Stock.AgentAmount = parent4Stock.AgentAmount + orderProduct.FromParentAmount4;
                    this.Update(parent4Stock);
                    AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 3, orderProduct.FromParentAmount4, 0, parent4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockCount, orderProduct.FromParentAmount4), selfUser.Uid, 0);
                }
                else
                {
                    Add(new AgentStockInfo()
                    {
                        Uid = orderProduct.FromParentId4,
                        Pid = orderProduct.Pid,
                        Balance = orderProduct.BuyCount,
                        AgentAmount = orderProduct.FromParentAmount4
                    });
                    AgentStockDetailBLL.AddDetail(orderProduct.FromParentId4, orderProduct.Pid, 3, orderProduct.FromParentAmount4, 0, orderProduct.FromParentAmount4, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, orderProduct.BuyCount, orderProduct.FromParentAmount4), selfUser.Uid, 0);
                }
            }
            if (orderProduct.FromParentAmount3 > 0)//VIP拿货退回
            {
                PartUserInfo Parent3 = Users.GetPartUserById(orderProduct.FromParentId3);
                AgentStockInfo parent3Stock = GetModel(string.Format(" uid={0} and pid={1} ", orderProduct.FromParentId3, orderProduct.Pid));
                if (parent3Stock != null)
                {
                    int stockCount = (int)(orderProduct.FromParentAmount3 / SingleAgentPrice(Parent3, orderProduct.Pid));
                    parent3Stock.Balance = parent3Stock.Balance + stockCount;
                    parent3Stock.AgentAmount = parent3Stock.AgentAmount + orderProduct.FromParentAmount3;
                    this.Update(parent3Stock);
                    AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 3, orderProduct.FromParentAmount3, 0, parent3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockCount, orderProduct.FromParentAmount3), selfUser.Uid, 0);
                }
                else
                {
                    Add(new AgentStockInfo()
                    {
                        Uid = orderProduct.FromParentId3,
                        Pid = orderProduct.Pid,
                        Balance = orderProduct.BuyCount,
                        AgentAmount = orderProduct.FromParentAmount3
                    });
                    AgentStockDetailBLL.AddDetail(orderProduct.FromParentId3, orderProduct.Pid, 3, orderProduct.FromParentAmount3, 0, orderProduct.FromParentAmount3, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, orderProduct.BuyCount, orderProduct.FromParentAmount3), selfUser.Uid, 0);
                }
            }
            if (orderProduct.FromParentAmount2 > 0)//星级拿货退回
            {
                PartUserInfo Parent2 = Users.GetPartUserById(orderProduct.FromParentId2);
                AgentStockInfo parent2Stock = GetModel(string.Format(" uid={0} and pid={1} ", orderProduct.FromParentId2, orderProduct.Pid));
                if (parent2Stock != null)
                {
                    int stockCount = (int)(orderProduct.FromParentAmount2 / SingleAgentPrice(Parent2, orderProduct.Pid));
                    parent2Stock.Balance = parent2Stock.Balance + stockCount;
                    parent2Stock.AgentAmount = parent2Stock.AgentAmount + orderProduct.FromParentAmount2;
                    this.Update(parent2Stock);
                    AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 3, orderProduct.FromParentAmount2, 0, parent2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockCount, orderProduct.FromParentAmount2), selfUser.Uid, 0);
                }
                else
                {
                    Add(new AgentStockInfo()
                    {
                        Uid = orderProduct.FromParentId2,
                        Pid = orderProduct.Pid,
                        Balance = orderProduct.BuyCount,
                        AgentAmount = orderProduct.FromParentAmount2
                    });
                    AgentStockDetailBLL.AddDetail(orderProduct.FromParentId2, orderProduct.Pid, 3, orderProduct.FromParentAmount2, 0, orderProduct.FromParentAmount2, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, orderProduct.BuyCount, orderProduct.FromParentAmount2), selfUser.Uid, 0);
                }
            }
            if (orderProduct.FromParentAmount1 > 0)//事业伙伴拿货退回
            {
                PartUserInfo Parent1 = Users.GetPartUserById(orderProduct.FromParentId1);
                AgentStockInfo parent1Stock = GetModel(string.Format(" uid={0} and pid={1} ", orderProduct.FromParentId1, orderProduct.Pid));
                if (parent1Stock != null)
                {
                    int stockCount = (int)(orderProduct.FromParentAmount1 / SingleAgentPrice(Parent1, orderProduct.Pid));
                    parent1Stock.Balance = parent1Stock.Balance + stockCount;
                    parent1Stock.AgentAmount = parent1Stock.AgentAmount + orderProduct.FromParentAmount1;
                    this.Update(parent1Stock);
                    AgentStockDetailBLL.AddDetail(Parent1.Uid, orderProduct.Pid, 3, orderProduct.FromParentAmount1, 0, parent1Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockCount, orderProduct.FromParentAmount1), selfUser.Uid, 0);
                }
                else
                {
                    Add(new AgentStockInfo()
                    {
                        Uid = orderProduct.FromParentId1,
                        Pid = orderProduct.Pid,
                        Balance = orderProduct.BuyCount,
                        AgentAmount = orderProduct.FromParentAmount1
                    });
                    AgentStockDetailBLL.AddDetail(orderProduct.FromParentId1, orderProduct.Pid, 3, orderProduct.FromParentAmount1, 0, orderProduct.FromParentAmount1, orderInfo.OSN, string.Format("下级会员{0}拿货退回，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, orderProduct.BuyCount, orderProduct.FromParentAmount1), selfUser.Uid, 0);
                }
            }
            //自身库存处理  --普通零售自身无库存--不需要减少库存
            if (selfUser.AgentType > 0)
            {
                AgentStockDetailInfo detailInfo = new AgentStockDetail().GetModel(string.Format(" uid={0} and pid={1} and ordercode='{2}' ", selfUser.Uid, orderProduct.Pid, orderInfo.OSN));
                if (detailInfo != null)
                {
                    AgentStockInfo selfStock = GetModel(string.Format(" uid={0} and pid={1} ", selfUser.Uid, orderProduct.Pid));
                    int selfCount = 0;
                    if (selfStock != null)
                    {
                        if (selfStock.Balance < orderProduct.BuyCount)
                            return;
                        selfCount = (int)Math.Floor(selfStock.Balance);
                        selfStock.Balance = selfStock.Balance - orderProduct.BuyCount;
                        selfStock.AgentAmount = selfStock.AgentAmount - SumAgentLevel(selfUser, orderProduct.Pid, orderProduct.BuyCount);
                        this.Update(selfStock);
                    }
                    string selfDesc = "";
                    if (stockFromParent > 0 && stockFromCompany <= 0)
                        selfDesc = string.Format("从上级{0}退货，产品：{1},数量：{2},金额：{3}", parentUser.UserName + "/" + parentUser.Mobile, orderProduct.Name, stockFromParent, orderProduct.BuyCount * orderProduct.DiscountPrice);
                    if (stockFromParent <= 0 && stockFromCompany > 0)
                        selfDesc = string.Format("从公司退货，产品：{0},数量：{1},金额：{3}", orderProduct.Name, stockFromCompany, orderProduct.BuyCount * orderProduct.DiscountPrice);
                    if (stockFromParent > 0 && stockFromCompany > 0)
                        selfDesc = string.Format("混合拿货退货，产品：{0},从公司数量：{1}，从上级{2},数量{3}", orderProduct.Name, stockFromCompany, parentUser.UserName + "/" + parentUser.Mobile, stockFromParent);
                    decimal OutAmount = SumAgentLevel(selfUser, orderProduct.Pid, orderProduct.BuyCount);
                    AgentStockDetailBLL.AddDetail(selfUser.Uid, orderProduct.Pid, 4, 0, OutAmount, selfStock.AgentAmount, orderInfo.OSN, selfDesc, parentUser.Uid, 0);
                }
            }
        }


        /// <summary>
        /// 大区库存操作
        /// </summary>
        public void StockForAgent4(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct)
        {
            PartUserInfo currentuser = Users.GetPartUserById(selfUser.Uid);//取最新的当前用户信息

            int fromparentid1 = 0; //int 事业伙伴出货uid
            decimal fromparentamount1 = 0M;//'事业伙伴出货库存金额
            int fromparentid2 = 0; //int 星级出货uid
            decimal fromparentamount2 = 0M;//星级出货库存金额
            int fromparentid3 = 0; // int VIP出货id
            decimal fromparentamount3 = 0M;//VIP出货库存金额
            int fromparentid4 = 0;  //大区出货id 
            decimal fromparentamount4 = 0M;//大区出货库存金额
            decimal fromcompanyamount = 0M;//公司出货产品金额

            PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
            PartUserInfo Parent2 = new PartUserInfo();//星级
            PartUserInfo Parent3 = new PartUserInfo();//VIP
            PartUserInfo Parent4 = new PartUserInfo();//大区

            int stockFromParent1 = 0;
            int stockFromParent2 = 0;
            int stockFromParent3 = 0;
            int stockFromParent4 = 0;
            int stockFromCompany = 0;

            int remain = orderProduct.BuyCount;
            Parent4 = Users.GetParentUserForAgentStock(currentuser);
            fromparentid4 = Parent4.Uid;
            AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
            if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
            {
                //大区该产品无货找其他库存换货
                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                if (Parent4OtherStock.Any())
                {
                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent4 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent4 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                fromparentamount4 += itemamount;
                                fromparentid4 = Parent4.Uid;

                                decimal changeAmount = singlePrice * stockFromParent4;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                item.Balance = stockRemain;
                                item.AgentAmount = item.AgentAmount - changeAmount;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                    stockFromCompany = remain;
                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                }
            }
            else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
            {
                stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                remain = remain - stockFromParent4;
                fromparentid4 = Parent4.Uid;
                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                this.Update(Paren4Stock);
                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                //大区该产品无货找其他库存换货
                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                if (Parent4OtherStock.Any())
                {
                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent4 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent4 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                fromparentamount4 += itemamount;
                                fromparentid4 = Parent4.Uid;

                                decimal changeAmount = singlePrice * stockFromParent4;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }

                stockFromCompany = remain;
                if (stockFromCompany > 0)
                {
                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                }
            }
            else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
            {
                stockFromParent4 = remain;
                decimal stockRemain = Paren4Stock.Balance - remain;
                fromparentid4 = Parent4.Uid;
                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                Paren4Stock.Balance = stockRemain;
                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                this.Update(Paren4Stock);
                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
            }

            Orders.UpdateOrderProductAgentCount(orderProduct.RecordId, 0, stockFromCompany, parentUser.Uid, fromparentid1, fromparentamount1, fromparentid2, fromparentamount2, fromparentid3, fromparentamount3, fromparentid4, fromparentamount4, fromcompanyamount);
        }

        /// <summary>
        /// VIP库存操作
        /// </summary>
        public void StockForAgent3(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct)
        {
            PartUserInfo currentuser = Users.GetPartUserById(selfUser.Uid);//取最新的当前用户信息

            int fromparentid1 = 0; //int 事业伙伴出货uid
            decimal fromparentamount1 = 0M;//'事业伙伴出货库存金额
            int fromparentid2 = 0; //int 星级出货uid
            decimal fromparentamount2 = 0M;//星级出货库存金额
            int fromparentid3 = 0; // int VIP出货id
            decimal fromparentamount3 = 0M;//VIP出货库存金额
            int fromparentid4 = 0;  //大区出货id 
            decimal fromparentamount4 = 0M;//大区出货库存金额
            decimal fromcompanyamount = 0M;//公司出货产品金额

            PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
            PartUserInfo Parent2 = new PartUserInfo();//星级
            PartUserInfo Parent3 = new PartUserInfo();//VIP
            PartUserInfo Parent4 = new PartUserInfo();//大区

            int stockFromParent1 = 0;
            int stockFromParent2 = 0;
            int stockFromParent3 = 0;
            int stockFromParent4 = 0;
            int stockFromCompany = 0;

            int remain = orderProduct.BuyCount;
            Parent3 = Users.GetParentUserForAgentStock(currentuser);
            fromparentid3 = Parent3.Uid;
            AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
            if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
            {
                //VIP该产品无货找其他库存换货
                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                if (Parent3OtherStock.Any())
                {
                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent3 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent3 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                fromparentamount3 += itemamount;
                                fromparentid3 = Parent3.Uid;

                                decimal changeAmount = singlePrice * stockFromParent3;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                    fromparentid4 = Parent4.Uid;
                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                    {
                        //大区该产品无货找其他库存换货
                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                        if (Parent4OtherStock.Any())
                        {
                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                        if (change >= remain)
                                        {
                                            stockFromParent4 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent4 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        fromparentamount4 += itemamount;
                                        fromparentid4 = Parent4.Uid;

                                        decimal changeAmount = singlePrice * stockFromParent4;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                            stockFromCompany = remain;
                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                        }
                    }
                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                    {
                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                        remain = remain - stockFromParent4;
                        fromparentid4 = Parent4.Uid;
                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                        this.Update(Paren4Stock);
                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                        //大区该产品无货找其他库存换货
                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                        if (Parent4OtherStock.Any())
                        {
                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                        if (change >= remain)
                                        {
                                            stockFromParent4 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent4 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        fromparentamount4 += itemamount;
                                        fromparentid4 = Parent4.Uid;

                                        decimal changeAmount = singlePrice * stockFromParent4;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        stockFromCompany = remain;
                        if (stockFromCompany > 0)
                        {
                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                        }
                    }
                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                    {
                        stockFromParent4 = remain;
                        decimal stockRemain = Paren4Stock.Balance - remain;
                        fromparentid4 = Parent4.Uid;
                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                        Paren4Stock.Balance = stockRemain;
                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                        this.Update(Paren4Stock);
                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
            {
                stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                remain = remain - stockFromParent3;
                fromparentid3 = Parent3.Uid;
                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                this.Update(Paren3Stock);
                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                //VIP该产品无货找其他库存换货
                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                if (Parent3OtherStock.Any())
                {
                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent3 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent3 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                fromparentamount3 += itemamount;
                                fromparentid3 = Parent3.Uid;

                                decimal changeAmount = singlePrice * stockFromParent3;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                    fromparentid4 = Parent4.Uid;
                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                    {
                        //大区该产品无货找其他库存换货
                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                        if (Parent4OtherStock.Any())
                        {
                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                        if (change >= remain)
                                        {
                                            stockFromParent4 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent4 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        fromparentamount4 += itemamount;
                                        fromparentid4 = Parent4.Uid;

                                        decimal changeAmount = singlePrice * stockFromParent4;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                            stockFromCompany = remain;
                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                        }
                    }
                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                    {
                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                        remain = remain - stockFromParent4;
                        fromparentid4 = Parent4.Uid;
                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                        this.Update(Paren4Stock);
                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                        //大区该产品无货找其他库存换货
                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                        if (Parent4OtherStock.Any())
                        {
                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                        if (change >= remain)
                                        {
                                            stockFromParent4 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent4 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        fromparentamount4 += itemamount;
                                        fromparentid4 = Parent4.Uid;

                                        decimal changeAmount = singlePrice * stockFromParent4;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }

                        stockFromCompany = remain;
                        if (stockFromCompany > 0)
                        {
                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                        }
                    }
                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                    {
                        stockFromParent4 = remain;
                        decimal stockRemain = Paren4Stock.Balance - remain;
                        fromparentid4 = Parent4.Uid;
                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                        Paren4Stock.Balance = stockRemain;
                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                        this.Update(Paren4Stock);
                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
            {
                stockFromParent3 = remain;
                decimal stockRemain = Paren3Stock.Balance - remain;
                fromparentid3 = Parent3.Uid;
                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                Paren3Stock.Balance = stockRemain;
                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                this.Update(Paren3Stock);
                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
            }

            Orders.UpdateOrderProductAgentCount(orderProduct.RecordId, 0, stockFromCompany, parentUser.Uid, fromparentid1, fromparentamount1, fromparentid2, fromparentamount2, fromparentid3, fromparentamount3, fromparentid4, fromparentamount4, fromcompanyamount);
        }

        /// <summary>
        /// 星级库存操作
        /// </summary>
        public void StockForAgent2(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct)
        {
            PartUserInfo currentuser = Users.GetPartUserById(selfUser.Uid);//取最新的当前用户信息

            int fromparentid1 = 0; //int 事业伙伴出货uid
            decimal fromparentamount1 = 0M;//'事业伙伴出货库存金额
            int fromparentid2 = 0; //int 星级出货uid
            decimal fromparentamount2 = 0M;//星级出货库存金额
            int fromparentid3 = 0; // int VIP出货id
            decimal fromparentamount3 = 0M;//VIP出货库存金额
            int fromparentid4 = 0;  //大区出货id 
            decimal fromparentamount4 = 0M;//大区出货库存金额
            decimal fromcompanyamount = 0M;//公司出货产品金额

            PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
            PartUserInfo Parent2 = new PartUserInfo();//星级
            PartUserInfo Parent3 = new PartUserInfo();//VIP
            PartUserInfo Parent4 = new PartUserInfo();//大区

            int stockFromParent1 = 0;
            int stockFromParent2 = 0;
            int stockFromParent3 = 0;
            int stockFromParent4 = 0;
            int stockFromCompany = 0;

            int remain = orderProduct.BuyCount;
            Parent2 = Users.GetParentUserForAgentStock(currentuser);
            fromparentid2 = Parent2.Uid;
            AgentStockInfo Paren2Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent2.Uid, orderProduct.Pid));
            if (Paren2Stock == null || Math.Floor(Paren2Stock.Balance) <= 0)
            {
                //星级该产品无货找其他库存换货
                List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                if (Parent2OtherStock.Any())
                {
                    foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent2 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent2 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                fromparentamount2 += itemamount;
                                fromparentid2 = Parent2.Uid;
                                decimal changeAmount = singlePrice * stockFromParent2;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent3 = Users.GetParentUserForAgentStock(Parent2);
                    fromparentid3 = Parent3.Uid;
                    AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                    if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                    {
                        //VIP该产品无货找其他库存换货
                        List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                        if (Parent3OtherStock.Any())
                        {
                            foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent3 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent3 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                        fromparentamount3 += itemamount;
                                        fromparentid3 = Parent3.Uid;

                                        decimal changeAmount = singlePrice * stockFromParent3;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent4 = Users.GetParentUserForAgentStock(Parent3);
                            fromparentid4 = Parent4.Uid;
                            AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                            if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                            {
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;


                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                    stockFromCompany = remain;
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                            {
                                stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                remain = remain - stockFromParent4;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;

                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                stockFromCompany = remain;
                                if (stockFromCompany > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent4 = remain;
                                decimal stockRemain = Paren4Stock.Balance - remain;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = stockRemain;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                    {
                        stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                        remain = remain - stockFromParent3;
                        fromparentid3 = Parent3.Uid;
                        fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                        Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                        Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                        this.Update(Paren3Stock);
                        AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                        //VIP该产品无货找其他库存换货
                        List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                        if (Parent3OtherStock.Any())
                        {
                            foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent3 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent3 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                        fromparentamount3 += itemamount;
                                        fromparentid3 = Parent3.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent3;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent4 = Users.GetParentUserForAgentStock(Parent3);
                            fromparentid4 = Parent4.Uid;
                            AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                            if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                            {
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                    stockFromCompany = remain;
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                            {
                                stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                remain = remain - stockFromParent4;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }

                                stockFromCompany = remain;
                                if (stockFromCompany > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent4 = remain;
                                decimal stockRemain = Paren4Stock.Balance - remain;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = stockRemain;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                    {
                        stockFromParent3 = remain;
                        decimal stockRemain = Paren3Stock.Balance - remain;
                        fromparentid3 = Parent3.Uid;
                        fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                        Paren3Stock.Balance = stockRemain;
                        Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                        this.Update(Paren3Stock);
                        AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren2Stock.Balance) < remain)
            {
                stockFromParent2 = (int)Math.Floor(Paren2Stock.Balance);
                remain = remain - stockFromParent2;
                fromparentid2 = Parent2.Uid;
                fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                Paren2Stock.Balance = Paren2Stock.Balance - stockFromParent2;
                Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                this.Update(Paren2Stock);
                AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
                //星级该产品无货找其他库存换货
                List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                if (Parent2OtherStock.Any())
                {
                    foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent2 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent2 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                fromparentamount2 += itemamount;
                                fromparentid2 = Parent2.Uid;
                                decimal changeAmount = singlePrice * stockFromParent2;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent3 = Users.GetParentUserForAgentStock(Parent2);
                    fromparentid3 = Parent3.Uid;
                    AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                    if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                    {
                        //VIP该产品无货找其他库存换货
                        List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                        if (Parent3OtherStock.Any())
                        {
                            foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent3 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent3 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                        fromparentamount3 += itemamount;
                                        fromparentid3 = Parent3.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent3;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent4 = Users.GetParentUserForAgentStock(Parent3);
                            fromparentid4 = Parent4.Uid;
                            AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                            if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                            {
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                    stockFromCompany = remain;
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                            {
                                stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                remain = remain - stockFromParent4;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                stockFromCompany = remain;
                                if (stockFromCompany > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent4 = remain;
                                decimal stockRemain = Paren4Stock.Balance - remain;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = stockRemain;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                    {
                        stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                        remain = remain - stockFromParent3;
                        fromparentid3 = Parent3.Uid;
                        fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                        Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                        Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                        this.Update(Paren3Stock);
                        AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                        //VIP该产品无货找其他库存换货
                        List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                        if (Parent3OtherStock.Any())
                        {
                            foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent3 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent3 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                        fromparentamount3 += itemamount;
                                        fromparentid3 = Parent3.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent3;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent4 = Users.GetParentUserForAgentStock(Parent3);
                            fromparentid4 = Parent4.Uid;
                            AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                            if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                            {
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                    stockFromCompany = remain;
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                            {
                                stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                remain = remain - stockFromParent4;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                //大区该产品无货找其他库存换货
                                List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                if (Parent4OtherStock.Any())
                                {
                                    foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent4 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent4 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                fromparentamount4 += itemamount;
                                                fromparentid4 = Parent4.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent4;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }

                                stockFromCompany = remain;
                                if (stockFromCompany > 0)
                                {
                                    fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                    AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                }
                            }
                            else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent4 = remain;
                                decimal stockRemain = Paren4Stock.Balance - remain;
                                fromparentid4 = Parent4.Uid;
                                fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                Paren4Stock.Balance = stockRemain;
                                Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                this.Update(Paren4Stock);
                                AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                    {
                        stockFromParent3 = remain;
                        decimal stockRemain = Paren3Stock.Balance - remain;
                        fromparentid3 = Parent3.Uid;
                        fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                        Paren3Stock.Balance = stockRemain;
                        Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                        this.Update(Paren3Stock);
                        AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren2Stock.Balance) >= remain)
            {
                stockFromParent2 = remain;
                decimal stockRemain = Paren2Stock.Balance - stockFromParent2;
                fromparentid2 = Parent2.Uid;
                fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                Paren2Stock.Balance = stockRemain;
                Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                this.Update(Paren2Stock);
                AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
            }

            Orders.UpdateOrderProductAgentCount(orderProduct.RecordId, 0, stockFromCompany, parentUser.Uid, fromparentid1, fromparentamount1, fromparentid2, fromparentamount2, fromparentid3, fromparentamount3, fromparentid4, fromparentamount4, fromcompanyamount);
        }

        /// <summary>
        /// 事业伙伴库存操作
        /// </summary>
        public void StockForAgent1(PartUserInfo parentUser, PartUserInfo selfUser, OrderInfo orderInfo, OrderProductInfo orderProduct)
        {
            PartUserInfo currentuser = Users.GetPartUserById(selfUser.Uid);//取最新的当前用户信息
            int fromparentid1 = 0; //int 事业伙伴出货uid
            decimal fromparentamount1 = 0M;//'事业伙伴出货库存金额
            int fromparentid2 = 0; //int 星级出货uid
            decimal fromparentamount2 = 0M;//星级出货库存金额
            int fromparentid3 = 0; // int VIP出货id
            decimal fromparentamount3 = 0M;//VIP出货库存金额
            int fromparentid4 = 0;  //大区出货id 
            decimal fromparentamount4 = 0M;//大区出货库存金额
            decimal fromcompanyamount = 0M;//公司出货产品金额

            PartUserInfo Parent1 = new PartUserInfo();//事业伙伴
            PartUserInfo Parent2 = new PartUserInfo();//星级
            PartUserInfo Parent3 = new PartUserInfo();//VIP
            PartUserInfo Parent4 = new PartUserInfo();//大区

            int stockFromParent1 = 0;
            int stockFromParent2 = 0;
            int stockFromParent3 = 0;
            int stockFromParent4 = 0;
            int stockFromCompany = 0;
            int remain = orderProduct.BuyCount;

            Parent1 = Users.GetParentUserForAgentStock(currentuser);
            fromparentid1 = Parent1.Uid;
            AgentStockInfo Paren1Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent1.Uid, orderProduct.Pid));
            if (Paren1Stock == null || Math.Floor(Paren1Stock.Balance) <= 0)//事业伙伴无货,往上找星级-VIP-大区
            {
                //星级该产品无货找其他库存换货
                List<AgentStockInfo> Parent1OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent1.Uid, orderProduct.Pid));
                if (Parent1OtherStock.Any())
                {
                    foreach (var item in Parent1OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent1, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent1, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent1 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent1 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent1, orderProduct.Pid, stockFromParent1);
                                fromparentamount1 += itemamount;
                                fromparentid1 = Parent1.Uid;
                                decimal changeAmount = singlePrice * stockFromParent1;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent1.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent1, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent2 = Users.GetParentUserForAgentStock(currentuser);
                    fromparentid2 = Parent2.Uid;
                    AgentStockInfo Paren2Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent2.Uid, orderProduct.Pid));
                    if (Paren2Stock == null || Math.Floor(Paren2Stock.Balance) <= 0)
                    {
                        //星级该产品无货找其他库存换货
                        List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                        if (Parent2OtherStock.Any())
                        {
                            foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent2 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent2 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                        fromparentamount2 += itemamount;
                                        fromparentid2 = Parent2.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent2;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent3 = Users.GetParentUserForAgentStock(Parent2);
                            fromparentid3 = Parent3.Uid;
                            AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                            if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                            {
                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;

                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;


                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;

                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                            {
                                stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                                remain = remain - stockFromParent3;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }

                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent3 = remain;
                                decimal stockRemain = Paren3Stock.Balance - remain;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = stockRemain;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren2Stock.Balance) < remain)
                    {
                        stockFromParent2 = (int)Math.Floor(Paren2Stock.Balance);
                        remain = remain - stockFromParent2;
                        fromparentid2 = Parent2.Uid;
                        fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                        Paren2Stock.Balance = Paren2Stock.Balance - stockFromParent2;
                        Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                        this.Update(Paren2Stock);
                        AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
                        //星级该产品无货找其他库存换货
                        List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                        if (Parent2OtherStock.Any())
                        {
                            foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent2 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent2 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                        fromparentamount2 += itemamount;
                                        fromparentid2 = Parent2.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent2;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent3 = Users.GetParentUserForAgentStock(Parent2);
                            fromparentid3 = Parent3.Uid;
                            AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                            if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                            {
                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                            {
                                stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                                remain = remain - stockFromParent3;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }

                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent3 = remain;
                                decimal stockRemain = Paren3Stock.Balance - remain;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = stockRemain;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren2Stock.Balance) >= remain)
                    {
                        stockFromParent2 = remain;
                        decimal stockRemain = Paren2Stock.Balance - stockFromParent2;
                        fromparentid2 = Parent2.Uid;
                        fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                        Paren2Stock.Balance = stockRemain;
                        Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                        this.Update(Paren2Stock);
                        AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren1Stock.Balance) < remain) //事业伙伴有货，但库存小于购买,不足部分从星级-VIP-大区补 ，再从公司补
            {
                stockFromParent1 = (int)Math.Floor(Paren1Stock.Balance);
                remain = remain - stockFromParent1;
                fromparentid1 = Parent1.Uid;
                fromparentamount1 = SumAgentLevel(Parent1, orderProduct.Pid, stockFromParent1);
                Paren1Stock.Balance = Paren1Stock.Balance - stockFromParent1;
                Paren1Stock.AgentAmount = Paren1Stock.AgentAmount - fromparentamount1;
                this.Update(Paren1Stock);
                AgentStockDetailBLL.AddDetail(Parent1.Uid, orderProduct.Pid, 2, 0, fromparentamount1, Paren1Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent1, fromparentamount1), 0, selfUser.Uid);
                //星级该产品无货找其他库存换货
                List<AgentStockInfo> Parent1OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent1.Uid, orderProduct.Pid));
                if (Parent1OtherStock.Any())
                {
                    foreach (var item in Parent1OtherStock.OrderByDescending(x => x.AgentAmount))
                    {
                        if (item.AgentAmount > 0)
                        {
                            if (remain > 0)
                            {
                                decimal singlePrice = SingleAgentPrice(Parent1, orderProduct.Pid);
                                decimal itemPrice = SingleAgentPrice(Parent1, item.Pid);
                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                if (change >= remain)
                                {
                                    stockFromParent1 = remain;
                                    remain = 0;
                                }
                                else
                                {
                                    stockFromParent1 = change;
                                    remain = remain - change;
                                }
                                decimal itemamount = SumAgentLevel(Parent1, orderProduct.Pid, stockFromParent1);
                                fromparentamount1 += itemamount;
                                fromparentid1 = Parent1.Uid;
                                decimal changeAmount = singlePrice * stockFromParent1;
                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                item.AgentAmount = item.AgentAmount - changeAmount;
                                item.Balance = stockRemain;
                                this.Update(item);
                                AgentStockDetailBLL.AddDetail(Parent1.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent1, itemamount), 0, selfUser.Uid);
                            }
                        }
                    }
                }
                if (remain > 0)
                {
                    Parent2 = Users.GetParentUserForAgentStock(currentuser);
                    fromparentid2 = Parent2.Uid;
                    AgentStockInfo Paren2Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent2.Uid, orderProduct.Pid));
                    if (Paren2Stock == null || Math.Floor(Paren2Stock.Balance) <= 0)
                    {
                        //星级该产品无货找其他库存换货
                        List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                        if (Parent2OtherStock.Any())
                        {
                            foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent2 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent2 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                        fromparentamount2 += itemamount;
                                        fromparentid2 = Parent2.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent2;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent3 = Users.GetParentUserForAgentStock(Parent2);
                            fromparentid3 = Parent3.Uid;
                            AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                            if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                            {
                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;

                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;


                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;

                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                            {
                                stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                                remain = remain - stockFromParent3;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);

                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);

                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }

                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent3 = remain;
                                decimal stockRemain = Paren3Stock.Balance - remain;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = stockRemain;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren2Stock.Balance) < remain)
                    {
                        stockFromParent2 = (int)Math.Floor(Paren2Stock.Balance);
                        remain = remain - stockFromParent2;
                        fromparentid2 = Parent2.Uid;
                        fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                        Paren2Stock.Balance = Paren2Stock.Balance - stockFromParent2;
                        Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                        this.Update(Paren2Stock);
                        AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
                        //星级该产品无货找其他库存换货
                        List<AgentStockInfo> Parent2OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent2.Uid, orderProduct.Pid));
                        if (Parent2OtherStock.Any())
                        {
                            foreach (var item in Parent2OtherStock.OrderByDescending(x => x.AgentAmount))
                            {
                                if (item.AgentAmount > 0)
                                {
                                    if (remain > 0)
                                    {
                                        decimal singlePrice = SingleAgentPrice(Parent2, orderProduct.Pid);
                                        decimal itemPrice = SingleAgentPrice(Parent2, item.Pid);
                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                        if (change >= remain)
                                        {
                                            stockFromParent2 = remain;
                                            remain = 0;
                                        }
                                        else
                                        {
                                            stockFromParent2 = change;
                                            remain = remain - change;
                                        }
                                        decimal itemamount = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                                        fromparentamount2 += itemamount;
                                        fromparentid2 = Parent2.Uid;
                                        decimal changeAmount = singlePrice * stockFromParent2;
                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                        item.Balance = stockRemain;
                                        this.Update(item);
                                        AgentStockDetailBLL.AddDetail(Parent2.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, itemamount), 0, selfUser.Uid);
                                    }
                                }
                            }
                        }
                        if (remain > 0)
                        {
                            Parent3 = Users.GetParentUserForAgentStock(Parent2);
                            fromparentid3 = Parent3.Uid;
                            AgentStockInfo Paren3Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent3.Uid, orderProduct.Pid));
                            if (Paren3Stock == null || Math.Floor(Paren3Stock.Balance) <= 0)//VIP无货,往上找大区
                            {
                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) < remain) //VIP有货，但库存小于购买,不足部分从大区补 ，再从公司补
                            {
                                stockFromParent3 = (int)Math.Floor(Paren3Stock.Balance);
                                remain = remain - stockFromParent3;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = Paren3Stock.Balance - stockFromParent3;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);

                                //VIP该产品无货找其他库存换货
                                List<AgentStockInfo> Parent3OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent3.Uid, orderProduct.Pid));
                                if (Parent3OtherStock.Any())
                                {
                                    foreach (var item in Parent3OtherStock.OrderByDescending(x => x.AgentAmount))
                                    {
                                        if (item.AgentAmount > 0)
                                        {
                                            if (remain > 0)
                                            {
                                                decimal singlePrice = SingleAgentPrice(Parent3, orderProduct.Pid);
                                                decimal itemPrice = SingleAgentPrice(Parent3, item.Pid);
                                                int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                if (change >= remain)
                                                {
                                                    stockFromParent3 = remain;
                                                    remain = 0;
                                                }
                                                else
                                                {
                                                    stockFromParent3 = change;
                                                    remain = remain - change;
                                                }
                                                decimal itemamount = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                                fromparentamount3 += itemamount;
                                                fromparentid3 = Parent3.Uid;
                                                decimal changeAmount = singlePrice * stockFromParent3;
                                                decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                item.AgentAmount = item.AgentAmount - changeAmount;
                                                item.Balance = stockRemain;
                                                this.Update(item);
                                                AgentStockDetailBLL.AddDetail(Parent3.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, itemamount), 0, selfUser.Uid);
                                            }
                                        }
                                    }
                                }
                                if (remain > 0)
                                {
                                    Parent4 = Users.GetParentUserForAgentStock(Parent3);
                                    fromparentid4 = Parent4.Uid;
                                    AgentStockInfo Paren4Stock = GetModel(string.Format(" uid={0} and pid={1} ", Parent4.Uid, orderProduct.Pid));
                                    if (Paren4Stock == null || Math.Floor(Paren4Stock.Balance) <= 0)//大区无货找公司
                                    {
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }
                                        if (remain > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, remain);
                                            stockFromCompany = remain;
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany, fromcompanyamount), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) < remain)//大区有货，但库存小于购买,不足部分从公司补
                                    {
                                        stockFromParent4 = (int)Math.Floor(Paren4Stock.Balance);
                                        remain = remain - stockFromParent4;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = Paren4Stock.Balance - stockFromParent4;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                        //大区该产品无货找其他库存换货
                                        List<AgentStockInfo> Parent4OtherStock = GetList(string.Format(" uid={0} and pid <>{1} ", Parent4.Uid, orderProduct.Pid));
                                        if (Parent4OtherStock.Any())
                                        {
                                            foreach (var item in Parent4OtherStock.OrderByDescending(x => x.AgentAmount))
                                            {
                                                if (item.AgentAmount > 0)
                                                {
                                                    if (remain > 0)
                                                    {
                                                        decimal singlePrice = SingleAgentPrice(Parent4, orderProduct.Pid);
                                                        decimal itemPrice = SingleAgentPrice(Parent4, item.Pid);
                                                        int change = (int)Math.Floor(item.AgentAmount / singlePrice);
                                                        if (change >= remain)
                                                        {
                                                            stockFromParent4 = remain;
                                                            remain = 0;
                                                        }
                                                        else
                                                        {
                                                            stockFromParent4 = change;
                                                            remain = remain - change;
                                                        }
                                                        decimal itemamount = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                                        fromparentamount4 += itemamount;
                                                        fromparentid4 = Parent4.Uid;
                                                        decimal changeAmount = singlePrice * stockFromParent4;
                                                        decimal stockRemain = CutDecimalWithN((item.AgentAmount - changeAmount) / itemPrice, 4);
                                                        item.AgentAmount = item.AgentAmount - changeAmount;
                                                        item.Balance = stockRemain;
                                                        this.Update(item);
                                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, item.Pid, 2, 0, itemamount, item.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, itemamount), 0, selfUser.Uid);
                                                    }
                                                }
                                            }
                                        }

                                        stockFromCompany = remain;
                                        if (stockFromCompany > 0)
                                        {
                                            fromcompanyamount = SumAgentLevel(currentuser, orderProduct.Pid, stockFromCompany);
                                            AgentStockDetailBLL.AddDetail(0, orderProduct.Pid, 2, 0, fromcompanyamount, 0, orderInfo.OSN, string.Format("会员{0}从公司拿货，产品：{1},数量：{2}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromCompany), 0, selfUser.Uid);
                                        }
                                    }
                                    else if (Math.Floor(Paren4Stock.Balance) >= remain) //库存大于购买
                                    {
                                        stockFromParent4 = remain;
                                        decimal stockRemain = Paren4Stock.Balance - remain;
                                        fromparentid4 = Parent4.Uid;
                                        fromparentamount4 = SumAgentLevel(Parent4, orderProduct.Pid, stockFromParent4);
                                        Paren4Stock.Balance = stockRemain;
                                        Paren4Stock.AgentAmount = Paren4Stock.AgentAmount - fromparentamount4;
                                        this.Update(Paren4Stock);
                                        AgentStockDetailBLL.AddDetail(Parent4.Uid, orderProduct.Pid, 2, 0, fromparentamount4, Paren4Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent4, fromparentamount4), 0, selfUser.Uid);
                                    }
                                }
                            }
                            else if (Math.Floor(Paren3Stock.Balance) >= remain) //库存大于购买
                            {
                                stockFromParent3 = remain;
                                decimal stockRemain = Paren3Stock.Balance - remain;
                                fromparentid3 = Parent3.Uid;
                                fromparentamount3 = SumAgentLevel(Parent3, orderProduct.Pid, stockFromParent3);
                                Paren3Stock.Balance = stockRemain;
                                Paren3Stock.AgentAmount = Paren3Stock.AgentAmount - fromparentamount3;
                                this.Update(Paren3Stock);
                                AgentStockDetailBLL.AddDetail(Parent3.Uid, orderProduct.Pid, 2, 0, fromparentamount3, Paren3Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent3, fromparentamount3), 0, selfUser.Uid);
                            }
                        }
                    }
                    else if (Math.Floor(Paren2Stock.Balance) >= remain)
                    {
                        stockFromParent2 = remain;
                        decimal stockRemain = Paren2Stock.Balance - stockFromParent2;
                        fromparentid2 = Parent2.Uid;
                        fromparentamount2 = SumAgentLevel(Parent2, orderProduct.Pid, stockFromParent2);
                        Paren2Stock.Balance = stockRemain;
                        Paren2Stock.AgentAmount = Paren2Stock.AgentAmount - fromparentamount2;
                        this.Update(Paren2Stock);
                        AgentStockDetailBLL.AddDetail(Parent2.Uid, orderProduct.Pid, 2, 0, fromparentamount2, Paren2Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent2, fromparentamount2), 0, selfUser.Uid);
                    }
                }
            }
            else if (Math.Floor(Paren1Stock.Balance) >= remain) //库存大于购买
            {
                stockFromParent1 = remain;
                decimal stockRemain = Paren1Stock.Balance - stockFromParent1;
                fromparentid1 = Parent1.Uid;
                fromparentamount1 = SumAgentLevel(Parent1, orderProduct.Pid, stockFromParent1);
                Paren1Stock.Balance = stockRemain;
                Paren1Stock.AgentAmount = Paren1Stock.AgentAmount - fromparentamount1;
                this.Update(Paren1Stock);
                AgentStockDetailBLL.AddDetail(Parent1.Uid, orderProduct.Pid, 2, 0, fromparentamount1, Paren1Stock.AgentAmount, orderInfo.OSN, string.Format("下级会员{0}拿货，产品：{1},数量：{2},金额：{3}", selfUser.UserName + "/" + selfUser.Mobile, orderProduct.Name, stockFromParent1, fromparentamount1), 0, selfUser.Uid);
            }

            Orders.UpdateOrderProductAgentCount(orderProduct.RecordId, 0, stockFromCompany, parentUser.Uid, fromparentid1, fromparentamount1, fromparentid2, fromparentamount2, fromparentid3, fromparentamount3, fromparentid4, fromparentamount4, fromcompanyamount);
        }


        #endregion  ExtensionMethod

    }
}

