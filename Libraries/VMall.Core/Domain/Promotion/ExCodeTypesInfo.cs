using System;
namespace VMall.Core
{
	/// <summary>
	/// �һ������ͱ�
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
		/// �һ�������id
		/// </summary>
		public int CodeTypeId
		{
			set{ _codetypeid=value;}
			get{return _codetypeid;}
		}
		/// <summary>
		/// ����id
		/// </summary>
		public int StoreId
		{
			set{ _storeid=value;}
			get{return _storeid;}
		}
		/// <summary>
		/// ״̬
		/// </summary>
		public int State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// ����
		/// </summary>
		public string Name
		{
			set{ _name=value;}
			get{return _name;}
		}
		/// <summary>
		/// ���
		/// </summary>
		public int Money
		{
			set{ _money=value;}
			get{return _money;}
		}
		/// <summary>
		/// ����
		/// </summary>
		public int Count
		{
			set{ _count=value;}
			get{return _count;}
		}
		/// <summary>
		/// ���ŷ�ʽ(0������ѷ���,1�����ֶ�����,2�����û�+��������,3������������)
		/// </summary>
		public int SendMode
		{
			set{ _sendmode=value;}
			get{return _sendmode;}
		}
		/// <summary>
		/// ��ȡ��ʽ(���ҽ������ŷ�ʽΪ�����ȡʱ��Ч.0����������,1��������һ��,2����ÿ������һ��)
		/// </summary>
		public int GetMode
		{
			set{ _getmode=value;}
			get{return _getmode;}
		}
		/// <summary>
		/// ʹ�÷�ʽ(0������Ե���,1�������Ե���)
		/// </summary>
		public int UseMode
		{
			set{ _usemode=value;}
			get{return _usemode;}
		}
		/// <summary>
		/// ����û��ȼ�
		/// </summary>
        public int UserRankLower
		{
			set{ _userranklower=value;}
			get{return _userranklower;}
		}
		/// <summary>
		/// ��Ͷ������
		/// </summary>
		public int OrderAmountLower
		{
			set{ _orderamountlower=value;}
			get{return _orderamountlower;}
		}
		/// <summary>
		/// ���Ƶ��̷���id
		/// </summary>
        public int LimitStoreCid
		{
			set{ _limitstorecid=value;}
			get{return _limitstorecid;}
		}
		/// <summary>
		/// �Ƿ�������Ʒ
		/// </summary>
		public int LimitProduct
		{
			set{ _limitproduct=value;}
			get{return _limitproduct;}
		}
		/// <summary>
		/// ���ſ�ʼʱ��
		/// </summary>
		public DateTime SendStartTime
		{
			set{ _sendstarttime=value;}
			get{return _sendstarttime;}
		}
		/// <summary>
		/// ���Ž���ʱ��
		/// </summary>
		public DateTime SendEndTime
		{
			set{ _sendendtime=value;}
			get{return _sendendtime;}
		}
		/// <summary>
		/// ʹ�����ʱ��
		/// </summary>
		public int UseExpireTime
		{
			set{ _useexpiretime=value;}
			get{return _useexpiretime;}
		}
		/// <summary>
		/// ʹ�ÿ�ʼʱ��
		/// </summary>
        public DateTime UseStartTime
		{
			set{ _usestarttime=value;}
			get{return _usestarttime;}
		}
		/// <summary>
		/// ʹ�ý���ʱ��
		/// </summary>
		public DateTime UseEndTime
		{
			set{ _useendtime=value;}
			get{return _useendtime;}
		}
		#endregion Model

	}
}

