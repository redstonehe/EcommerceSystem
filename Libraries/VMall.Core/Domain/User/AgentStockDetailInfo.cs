using System;
namespace VMall.Core
{
	/// <summary>
	/// 代理库存详情
	/// </summary>
	[Serializable]
	public partial class AgentStockDetailInfo
	{
        public AgentStockDetailInfo()
		{}
		#region Model
		private int _id;
		private DateTime _creationdate= DateTime.Now;
		private int _uid=0;
		private int _pid=0;
		private int _detailtype=0;
		private decimal _inamount=0;
        private decimal _outamount = 0;
		private decimal _currentbalance=0;
		private string _ordercode="‘’";
		private string _detaildesc="‘’";
		private int _fromuser=0;
		private int _touser=0;
		/// <summary>
		/// 记录id
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreationDate
		{
			set{ _creationdate=value;}
			get{return _creationdate;}
		}
		/// <summary>
		/// 会员id
		/// </summary>
		public int Uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 产品id
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
		}
		/// <summary>
		/// 记录类型，1增加，2减少
		/// </summary>
		public int DetailType
		{
			set{ _detailtype=value;}
			get{return _detailtype;}
		}
		/// <summary>
		/// 库存收入
		/// </summary>
        public decimal InAmount
		{
			set{ _inamount=value;}
			get{return _inamount;}
		}
		/// <summary>
		/// 库存支出
		/// </summary>
        public decimal OutAmount
		{
			set{ _outamount=value;}
			get{return _outamount;}
		}
		/// <summary>
		/// 当前库存余额
		/// </summary>
		public decimal CurrentBalance
		{
			set{ _currentbalance=value;}
			get{return _currentbalance;}
		}
		/// <summary>
		/// 关联订单号
		/// </summary>
		public string OrderCode
		{
			set{ _ordercode=value;}
			get{return _ordercode;}
		}
		/// <summary>
		/// 记录详情
		/// </summary>
		public string DetailDesc
		{
			set{ _detaildesc=value;}
			get{return _detaildesc;}
		}
		/// <summary>
		/// 来源会员id，0表示来源公司
		/// </summary>
		public int FromUser
		{
			set{ _fromuser=value;}
			get{return _fromuser;}
		}
		/// <summary>
		/// 去向库存会员
		/// </summary>
		public int ToUser
		{
			set{ _touser=value;}
			get{return _touser;}
		}
		#endregion Model

	}
}

