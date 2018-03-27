using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    /// <summary>
    /// 错误代码标识
    /// </summary>
    public enum ErrCodeEnum
    {
        OK = 0,

        VoildAccessToken = 40014,

        /// <summary>  
        /// 未知  
        /// </summary>  
        Unknown = int.MaxValue
    }
}