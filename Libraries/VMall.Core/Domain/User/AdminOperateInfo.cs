using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// AdminOperateInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class AdminOperateInfo
    {
        public AdminOperateInfo()
		{}
		#region Model
		private int _operateid;
		private string _operate="";
		private string _operatename="‘’";
		private int _aid=0;
		/// <summary>
		/// 记录id
		/// </summary>
		public int OperateId
		{
			set{ _operateid=value;}
			get{return _operateid;}
		}
		/// <summary>
		/// 操作类型id
		/// </summary>
		public string Operate
		{
			set{ _operate=value;}
			get{return _operate;}
		}
		/// <summary>
		/// 操作类型名称
		/// </summary>
		public string OperateName
		{
			set{ _operatename=value;}
			get{return _operatename;}
		}
		/// <summary>
		/// 菜单id
		/// </summary>
		public int Aid
		{
			set{ _aid=value;}
			get{return _aid;}
		}
		#endregion Model
    }
}
