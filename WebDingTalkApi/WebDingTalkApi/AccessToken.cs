using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    /// <summary>  
    /// 访问票据  
    /// </summary>  
    public class AccessToken
    {
        /// <summary>  
        /// 票据的值  
        /// </summary>  
        public static String Value { get; set; }

        /// <summary>  
        /// 票据的开始时间  
        /// </summary>  
        public static DateTime Begin { get; set; } = DateTime.Parse("1970-01-01");
    }
}