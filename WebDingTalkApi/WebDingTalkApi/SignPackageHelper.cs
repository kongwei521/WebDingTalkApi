using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static WebDingTalkApi.ApiTool;

namespace WebDingTalkApi
{
    public class SignPackageHelper
    {
        #region FetchSignPackage Function   
        /// <summary>  
        /// 获取签名包  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static SignPackage FetchSignPackage(String url, JSTicket jsticket)
        {
            int unixTimestamp = SignPackageHelper.ConvertToUnixTimeStamp(DateTime.Now);
            string timestamp = Convert.ToString(unixTimestamp);
            string nonceStr = SignPackageHelper.CreateNonceStr();
            if (jsticket == null)
            {
                return null;
            }

            // 这里参数的顺序要按照 key 值 ASCII 码升序排序   
            string rawstring = $"{Keys.jsapi_ticket}=" + jsticket.ticket
                             + $"&{Keys.noncestr}=" + nonceStr
                             + $"&{Keys.timestamp}=" + timestamp
                             + $"&{Keys.url}=" + url;
            string signature = SignPackageHelper.Sha1Hex(rawstring).ToLower();
             
            var signPackage = new SignPackage()
            {
                agentId = ConfigHelper.FetchAgentID(),//取配置文件中的agentId，可依据实际配置而作调整  
                corpId = ConfigHelper.FetchCorpID(),//取配置文件中的coprId，可依据实际配置而作调整  
                timeStamp = timestamp,
                nonceStr = nonceStr,
                signature = signature,
                url = url,
                rawstring = rawstring,
                jsticket = jsticket.ticket,
                errcode = jsticket.ErrCode.ToString(),
                errmsg = jsticket.ErrMsg

            };
            return signPackage;
        }

        /// <summary>  
        /// 获取签名包  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static SignPackage FetchSignPackage(String url)
        {
            int unixTimestamp = SignPackageHelper.ConvertToUnixTimeStamp(DateTime.Now);
            string timestamp = Convert.ToString(unixTimestamp);
            string nonceStr = SignPackageHelper.CreateNonceStr();
            var jsticket = FetchJSTicket();
            var signPackage = FetchSignPackage(url, jsticket);
            return signPackage;
        }
        #endregion

        #region FetchJSTicket Function    
        /// <summary>  
        /// 获取JS票据  
        /// </summary>  
        /// <param name="url"></param>  
        /// <returns></returns>  
        public static JSTicket FetchJSTicket()
        {
            var cache = SimpleCacheProvider.GetInstance();
            var jsTicket = cache.GetCache<JSTicket>(ConstVars.CACHE_JS_TICKET_KEY);
            if (jsTicket == null || AccessToken.Begin.AddSeconds(ConstVars.CACHE_TIME) < DateTime.Now)//jsTicket为null表示不存在或过期,或AccessToken过期  
            {
                String apiurl = FormatApiUrlWithToken(Urls.get_jsapi_ticket);//该方法参看《钉钉开发系列(三)API的调用》  
                jsTicket = Analyze.Get<JSTicket>(apiurl);
                cache.SetCache(ConstVars.CACHE_JS_TICKET_KEY, jsTicket, ConstVars.CACHE_TIME - 500);//增加500的时间差以防与AccessToken错位过期  
            }
            return jsTicket;
        }
        #endregion

        #region Sha1Hex  
        public static string Sha1Hex(string value)
        {
           System.Security.Cryptography.SHA1 algorithm = System.Security.Cryptography.SHA1.Create();
            byte[] data = algorithm.ComputeHash(System.Text.Encoding.UTF8.GetBytes(value));
            string sh1 = "";
            for (int i = 0; i < data.Length; i++)
            {
                sh1 += data[i].ToString("x2").ToUpperInvariant();
            }
            return sh1;
        }
        #endregion

        #region CreateNonceStr  
        /// <summary>  
        /// 创建随机字符串  
        /// </summary>  
        /// <returns></returns>  
        public static string CreateNonceStr()
        {
            int length = 16;
            string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string str = "";
            Random rad = new Random();
            for (int i = 0; i < length; i++)
            {
                str += chars.Substring(rad.Next(0, chars.Length - 1), 1);
            }
            return str;
        }
        #endregion

        #region ConvertToUnixTimeStamp        
        /// <summary>    
        /// 将DateTime时间格式转换为Unix时间戳格式    
        /// </summary>    
        /// <param name="time">时间</param>    
        /// <returns>double</returns>    
        public static int ConvertToUnixTimeStamp(DateTime time)
        {
            int intResult = 0;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            intResult = Convert.ToInt32((time - startTime).TotalSeconds);
            return intResult;
        }
        #endregion
    }
    /// <summary>  
    /// 签名包  
    /// </summary>  
    public class SignPackage
    {
        public String agentId { get; set; }

        public String corpId { get; set; }

        public String timeStamp { get; set; }

        public String nonceStr { get; set; }

        public String signature { get; set; }

        public String url { get; set; }

        public String rawstring { get; set; }

        public string jsticket { get; set; }

        public string errcode { get; set; }

        public string errmsg { get; set; }
    }

}