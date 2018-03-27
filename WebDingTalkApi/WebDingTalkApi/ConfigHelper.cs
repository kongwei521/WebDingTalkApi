using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    public class ConfigHelper
    {
        #region FetchAgentID Function    
        /// <summary>  
        /// 获取agentid 
        /// </summary>  
        /// <returns></returns>  
        public static string FetchAgentID()
        {
            return FetchValue("AgentID");
        }
        #endregion

        #region DeptID Function    
        /// <summary>  
        /// 获取接收者的部门id 
        /// </summary>  
        /// <returns></returns>  
        public static string DeptID()
        {
            return FetchValue("DeptID");
        }
        #endregion

        #region FetchCorpID Function    
        /// <summary>  
        /// 获取CorpID  
        /// </summary>  
        /// <returns></returns>  
        public static string FetchCorpID()
        {
            return FetchValue("CorpID");
        }
        #endregion

        #region FetchCorpSecret Function  
        /// <summary>  
        /// 获取CorpSecret  
        /// </summary>  
        /// <returns></returns>  
        public static string FetchCorpSecret()
        {
            return FetchValue("CorpSecret");
        }
        #endregion

        #region FetchValue Function                
        private static string FetchValue(String key)
        {
           string value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                throw new Exception($"{key} is null.请确认配置文件中是否已配置.");
            }
            return value;
        }
        #endregion
    }
}