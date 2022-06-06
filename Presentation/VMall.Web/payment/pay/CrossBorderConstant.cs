using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Com.LaKaLa
{
    public class CrossBorderConstant
    {
        /**银行接口超时时间**/
	public const String TIME_OUT = "55000";
	
	public const String SUCCESS = "0000";
	public const String SUCCESS_MSG = "操作成功";
	
	public const String ERROR_0001 = "0001";
	public const String ERROR_0001_MSG = "请求失败";
	
	public const String ERROR_0002 = "0002";
	public const String ERROR_0002_MSG = "请求参数错误";
	
	public const String ERROR_0003 = "0003";
	public const String ERROR_0003_MSG = "获取业务处理类失败";
	
	public const String ERROR_0004 = "0004";
	public const String ERROR_0004_MSG = "后台业务处理返回为空";
	
	public const String ERROR_0005 = "0005";
	public const String ERROR_0005_MSG = "银行返回为空";
	
	public const String ERROR_0006 = "0006";
	public const String ERROR_0006_MSG = "银行处理失败";
	
	public const String ERROR_0007 = "0007";
	public const String ERROR_0007_MSG = "签约失败";
	
	public const String ERROR_0008 = "0008";
	public const String ERROR_0008_MSG = "已签约";
	
	public const String ERROR_0009 = "0009";
	public const String ERROR_0009_MSG = "签约审批中";
	
	public const String ERROR_0010 = "0010";
	public const String ERROR_0010_MSG = "已解约";
	
	public const String ERROR_0011 = "0011";
	public const String ERROR_0011_MSG = "解密业务参数失败";
	
	public const String ERROR_0012 = "0012";
	public const String ERROR_0012_MSG = "加密返回数据失败";
	
	public const String ERROR_0013 = "0013";
	public const String ERROR_0013_MSG = "加密3DES密钥失败";

	public const String ERROR_0014 = "0014";
	public const String ERROR_0014_MSG = "获取商户信息失败";

	public const String ERROR_0015 = "0015";
	public const String ERROR_0015_MSG = "验证MAC失败";
	
	public const String ERROR_0016 = "0016";
	public const String ERROR_0016_MSG = "解密MAC失败";
	
	public const String ERROR_0017 = "0017";
	public const String ERROR_0017_MSG = "解密商户对称密钥失败";

	public const String ERROR_0018 = "0018";
	public const String ERROR_0018_MSG = "商户已停用或未配置快捷支付银行";
	
	public const String ERROR_0019 = "0019";
	public const String ERROR_0019_MSG = "订单金额不合法";

	public const String ERROR_0020 = "0020";
	public const String ERROR_0020_MSG = "url字节数不能超过256";
	
	public const String ERROR_0021 = "0021";
	public const String ERROR_0021_MSG = "url编码转换出现异常";

	public const String ERROR_0022 = "0022";
	public const String ERROR_0022_MSG = "url地址不合法";
	
	public const String ERROR_0023 = "0023";
	public const String ERROR_0023_MSG = "商户订单已支付成功";

	public const String ERROR_0024 = "0024";
	public const String ERROR_0024_MSG = "订单金额不能为空，必须是大于0.00浮点数DECIMAL(10,2)";

	public const String ERROR_0025 = "0025";
	public const String ERROR_0025_MSG = "订单金额小于0";

	public const String ERROR_0026 = "0026";
	public const String ERROR_0026_MSG = "主收款方应该收金额必须是浮点数DECIMAL(10,2)";

	public const String ERROR_0027 = "0027";
	public const String ERROR_0027_MSG = "主收款方应该收金额小于等于0";

	public const String ERROR_0028 = "0028";
	public const String ERROR_0028_MSG = "主收款方应收金额不应该大于订单金额";

	public const String ERROR_0029 = "0029";
	public const String ERROR_0029_MSG = "主收款方应收金额为空 ";
	
	public const String ERROR_0030 = "0030";
	public const String ERROR_0030_MSG = "订单时间格式有误 ";
	
	public const String ERROR_0031 = "0031";
	public const String ERROR_0031_MSG = "订单概要字节数不能超过256字节";
	
	public const String ERROR_0032 = "0032";
	public const String ERROR_0032_MSG = "订单概要编码转换出现异常";
	
	public const String ERROR_0033 = "0033";
	public const String ERROR_0033_MSG = "扩展字段不能大于512字节";
	
	public const String ERROR_0034 = "0034";
	public const String ERROR_0034_MSG = "扩展字段编码转换出现异常";
	
	public const String ERROR_0035 = "0035";
	public const String ERROR_0035_MSG = "手机号格式错误";
	
	public const String ERROR_0036 = "0036";
	public const String ERROR_0036_MSG = "用户未签约";
	
	public const String ERROR_0037 = "0037";
	public const String ERROR_0037_MSG = "系统异常";
	
	public const String ERROR_0038 = "0038";
	public const String ERROR_0038_MSG = "订单查询失败！";
	
	public const String ERROR_0039 = "0039";
	public const String ERROR_0039_MSG = "订单不存在！";
	
	public const String ERROR_0040 = "0040";
	public const String ERROR_0040_MSG = "查询到多笔记录！";
	
	public const String ERROR_0041 = "0041";
	public const String ERROR_0041_MSG = "商户未配置手续费规则";
	
	public const String ERROR_0042 = "0042";
	public const String ERROR_0042_MSG = "订单手续费填写失败";
	
	public const String ERROR_0043 = "0043";
	public const String ERROR_0043_MSG = "该商户没有配置快捷支付银行";

	public const String ERROR_0044 = "0044";
	public const String ERROR_0044_MSG = "订单信息入库失败";
	
	public const String ERROR_0045 = "0045";
	public const String ERROR_0045_MSG = "订单交易记录入库失败";

	public const String ERROR_0046 = "0046";
	public const String ERROR_0046_MSG = "此订单已失效";
	
	public const String ERROR_0047 = "0047";
	public const String ERROR_0047_MSG = "订单记录修改失败";

	public const String ERROR_0048 = "0048";
	public const String ERROR_0048_MSG = "未查询到商户可用汇率";
	
	public const String ERROR_0049 = "0049";
	public const String ERROR_0049_MSG = "订单单次金额超限";
	
	public const String ERROR_0050 = "0050";
	public const String ERROR_0050_MSG = "订单月累计金额超限";
	
	public const String ERROR_0051 = "0051";
	public const String ERROR_0051_MSG = "下单：获取支付短信验证码失败";
	
	public const String ERROR_0052 = "0052";
	public const String ERROR_0052_MSG = "获取签约短信验证码失败";
	
	public const String ERROR_0053 = "0053";
	public const String ERROR_0053_MSG = "证件号长度错误";
	
	public const String ERROR_0054 = "0054";
	public const String ERROR_0054_MSG = "卡号长度错误";
	
	public const String ERROR_0055 = "0055";
	public const String ERROR_0055_MSG = "未查询到商户支持的外币币种";
	
	public const String ERROR_0056 = "0056";
	public const String ERROR_0056_MSG = "该商户不支持此币种";
	
	public const String ERROR_0057 = "0057";
	public const String ERROR_0057_MSG = "平台暂时不支持此币种";
	
	public const String ERROR_0058 = "0058";
	public const String ERROR_0058_MSG = "获取商户对账请求处理异常！";
	
	public const String ERROR_0059 = "0059";

	public const String ERROR_0060 = "0060";
	public const String ERROR_0060_MSG = "未查询到商户用户信息，请检查是否已签约";

	public const String ERROR_0061 = "0061";
	public const String ERROR_0061_MSG = "用户在关注名单中";
	
	//public static Map<String, Map<String, String>> mapConstant = new HashMap<String, Map<String, String>>();
	
	public const String ERROR_0062 = "0062";
	public const String ERROR_0062_MSG = "商户为进行注册";
	public const String ERROR_0063 = "0063";
	public const String ERROR_0063_MSG = "商户注册失败";

	public const String ERROR_0064 = "0064";
	public const String ERROR_0064_MSG = "未查询到业务范围信息";
	
	public const String ERROR_0065 = "0065";
	public const String ERROR_0065_MSG = "未查询到协议号信息";
    }
}