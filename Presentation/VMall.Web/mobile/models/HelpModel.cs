using System;
using System.Collections.Generic;

using VMall.Core;
using VMall.Services;
using VMall.Web.Framework;

namespace VMall.Web.Mobile.Models
{
    /// <summary>
    /// 问题模型类
    /// </summary>
    public class QuestionModel
    {
        public HelpInfo HelpInfo { get; set; }
        public List<HelpInfo> HelpList { get; set; }
    }
}