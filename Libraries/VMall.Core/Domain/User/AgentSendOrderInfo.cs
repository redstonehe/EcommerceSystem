using System;
namespace VMall.Core
{
	/// <summary>
	/// ��������
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
		/// ��¼id
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime CreationDate
		{
			set{ _creationdate=value;}
			get{return _creationdate;}
		}
		/// <summary>
		/// ��Ʒid
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
		}
		/// <summary>
		/// ��Աid
		/// </summary>
		public int Uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// ��������
		/// </summary>
		public string SendOSN
		{
			set{ _sendosn=value;}
			get{return _sendosn;}
		}
		/// <summary>
		/// ����״̬
		/// </summary>
		public int SendState
		{
			set{ _sendstate=value;}
			get{return _sendstate;}
		}
		/// <summary>
		/// ��������
		/// </summary>
		public int SendCount
		{
			set{ _sendcount=value;}
			get{return _sendcount;}
		}
		/// <summary>
		/// ������ݵ���
		/// </summary>
		public string ShipSN
		{
			set{ _shipsn=value;}
			get{return _shipsn;}
		}
		/// <summary>
		/// ��ݹ�˾id
		/// </summary>
		public int ShipCoid
		{
			set{ _shipcoid=value;}
			get{return _shipcoid;}
		}
		/// <summary>
		/// �������
		/// </summary>
		public string ShipCoName
		{
			set{ _shipconame=value;}
			get{return _shipconame;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime ShipTime
		{
			set{ _shiptime=value;}
			get{return _shiptime;}
		}
		/// <summary>
		/// �˷�
		/// </summary>
		public decimal ShipFee
		{
			set{ _shipfee=value;}
			get{return _shipfee;}
		}
		/// <summary>
		/// ����id
		/// </summary>
		public int RegionId
		{
			set{ _regionid=value;}
			get{return _regionid;}
		}
		/// <summary>
		/// ��ϸ��ַ
		/// </summary>
		public string Address
		{
			set{ _address=value;}
			get{return _address;}
		}
		/// <summary>
		/// �ջ���
		/// </summary>
		public string Consignee
		{
			set{ _consignee=value;}
			get{return _consignee;}
		}
		/// <summary>
		/// �ֻ�
		/// </summary>
		public string Mobile
		{
			set{ _mobile=value;}
			get{return _mobile;}
		}
		/// <summary>
		/// ��ע
		/// </summary>
		public string BuyerRemark
		{
			set{ _buyerremark=value;}
			get{return _buyerremark;}
		}
		/// <summary>
		/// �ջ�ʱ��
		/// </summary>
		public DateTime ReceivingTime
		{
			set{ _receivingtime=value;}
			get{return _receivingtime;}
		}
		/// <summary>
		/// �Ƿ��ӳ��ջ�
		/// </summary>
		public int IsExtendReceive
		{
			set{ _isextendreceive=value;}
			get{return _isextendreceive;}
		}
		#endregion Model

	}
}

