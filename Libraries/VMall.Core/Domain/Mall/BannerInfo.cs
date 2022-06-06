using System;

namespace VMall.Core
{
    /// <summary>
    /// banner信息类
    /// </summary>
    public class BannerInfo
    {
        private int _id;//编号
        private int _type;//类型(0代表PC,1代表手机)
        private DateTime _starttime;//开始时间
        private DateTime _endtime;//结束时间
        private int _isshow;//是否显示
        private string _title;//标题
        private string _img;//图片
        private string _url;//网址
        private int _displayorder;//排序
        private string _extfield1=string.Empty;//扩展字段1
        private string _extfield2=string.Empty;//扩展字段2
        private string _extfield3=string.Empty;//扩展字段3

        /// <summary>
        /// 编号
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        /// <summary>
        /// 类型(0代表PC,1代表手机)
        /// </summary>
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime
        {
            get { return _starttime; }
            set { _starttime = value; }
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime
        {
            get { return _endtime; }
            set { _endtime = value; }
        }
        /// <summary>
        /// 是否显示
        /// </summary>
        public int IsShow
        {
            get { return _isshow; }
            set { _isshow = value; }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get { return _title; }
            set { _title = value.TrimEnd(); }
        }
        /// <summary>
        /// 图片
        /// </summary>
        public string Img
        {
            get { return _img; }
            set { _img = value.TrimEnd(); }
        }
        /// <summary>
        /// 网址
        /// </summary>
        public string Url
        {
            get { return _url; }
            set { _url = value.TrimEnd(); }
        }
        /// <summary>
        /// 排序
        /// </summary>
        public int DisplayOrder
        {
            get { return _displayorder; }
            set { _displayorder = value; }
        }
        /// <summary>
        /// 扩展字段1
        /// </summary>
        public string ExtField1
        {
            get { return _extfield1; }
            set { _extfield1 = value.TrimEnd(); }
        }
        /// <summary>
        /// 扩展字段2
        /// </summary>
        public string ExtField2
        {
            get { return _extfield2; }
            set { _extfield2 = value.TrimEnd(); }
        }
        /// <summary>
        /// 扩展字段3
        /// </summary>
        public string ExtField3
        {
            get { return _extfield3; }
            set { _extfield3 = value.TrimEnd(); }
        }
    }
}
