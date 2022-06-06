
using System;
namespace VMall.Core
{
	/// <summary>
	/// HaiMiDrawCash:实体类(属性说明自动提取数据库字段的描述信息)
	/// </summary>
	[Serializable]
	public  class HaiMiDrawCashInfo
	{
        public HaiMiDrawCashInfo()
		{}
		#region Model
		private int _id;
		private DateTime _createtime= DateTime.Now;
		private int _uid=0;
		private int _accountid=0;
		private string _drawcashsn="";
		private int _adminuid=0;
		private decimal _amount=0.00M;
		private decimal _poundage=0.00M;
		private decimal _actualamount=0.00M;
        private int _state = 1;// 1代表提交申请等待处理，2代表处理成功，0代表处理失败，已撤销
		private DateTime _paytime= DateTime.Now;
		private string _remark="";
		private int _bankid=0;
		private string _bankname="";
		private string _bankprovice="‘’";
		private string _bankcity="‘’";
		private string _bankaddress="";
		private string _bankcardcode="";
		private string _bankusername="";
		private string _result="";
        private decimal _taxamount = 0.00M;
		/// <summary>
		/// 
		/// </summary>
		public int Id
		{
			set{ _id=value;}
			get{return _id;}
		}
		/// <summary>
		/// 创建时间
		/// </summary>
		public DateTime CreateTime
		{
			set{ _createtime=value;}
			get{return _createtime;}
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
		/// 账户id
		/// </summary>
		public int AccountId
		{
			set{ _accountid=value;}
			get{return _accountid;}
		}
		/// <summary>
		/// 提现号
		/// </summary>
		public string DrawCashSN
		{
			set{ _drawcashsn=value;}
			get{return _drawcashsn;}
		}
		/// <summary>
		/// 操作员id
		/// </summary>
		public int AdminUid
		{
			set{ _adminuid=value;}
			get{return _adminuid;}
		}
		/// <summary>
		/// 金额
		/// </summary>
		public decimal Amount
		{
			set{ _amount=value;}
			get{return _amount;}
		}
		/// <summary>
		/// 手续费
		/// </summary>
		public decimal Poundage
		{
			set{ _poundage=value;}
			get{return _poundage;}
		}
		/// <summary>
		/// 实际金额
		/// </summary>
		public decimal ActualAmount
		{
			set{ _actualamount=value;}
			get{return _actualamount;}
		}
		/// <summary>
        /// 状态 1代表提交申请等待处理，2代表处理成功，0代表处理失败，已撤销
		/// </summary>
		public int State
		{
			set{ _state=value;}
			get{return _state;}
		}
		/// <summary>
		/// 提现时间
		/// </summary>
		public DateTime PayTime
		{
			set{ _paytime=value;}
			get{return _paytime;}
		}
		/// <summary>
		/// 备注
		/// </summary>
		public string Remark
		{
			set{ _remark=value;}
			get{return _remark;}
		}
		/// <summary>
		/// 银行id
   
		/// </summary>
		public int BankId
		{
			set{ _bankid=value;}
			get{return _bankid;}
		}
		/// <summary>
		/// 银行名字
		/// </summary>
		public string BankName
		{
			set{ _bankname=value;}
			get{return _bankname;}
		}
		/// <summary>
		/// 银行省份
		/// </summary>
		public string BankProvice
		{
			set{ _bankprovice=value;}
			get{return _bankprovice;}
		}
		/// <summary>
		/// 银行城市
		/// </summary>
		public string BankCity
		{
			set{ _bankcity=value;}
			get{return _bankcity;}
		}
		/// <summary>
		/// 银行开户行
		/// </summary>
		public string BankAddress
		{
			set{ _bankaddress=value;}
			get{return _bankaddress;}
		}
		/// <summary>
		/// 银行卡号
		/// </summary>
		public string BankCardCode
		{
			set{ _bankcardcode=value;}
			get{return _bankcardcode;}
		}
		/// <summary>
		/// 银行开户人
		/// </summary>
		public string BankUserName
		{
			set{ _bankusername=value;}
			get{return _bankusername;}
		}
		/// <summary>
		/// 处理结果
		/// </summary>
		public string Result
		{
			set{ _result=value;}
			get{return _result;}
		}
        /// <summary>
        /// 税费
        /// </summary>
        public decimal TaxAmount
        {
            set { _taxamount = value; }
            get { return _taxamount; }
        }
		#endregion Model

	}
}

