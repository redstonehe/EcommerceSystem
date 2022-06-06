using System;

namespace VMall.Core
{
    /// <summary>
    /// 店铺配送费用信息类
    /// </summary>
    public class StoreShipFeeInfo
    {
        private int _recordid;//记录id
        private int _storestid;//店铺配送模板id
        private string _regionid;//区域id 组合，每个省份id以,连接 默认地区-1
        private float _startvalue;//开始值 首重/首件
        private decimal _startfee;//开始费用 首费
        private float _addvalue;//添加值 续重/续件
        private decimal _addfee;//添加费用  续费
        private int _shiptype = 0;//运费类型 0表示满足包邮价格 1表示不满足包邮价格

        private string _cityid = "";//城市id 组合，每个城市id以,连接，默认地区空
        /// <summary>
        /// 记录id
        /// </summary>
        public int RecordId
        {
            get { return _recordid; }
            set { _recordid = value; }
        }
        /// <summary>
        /// 店铺配送模板id
        /// </summary>
        public int StoreSTid
        {
            get { return _storestid; }
            set { _storestid = value; }
        }
        /// <summary>
        /// 区域id
        /// </summary>
        public string  RegionId
        {
            get { return _regionid; }
            set { _regionid = value.TrimEnd(); }
        }
        
        /// <summary>
        /// 开始值
        /// </summary>
        public float StartValue
        {
            get { return _startvalue; }
            set { _startvalue = value; }
        }
        /// <summary>
        /// 开始费用
        /// </summary>
        public decimal StartFee
        {
            get { return _startfee; }
            set { _startfee = value; }
        }
        /// <summary>
        /// 添加值
        /// </summary>
        public float AddValue
        {
            get { return _addvalue; }
            set { _addvalue = value; }
        }
        /// <summary>
        /// 添加费用
        /// </summary>
        public decimal AddFee
        {
            get { return _addfee; }
            set { _addfee = value; }
        }
        /// <summary>
        /// 运费类型 0表示满足包邮价格 1表示不满足包邮价格
        /// </summary>
        public int ShipType
        {
            get { return _shiptype; }
            set { _shiptype = value; }
        }
        /// <summary>
        /// 城市id
        /// </summary>
        public string CityId
        {
            get { return _cityid; }
            set { _cityid = value.TrimEnd(); }
        }
    }
}
