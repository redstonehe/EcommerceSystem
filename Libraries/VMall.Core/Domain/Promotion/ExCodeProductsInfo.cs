using System;
namespace VMall.Core
{
	/// <summary>
	/// 兑换码商品表
	/// </summary>
	[Serializable]
	public partial class ExCodeProductsInfo
	{
		public ExCodeProductsInfo()
		{}
		#region Model
		private int _recordid;
		private int _codetypeid=0;
		private int _pid=0;
		/// <summary>
		/// 记录id
		/// </summary>
		public int RecordId
		{
			set{ _recordid=value;}
			get{return _recordid;}
		}
		/// <summary>
		/// 兑换码类型id
		/// </summary>
		public int CodeTypeId
		{
			set{ _codetypeid=value;}
			get{return _codetypeid;}
		}
		/// <summary>
		/// 商品id
		/// </summary>
		public int Pid
		{
			set{ _pid=value;}
			get{return _pid;}
		}
		#endregion Model

	}
}

