using System;

namespace VMall.Core
{
	/// <summary>
	/// 兑换码
	/// </summary>
	[Serializable]
	public partial class ExChangeCouponsInfo
	{
		public ExChangeCouponsInfo()
		{}
		#region Model
		private int _exid;
		private string _exsn="";
		private int _uid=0;
		private int _type=0;
		private int _state=0;
		private int _storeid=0;
		private int _oid=0;
		private DateTime _usetime= DateTime.Now;
		private DateTime _validtime= DateTime.Now;
		private string _useip="";
		private int _createuid=0;
		private int _createoid=0;
		private DateTime _createtime= DateTime.Now;
        private int _codetypeid = 0;
		/// <summary>
		/// 兑换码id
		/// </summary>
		public int exid
		{
			set{ _exid=value;}
			get{return _exid;}
		}
		/// <summary>
		/// 兑换码编号
		/// </summary>
		public string exsn
		{
			set{ _exsn=value;}
			get{return _exsn;}
		}
		/// <summary>
		/// 会员id
		/// </summary>
		public int uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// 兑换码类型，1为订单赠送，2为后台赠送
		/// </summary>
		public int type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// 状态
		/// </summary>
		public int state
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// 店铺id
		/// </summary>
		public int storeid
		{
			set{ _storeid=value;}
			get{return _storeid;}
		}
		/// <summary>
		/// 订单id
		/// </summary>
		public int oid
		{
			set{ _oid=value;}
			get{return _oid;}
		}
		/// <summary>
		/// 使用时间
		/// </summary>
		public DateTime usetime
		{
			set{ _usetime=value;}
			get{return _usetime;}
		}
		/// <summary>
		/// 
		/// </summary>
		public DateTime validtime
		{
			set{ _validtime=value;}
			get{return _validtime;}
		}
		/// <summary>
		/// 使用ip
		/// </summary>
		public string useip
		{
			set{ _useip=value;}
			get{return _useip;}
		}
		/// <summary>
		/// 创建会员id
		/// </summary>
		public int createuid
		{
			set{ _createuid=value;}
			get{return _createuid;}
		}
		/// <summary>
		/// 创建订单id
		/// </summary>
		public int createoid
		{
			set{ _createoid=value;}
			get{return _createoid;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime createtime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
        /// <summary>
        /// 兑换码类型id
        /// </summary>
        public int codetypeid {
            set { _codetypeid = value; }
            get { return _codetypeid; }
        }
		#endregion Model

	}
}

