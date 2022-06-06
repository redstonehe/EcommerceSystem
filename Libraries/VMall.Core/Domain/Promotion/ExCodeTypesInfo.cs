using System;
namespace VMall.Core
{
	/// <summary>
	/// 兑换码类型表
	/// </summary>
	[Serializable]
	public partial class ExCodeTypesInfo
	{
		public ExCodeTypesInfo()
		{}
		#region Model
		private int _codetypeid;
		private int _storeid=0;
		private int _state=0;
		private string _name="";
		private int _money=0;
		private int _count=0;
		private int _sendmode=0;
		private int _getmode=0;
		private int _usemode=0;
		private int _userranklower=0;
		private int _orderamountlower=0;
		private int _limitstorecid=0;
		private int _limitproduct=0;
        private DateTime _sendstarttime = new DateTime(1900, 1, 1);
        private DateTime _sendendtime = new DateTime(1900, 1, 1);
		private int _useexpiretime=0;
        private DateTime _usestarttime = new DateTime(1900, 1, 1);
        private DateTime _useendtime = new DateTime(1900, 1, 1);
		/// <summary>
		/// 兑换码类型id
		/// </summary>
		public int CodeTypeId
		{
			set{ _codetypeid=value;}
			get{return _codetypeid;}
		}
		/// <summary>
		/// 店铺id
		/// </summary>
		public int StoreId
		{
			set{ _storeid=value;}
			get{return _storeid;}
		}
		/// <summary>
		/// 状态
		/// </summary>
		public int State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// 名称
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// 金额
		/// </summary>
		public int Money
		{
			set{ _money=value;}
			get{return _money;}
		}
		/// <summary>
		/// 数量
		/// </summary>
		public int Count
		{
			set{ _count=value;}
			get{return _count;}
		}
		/// <summary>
		/// 发放方式(0代表免费发放,1代表手动发放,2代表按用户+订单发放,3代表按订单发放)
		/// </summary>
		public int SendMode
		{
			set{ _sendmode=value;}
			get{return _sendmode;}
		}
		/// <summary>
		/// 领取方式(当且仅当发放方式为免费领取时有效.0代表无限制,1代表限领一次,2代表每天限领一次)
		/// </summary>
		public int GetMode
		{
			set{ _getmode=value;}
			get{return _getmode;}
		}
		/// <summary>
		/// 使用方式(0代表可以叠加,1代表不可以叠加)
		/// </summary>
		public int UseMode
		{
			set{ _usemode=value;}
			get{return _usemode;}
		}
		/// <summary>
		/// 最低用户等级
		/// </summary>
        public int UserRankLower
		{
			set{ _userranklower=value;}
			get{return _userranklower;}
		}
		/// <summary>
		/// 最低订单金额
		/// </summary>
		public int OrderAmountLower
		{
			set{ _orderamountlower=value;}
			get{return _orderamountlower;}
		}
		/// <summary>
		/// 限制店铺分类id
		/// </summary>
        public int LimitStoreCid
		{
			set{ _limitstorecid=value;}
			get{return _limitstorecid;}
		}
		/// <summary>
		/// 是否限制商品
		/// </summary>
		public int LimitProduct
		{
			set{ _limitproduct=value;}
			get{return _limitproduct;}
		}
		/// <summary>
		/// 发放开始时间
		/// </summary>
		public DateTime SendStartTime
		{
			set{ _sendstarttime=value;}
			get{return _sendstarttime;}
		}
		/// <summary>
		/// 发放结束时间
		/// </summary>
		public DateTime SendEndTime
		{
			set{ _sendendtime=value;}
			get{return _sendendtime;}
		}
		/// <summary>
		/// 使用相对时间
		/// </summary>
		public int UseExpireTime
		{
			set{ _useexpiretime=value;}
			get{return _useexpiretime;}
		}
		/// <summary>
		/// 使用开始时间
		/// </summary>
        public DateTime UseStartTime
		{
			set{ _usestarttime=value;}
			get{return _usestarttime;}
		}
		/// <summary>
		/// 使用结束时间
		/// </summary>
		public DateTime UseEndTime
		{
			set{ _useendtime=value;}
			get{return _useendtime;}
		}
		#endregion Model

	}
}

