var menuList = [
    {
        "title": "商品管理",
        "subMenuList": [
            { "title": "添加商品", "url": "/malladmin/product/addproduct" },
            { "title": "添加SKU", "url": "/malladmin/product/addsku" },
            { "title": "在售商品", "url": "/malladmin/product/onsaleproductlist" },
            { "title": "下架商品", "url": "/malladmin/product/outsaleproductlist" },
            //{ "title": "定时商品", "url": "/malladmin/product/timeproductlist" },
            //{ "title": "回收站", "url": "/malladmin/product/recyclebinproductlist" },
            { "title": "商品品牌", "url": "/malladmin/brand/list" },
            { "title": "商品分类", "url": "/malladmin/category/categorylist" },
            { "title": "频道分区", "url": "/malladmin/channel/list" },
            //{ "title": "二级主页", "url": "/malladmin/channel/IndexList" }
        ],
        "icon": "fa-archive"
    },
    {
        "title": "促销活动",
        "subMenuList": [
            { "title": "单品促销", "url": "/malladmin/promotion/singlepromotionlist" },
            //{ "title": "买送促销", "url": "/malladmin/promotion/buysendpromotionlist" },
            { "title": "买赠促销", "url": "/malladmin/promotion/giftpromotionlist" },
            { "title": "套装促销", "url": "/malladmin/promotion/suitpromotionlist" },
            { "title": "满赠促销", "url": "/malladmin/promotion/fullsendpromotionlist" },
            { "title": "满减促销", "url": "/malladmin/promotion/fullcutpromotionlist" },
            //{ "title": "专题管理", "url": "/malladmin/topic/list" },
            { "title": "优惠劵", "url": "/malladmin/coupon/coupontypelist" },
            //{ "title": "兑换码", "url": "/malladmin/exchangecoupon/codetypelist" }
        ],
        "icon": "fa-expeditedssl"
    },
    {
        "title": "订单管理",
        "subMenuList": [
            { "title": "订单列表", "url": "/malladmin/order/orderlist" },
            { "title": "退货列表", "url": "/malladmin/order/returnlist" },
            { "title": "退款列表", "url": "/malladmin/order/refundlist" },
            { "title": "换货列表", "url": "/malladmin/order/orderchangelist" },
            //{ "title": "代理要货单", "url": "/malladmin/agentsendorder/SendOrderList" }
        ],
        "icon": "fa-shopping-cart"
    },
    {
        "title": "用户管理",
        "subMenuList": [
            { "title": "用户列表", "url": "/malladmin/user/list" },
            { "title": "引流赠品", "url": "/malladmin/ordergift/ordergiftlist" },
            //{ "title": "会员等级", "url": "/malladmin/userrank/list" },
            //{ "title": "汇购卡", "url": "/malladmin/cashcoupon/cashcouponlist" },
            //{ "title": "海米/代理提现", "url": "/malladmin/HaimiDrawCash/list" },
            //{ "title": "用户账户查询", "url": "/malladmin/HaimiDrawCash/useraccountlist" },
            { "title": "佣金查询", "url": "/malladmin/HaimiDrawCash/incomeaccount" },
            { "title": "佣金提现", "url": "/malladmin/HaimiDrawCash/incomelist" },
            //{ "title": "尚睿淳余额转出", "url": "/malladmin/kf2severb/kf2severbapplylist" },
            { "title": "会员投诉", "url": "/malladmin/complain/complainlist" },
            { "title": "消息类型", "url": "/malladmin/inform/typelist" },
            { "title": "会员消息", "url": "/malladmin/inform/informlist" }
        ],
        "icon": "fa-user"
    },
     //{
     //    "title": "报单管理",
     //    "subMenuList": [
     //        //{ "title": "代理会员录入", "url": "/malladmin/home/OutLineAgent" },             
     //        { "title": "有机胚芽报单", "url": "/malladmin/home/AgentOrderSelect" },
     //        { "title": "1980报单申请", "url": "/malladmin/dsorder/orderapplylist" }
     //    ],
     //    "icon": "fa-list-alt"
     //},
    {
        "title": "商家店铺管理",
        "subMenuList": [
            { "title": "店铺列表", "url": "/malladmin/store/storelist" },
            //{ "title": "店铺行业", "url": "/malladmin/storeindustry/list" },
            //{ "title": "店铺等级", "url": "/malladmin/storerank/list" },
            //{ "title": "商家入驻", "url": "javascript:void(0)" }
        ],
        "icon": "fa-cubes"
    },
     {
         "title": "广告评价管理",
         "subMenuList": [
            { "title": "广告位置", "url": "/malladmin/advert/advertpositionlist" },
            { "title": "广告列表", "url": "/malladmin/advert/advertlist" },
            { "title": "Banner", "url": "/malladmin/banner/list" },
            { "title": "商品评价", "url": "/malladmin/productreview/productreviewlist" },
            //{ "title": "商品咨询", "url": "/malladmin/productconsult/productconsultlist" },
            //{ "title": "咨询类型", "url": "/malladmin/productconsult/productconsulttypelist" }
         ],
         "icon": "fa-laptop"
     },
    {
        "title": "商城内容",
        "subMenuList": [
            //{ "title": "导航菜单", "url": "/malladmin/nav/list" },
            { "title": "商城帮助", "url": "/malladmin/help/list" },
            { "title": "友情链接", "url": "/malladmin/friendlink/list" },
            { "title": "新闻类型", "url": "/malladmin/news/newstypelist" },
            { "title": "新闻列表", "url": "/malladmin/news/newslist" }
        ],
        "icon": "fa-comments-o"
    },

    {
        "title": "报表统计",
        "subMenuList": [
            //{ "title": "在线用户", "url": "/malladmin/stat/onlineuserlist" },
            { "title": "搜索分析", "url": "/malladmin/stat/searchwordstatlist" },
            { "title": "商品统计", "url": "/malladmin/stat/productstat" },
            { "title": "销售明细", "url": "/malladmin/stat/salelist" },
            
            //{ "title": "销售趋势", "url": "/malladmin/stat/saletrend" },
            { "title": "地区统计", "url": "/malladmin/stat/regionstat" },
            //{ "title": "客户端统计", "url": "/malladmin/stat/clientstat" },
            { "title": "销售业绩报表", "url": "/malladmin/stat/saleresult" },
            { "title": "退款统计", "url": "/malladmin/stat/refundstatlist" }
        ],
        "icon": "fa-bar-chart"
    },
    {
        "title": "系统设置",
        "subMenuList": [
            { "title": "站点信息", "url": "/malladmin/set/site" },
            { "title": "商城设置", "url": "/malladmin/set/mall" },
            { "title": "账户设置", "url": "/malladmin/set/account" },
            { "title": "上传设置", "url": "/malladmin/set/upload" },
            { "title": "性能设置", "url": "/malladmin/set/performance" },
            //{ "title": "访问控制", "url": "/malladmin/set/access" },
            { "title": "邮箱设置", "url": "/malladmin/set/email" },
            //{ "title": "短信设置", "url": "/malladmin/set/sms" },
            //{ "title": "积分设置", "url": "/malladmin/set/credit" },
            //{ "title": "打印订单", "url": "/malladmin/set/printorder" },
            { "title": "配送公司", "url": "/malladmin/shipcompany/list" },
            //{ "title": "禁止IP", "url": "/malladmin/bannedip/list" },
            //{ "title": "筛选词", "url": "/malladmin/filterword/list" }
        ],
        "icon": "fa-cogs"
    }
    ,
    {
        "title": "日志管理",
        "subMenuList": [
            { "title": "商城日志", "url": "/malladmin/log/malladminloglist" },
            //{ "title": "店铺日志", "url": "/malladmin/log/storeadminloglist" },
            //{ "title": "积分日志", "url": "/malladmin/log/creditloglist" }
            { "title": "错误日志", "url": "/malladmin/log/errorloglist" },
        ],
        "icon": "fa-database"
    }
    //,
    //{
    //    "title": "开发管理",
    //    "subMenuList": [
    //        { "title": "事件管理", "url": "/malladmin/event/list" },
    //        { "title": "数据库管理", "url": "/malladmin/database/manage" },
    //        { "title": "错误日志", "url": "/malladmin/log/errorloglist" },
    //        { "title": "插件管理", "url": "/malladmin/plugin/list" }
    //    ]
    //}
    ,
    {
        "title": "权限管理",
        "subMenuList": [
            { "title": "管理员组", "url": "/malladmin/malladmingroup/list" },
            { "title": "菜单列表", "url": "/malladmin/malladminaction/list" },
            { "title": "插件管理", "url": "/malladmin/plugin/list?type=1" }
        ],
        "icon": "fa-group"
    }
    ,
    {
        "title": "系统帮助",
        "subMenuList": [
            { "title": "操作指南", "url": "/malladmin/mallguide/useGuide" },
            { "title": "系统公告", "url": "/malladmin/mallguide/systemnote" },
            { "title": "更新日志", "url": "/malladmin/mallguide/updatelog" },
            { "title": "商城工具", "url": "/malladmin/mallguide/admintools" }
        ],
        "icon": "fa-cutlery"
    }
]