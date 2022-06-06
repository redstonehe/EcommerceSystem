using System;
namespace VMall.Core
{
	/// <summary>
	/// 代理库存
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

        
        private decimal _agentamount = 0M;//代理库存金额
		/// <summary>
		/// 记录id
		/// </summary>
		public int Asid
		{
			set{ _asid=value;}
			get{return _asid;}
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
		/// 代理库存数量
		/// </summary>
		public decimal Balance
		{
			set{ _balance=value;}
			get{return _balance;}
		}
        
        /// <summary>
        /// 代理库存金额
        /// </summary>
        public decimal AgentAmount
        {
            set { _agentamount = value; }
            get { return _agentamount; }
        }

		#endregion Model

	}
}

