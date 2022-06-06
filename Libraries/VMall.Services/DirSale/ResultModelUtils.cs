using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VMall.Services.DirSale
{
    public class ResultModelUtils
    {
        /// <summary>
        /// 返回代码，0 表示请求成功 非0表示不成功
        /// </summary>
        public string Result { get; set; }
        /// <summary>
        /// 返回信息，
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 返回数据，成功时返回所需的数据
        /// </summary>
        public object Info { get; set; }
        
    }
}
