using System;
namespace VMall.Core
{
	/// <summary>
	/// GroupProductInfo:ʵ����(����˵���Զ���ȡ���ݿ��ֶε�������Ϣ)
	/// </summary>
	[Serializable]
	public partial class GroupProductInfo
	{
		public GroupProductInfo()
		{}
		#region Model
		private int _groupid;
		private DateTime _creationtime= DateTime.Now;
		private string _grouptitle="";
		private string _grouplogo="";
		private int _channelid=0;
		private int _state=0;
		private DateTime _starttime= DateTime.Now;
		private DateTime _endtime= DateTime.Now;
		private int _displayorder=0;
		private int _type=0;
		private string _link="";
		private string _products="";
		private string _extfield1="";
		private string _extfield2="";
		private string _extfield3="";
		private string _extfield4="";
		private string _extfield5="";
		/// <summary>
		/// ����id
		/// </summary>
		public int Groupid
		{
			set{ _groupid=value;}
			get{return _groupid;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime CreationTime
		{
			set{ _creationtime=value;}
			get{return _creationtime;}
		}
		/// <summary>
		/// �������
		/// </summary>
		public string GroupTitle
		{
			set{ _grouptitle=value;}
			get{return _grouptitle;}
		}
		/// <summary>
		/// ����logo
		/// </summary>
		public string GroupLogo
		{
			set{ _grouplogo=value;}
			get{return _grouplogo;}
		}
		/// <summary>
		/// ��������id
		/// </summary>
		public int ChannelId
		{
			set{ _channelid=value;}
			get{return _channelid;}
		}
		/// <summary>
		/// ״̬ 0�ر� 1����
		/// </summary>
		public int State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// ��ʼʱ��
		/// </summary>
		public DateTime StartTime
		{
			set{ _starttime=value;}
			get{return _starttime;}
		}
		/// <summary>
		/// ����ʱ��
		/// </summary>
		public DateTime EndTime
		{
			set{ _endtime=value;}
			get{return _endtime;}
		}
		/// <summary>
		/// ����
		/// </summary>
		public int DisplayOrder
		{
			set{ _displayorder=value;}
			get{return _displayorder;}
		}
		/// <summary>
		/// �������� 
		/// </summary>
		public int Type
		{
			set{ _type=value;}
			get{return _type;}
		}
		/// <summary>
		/// ����
		/// </summary>
		public string Link
		{
			set{ _link=value;}
			get{return _link;}
		}
		/// <summary>
		/// �����ƷId
		/// </summary>
		public string Products
		{
			set{ _products=value;}
			get{return _products;}
		}
		/// <summary>
		/// ��չ�ֶ�1
		/// </summary>
		public string ExtField1
		{
			set{ _extfield1=value;}
			get{return _extfield1;}
		}
		/// <summary>
		/// ��չ�ֶ�2
		/// </summary>
		public string ExtField2
		{
			set{ _extfield2=value;}
			get{return _extfield2;}
		}
		/// <summary>
		/// ��չ�ֶ�3
		/// </summary>
		public string ExtField3
		{
			set{ _extfield3=value;}
			get{return _extfield3;}
		}
		/// <summary>
		/// ��չ�ֶ�4
		/// </summary>
		public string ExtField4
		{
			set{ _extfield4=value;}
			get{return _extfield4;}
		}
		/// <summary>
		/// ��չ�ֶ�5
		/// </summary>
		public string ExtField5
		{
			set{ _extfield5=value;}
			get{return _extfield5;}
		}
		#endregion Model

	}
}

