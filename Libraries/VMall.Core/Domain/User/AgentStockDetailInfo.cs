using System;
namespace VMall.Core
{
	/// <summary>
	/// ����������
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
		private string _ordercode="����";
		private string _detaildesc="����";
		private int _fromuser=0;
		private int _touser=0;
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
		/// ��Աid
		/// </summary>
		public int Uid
		{
			set{ _uid=value;}
			get{return _uid;}
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
		/// ��¼���ͣ�1���ӣ�2����
		/// </summary>
		public int DetailType
		{
			set{ _detailtype=value;}
			get{return _detailtype;}
		}
		/// <summary>
		/// �������
		/// </summary>
        public decimal InAmount
		{
			set{ _inamount=value;}
			get{return _inamount;}
		}
		/// <summary>
		/// ���֧��
		/// </summary>
        public decimal OutAmount
		{
			set{ _outamount=value;}
			get{return _outamount;}
		}
		/// <summary>
		/// ��ǰ������
		/// </summary>
		public decimal CurrentBalance
		{
			set{ _currentbalance=value;}
			get{return _currentbalance;}
		}
		/// <summary>
		/// ����������
		/// </summary>
		public string OrderCode
		{
			set{ _ordercode=value;}
			get{return _ordercode;}
		}
		/// <summary>
		/// ��¼����
		/// </summary>
		public string DetailDesc
		{
			set{ _detaildesc=value;}
			get{return _detaildesc;}
		}
		/// <summary>
		/// ��Դ��Աid��0��ʾ��Դ��˾
		/// </summary>
		public int FromUser
		{
			set{ _fromuser=value;}
			get{return _fromuser;}
		}
		/// <summary>
		/// ȥ�����Ա
		/// </summary>
		public int ToUser
		{
			set{ _touser=value;}
			get{return _touser;}
		}
		#endregion Model

	}
}

