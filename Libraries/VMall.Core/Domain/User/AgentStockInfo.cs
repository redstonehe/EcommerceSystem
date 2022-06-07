using System;
namespace VMall.Core
{
	/// <summary>
	/// ������
	/// </summary>
	[Serializable]
	public partial class AgentStockInfo
	{
		public AgentStockInfo()
		{}
		#region Model
		private int _asid;
		private int _uid=0;
		private int _pid=0;
		private decimal _balance=0M;

        
        private decimal _agentamount = 0M;//��������
		/// <summary>
		/// ��¼id
		/// </summary>
		public int Asid
		{
			set{ _asid=value;}
			get{return _asid;}
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
		/// ����������
		/// </summary>
		public decimal Balance
		{
			set{ _balance=value;}
			get{return _balance;}
		}
        
        /// <summary>
        /// ��������
        /// </summary>
        public decimal AgentAmount
        {
            set { _agentamount = value; }
            get { return _agentamount; }
        }

		#endregion Model

	}
}

