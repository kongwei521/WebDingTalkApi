using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    /// <summary>  
    /// SDK使用的URL  
    /// </summary>  
    public sealed class Urls
    {
        #region 发送群消息  需要的chatid参数可以通过dd.biz.chat.chooseConversationByCorpId获取 
        //https://open-doc.dingtalk.com/docs/doc.htm?spm=a219a.7386797.0.0.iOFz8a&source=search&treeId=374&articleId=104977&docType=1
        /// <summary>  
        /// 创建会话  
        /// </summary>  
        public const string chat_create = "https://oapi.dingtalk.com/chat/create";
        /// <summary>  
        /// 获取会话信息  
        /// </summary>  
        public const string chat_get = "https://oapi.dingtalk.com/chat/get";
        /// <summary>  
        /// 发送消息到群会话 消息  
        /// </summary>  
        public const string chat_send = "https://oapi.dingtalk.com/chat/send";
        /// <summary>  
        /// 更新会话信息  
        /// </summary>  
        public const string chat_update = "https://oapi.dingtalk.com/chat/update";
        #endregion
        /// <summary>  
        /// 获取部门列表  
        /// </summary>  
        public const string department_list = "https://oapi.dingtalk.com/department/list";

        /// <summary>  
        /// 获取访问票记  
        /// </summary>  
        public const string gettoken = "https://oapi.dingtalk.com/gettoken";

        /// <summary>  
        /// 发送 群组消息  //企业通知消息接口
        /// </summary>  
        public const string group_message_send = "https://oapi.dingtalk.com/message/send";

        /// <summary>
        /// 普通消息发送  需要的cid和群消息的chatid不一样，(通过JSAPI之pickConversation接口唤起联系人界面选择之后即可拿到会话cid，之后您可以使用获取到的cid调用此接口）
        /// </summary>
        public const string conversation_message_send = "https://oapi.dingtalk.com/message/send_to_conversation";

        /// <summary>
        /// 企业通知消息：可以主动发消息给员工，异步发送
        /// </summary>
        public const string corpconversation_asyncsend = "https://eco.taobao.com/router/rest";
        /// <summary>  
        /// 用户列表  
        /// </summary>  
        public const string user_list = "https://oapi.dingtalk.com/user/list";
        /// <summary>  
        /// 用户详情  
        /// </summary>  
        public const string user_get = "https://oapi.dingtalk.com/user/get";

        /// <summary>  
        /// 获取JSAPI的票据  
        /// </summary>  
        public const string get_jsapi_ticket = "https://oapi.dingtalk.com/get_jsapi_ticket";
        //更多地址:
        //https://open-doc.dingtalk.com/docs/doc.htm?spm=a219a.7386797.0.0.2oOQSJ&source=search&treeId=366&articleId=107549&docType=1
    }
}