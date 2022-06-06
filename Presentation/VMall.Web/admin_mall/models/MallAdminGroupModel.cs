using System;
using System.Data;
using System.Web.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using VMall.Core;
using VMall.Services;


namespace VMall.Web.MallAdmin.Models
{
    /// <summary>
    /// 商城管理员组列表模型类
    /// </summary>
    public class MallAdminGroupListModel
    {
        /// <summary>
        /// 商城管理员组列表
        /// </summary>
        public MallAdminGroupInfo[] MallAdminGroupList { get; set; }
    }

    /// <summary>
    /// 商城管理员组模型类
    /// </summary>
    public class MallAdminGroupModel
    {
        public int MallAGid { get; set; }
        /// <summary>
        /// 管理员组标题
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(25, ErrorMessage = "名称长度不能大于25")]
        public string AdminGroupTitle { get; set; }

        /// <summary>
        /// 动作列表
        /// </summary>
        public string[] ActionList { get; set; }
    }

    /// <summary>
    /// 商城菜单列表模型类
    /// </summary>
    public class MallAdminActionListModel
    {
        /// <summary>
        /// 商城菜单组列表
        /// </summary>
        public List<MallAdminActionInfo> MallAdminActionList { get; set; }
    }
    /// <summary>
    /// 商城菜单模型类
    /// </summary>
    public class MallAdminActionModel
    {
        /// <summary>
        /// 菜单标题
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [StringLength(25, ErrorMessage = "名称长度不能大于25")]
        public string Title { get; set; }

        /// <summary>
        /// 动作
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 父id
        /// </summary>
        [Required(ErrorMessage = "父id不能为空")]
        [Range(0, int.MaxValue, ErrorMessage = "父id必须大于等于0")]
        public int ParentId { get; set; }
        /// <summary>
        /// 显示（0表示对普通管理员显示，1表示仅对开发管理员显示）
        /// </summary>
        [Required(ErrorMessage = "显示为空")]
        [Range(0, int.MaxValue, ErrorMessage = "显示必须大于等于0")]
        public int DisplayOrder { get; set; }
    }

    /// <summary>
    /// 操作权限模型类
    /// </summary>
    public class OperateRightsModel
    {
        public int Aid { get; set; }
        public int mallAGid { get; set; }
        public MallAdminActionInfo ActionInfo { get; set; }
        public MallAdminActionInfo ParentActionInfo { get; set; }
        public MallAdminGroupInfo adminGroup { get; set; }
        /// <summary>
        /// 已有操作列表
        /// </summary>
        public string[] HasRightsList { get; set; }
        /// <summary>
        /// 所有操作列表
        /// </summary>
        public List<AdminOperateRightsInfo> ALLRightsList { get; set; }
        /// <summary>
        /// 已选操作列表
        /// </summary>
        public string[] SelectRightsList { get; set; }
    }

    /// <summary>
    /// 操作权限列表模型类
    /// </summary>
    public class RightsListModel
    {
        public int Aid { get; set; }
        public int mallAGid { get; set; }
        public List<AdminOperateInfo> list { get; set; }
        public MallAdminActionInfo ActionInfo { get; set; }
        public MallAdminActionInfo ParentActionInfo { get; set; }
    }

    /// <summary>
    /// 操作权限模型类
    /// </summary>
    public class OperateModel
    {
        public int aid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "操作不能为空")]
        [StringLength(25, ErrorMessage = "操作长度不能大于25")]
        public string Operate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Required(ErrorMessage = "操作名称不能为空")]
        [StringLength(25, ErrorMessage = "操作名称长度不能大于25")]
        public string OperateName { get; set; }
    }
}
