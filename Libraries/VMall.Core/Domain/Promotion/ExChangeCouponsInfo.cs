using System;

namespace VMall.Core
{
	/// <summary>
	/// �һ���
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
		/// �һ���id
		/// </summary>
		public int exid
		{
			set{ _exid=value;}
			get{return _exid;}
		}
		/// <summary>
		/// �һ�����
		/// </summary>
		public string exsn
		{
			set{ _exsn=value;}
			get{return _exsn;}
		}
		/// <summary>
		/// ��Աid
		/// </summary>
		public int uid
		{
			set{ _uid=value;}
			get{return _uid;}
		}
		/// <summary>
		/// �һ������ͣ�1Ϊ�������ͣ�2Ϊ��̨����
		/// </summary>
		public int type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// ״̬
		/// </summary>
		public int state
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// ����id
		/// </summary>
		public int storeid
		{
			set{ _storeid=value;}
			get{return _storeid;}
		}
		/// <summary>
		/// ����id
		/// </summary>
		public int oid
		{
			set{ _oid=value;}
			get{return _oid;}
		}
		/// <summary>
		/// ʹ��ʱ��
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
		/// ʹ��ip
		/// </summary>
		public string useip
		{
			set{ _useip=value;}
			get{return _useip;}
		}
		/// <summary>
		/// ������Աid
		/// </summary>
		public int createuid
		{
			set{ _createuid=value;}
			get{return _createuid;}
		}
		/// <summary>
		/// ��������id
		/// </summary>
		public int createoid
		{
			set{ _createoid=value;}
			get{return _createoid;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime createtime
		{
			set{ _createtime=value;}
			get{return _createtime;}
		}
        /// <summary>
        /// �һ�������id
        /// </summary>
        public int codetypeid {
            set { _codetypeid = value; }
            get { return _codetypeid; }
        }
		#endregion Model

	}
}

