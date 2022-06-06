using System;
namespace VMall.Core
{
	/// <summary>
	/// 代理发货表
	/// </summary>
	[Serializable]
	public partial class AgentSendOrderInfo
	{
		public AgentSendOrderInfo()
		{}
		#region Model
		private int _id;
		private DateTime _creationdate= DateTime.Now;
		private int _pid=0;
		private int _uid=0;
		private string _sendosn="";
		private int _sendstate=0;
		private int _sendcount=0;
		private string _shipsn="";
		private int _shipcoid=0;
		private string _shipconame="";
		private DateTime _shiptime= DateTime.Now;
		private decimal _shipfee=0.00M;
		private int _regionid=0;
		private string _address="";
		private string _consignee="";
		private string _mobile="";
		private string _buyerremark="";
		private DateTime _receivingtime= DateTime.Now;
		private int _isextendreceive=0;
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
		/// 产品id
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
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
		/// 发货单号
		/// </summary>
		public string SendOSN
		{
			set{ _sendosn=value;}
			get{return _sendosn;}
		}
		/// <summary>
		/// 发货状态
		/// </summary>
		public int SendState
		{
			set{ _sendstate=value;}
			get{return _sendstate;}
		}
		/// <summary>
		/// 发货数量
		/// </summary>
		public int SendCount
		{
			set{ _sendcount=value;}
			get{return _sendcount;}
		}
		/// <summary>
		/// 发货快递单号
		/// </summary>
		public string ShipSN
		{
			set{ _shipsn=value;}
			get{return _shipsn;}
		}
		/// <summary>
		/// 快递公司id
		/// </summary>
		public int ShipCoid
		{
			set{ _shipcoid=value;}
			get{return _shipcoid;}
		}
		/// <summary>
		/// 快递名称
		/// </summary>
		public string ShipCoName
		{
			set{ _shipconame=value;}
			get{return _shipconame;}
		}
		/// <summary>
		/// 发货时间
		/// </summary>
		public DateTime ShipTime
		{
			set{ _shiptime=value;}
			get{return _shiptime;}
		}
		/// <summary>
		/// 运费
		/// </summary>
		public decimal ShipFee
		{
			set{ _shipfee=value;}
			get{return _shipfee;}
		}
		/// <summary>
		/// 地区id
		/// </summary>
		public int RegionId
		{
			set{ _regionid=value;}
			get{return _regionid;}
		}
		/// <summary>
		/// 详细地址
		/// </summary>
		public string Address
		{
			set{ _address=value;}
			get{return _address;}
		}
		/// <summary>
		/// 收货人
		/// </summary>
		public string Consignee
		{
			set{ _consignee=value;}
			get{return _consignee;}
		}
		/// <summary>
		/// 手机
		/// </summary>
		public string Mobile
		{
			set{ _mobile=value;}
			get{return _mobile;}
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string BuyerRemark
		{
			set{ _buyerremark=value;}
			get{return _buyerremark;}
		}
		/// <summary>
		/// 收货时间
		/// </summary>
		public DateTime ReceivingTime
		{
			set{ _receivingtime=value;}
			get{return _receivingtime;}
		}
		/// <summary>
		/// 是否延长收货
		/// </summary>
		public int IsExtendReceive
		{
			set{ _isextendreceive=value;}
			get{return _isextendreceive;}
		}
		#endregion Model

	}
}

