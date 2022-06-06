using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Core
{
    /// <summary>
    /// AdminOperateRightsInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public class AdminOperateRightsInfo
    {
        public AdminOperateRightsInfo()
		{}
        #region Model
        private int _rightsid;
        private string _operate = "";
        private int _aid = 0;
        private int _mallagid = 0;
        private int _state = 0;
        /// <summary>
        /// 记录id
        /// </summary>
        public int RightsId
        {
            set { _rightsid = value; }
            get { return _rightsid; }
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string Operate
        {
            set { _operate = value; }
            get { return _operate; }
        }
        /// <summary>
        /// 菜单id
        /// </summary>
        public int Aid
        {
            set { _aid = value; }
            get { return _aid; }
        }
        /// <summary>
        /// 管理组id
        /// </summary>
        public int MallAGid
        {
            set { _mallagid = value; }
            get { return _mallagid; }
        }
        /// <summary>
        /// 状态，0无效，1有效
        /// </summary>
        public int State
        {
            set { _state = value; }
            get { return _state; }
        }
        #endregion Model
    }
}
