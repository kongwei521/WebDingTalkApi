钉钉配置及进行调用TopSDK.dll 进行异步发送消息功能。 
本文首先感谢原作者_学而时习之_博客地址是：https://blog.csdn.net/xxdddail/article/category/6776456 主要参考并整理完成此源码开源。 

config配置：
 <appSettings>
     <!--应用ID的名义发送消息-->
    <add key="AgentID" value="126463348"/>
    <!--消息接收者的部门id-->
    <add key="DeptID" value="254203329"/>
    <!--钉钉中公司ID及密匙-->
    <add key="CorpID" value="dingbxxxxxb3c"/>
    <add key="CorpSecret" value="cxxxxxxxxxxxxxx1IQfF3JKBcCi"/>
  </appSettings>
  
  发送消息方法:
  
    #region 钉钉发送消息通知
        /// <summary>  
        /// 企业通知消息接口发送消息  https://open-doc.dingtalk.com/docs/doc.htm?spm=a219a.7629140.0.0.dWrFg3&treeId=172&articleId=104973&docType=1  可用此方法
        /// </summary>  
        /// <param name="toUser">目标用户</param>  
        /// <param name="toParty">目标部门.当toParty和toUser同时指定时，以toParty来发送。</param>  
        /// <param name="content">消息文本</param>  
        /// <returns></returns>  
        //public static SendMessageResult GroupSendTextMsg(string toUser, string toParty, string content)
        //{
        //    var txtmsg = new
        //    {
        //        touser = toUser,//员工id列表（消息接收者，多个接收者用|分隔）
        //        toparty = toParty,//部门id列表
        //        msgtype = MsgType.text.ToString(),
        //        agentid = ConfigHelper.FetchAgentID(),//应用的标识
        //        text = new
        //        {
        //            content = content
        //        }
        //    };
        //    string apiurl = ApiTool.FormatApiUrlWithToken(Urls.group_message_send);
        //    string json = JsonConvert.SerializeObject(txtmsg);
        //    var result = Analyze.Post<SendMessageResult>(apiurl, json);
        //    return result;
        //}

        /// <summary>
        /// 发送到部门群组\单个人
        /// </summary>
        /// <param name="toUser"></param>
        /// <param name="content"></param>
        /// <param name="forceUpdate"></param>
        /// <returns></returns>
        public static void AsyncSendDingTalkTextMessage(string toUser, string msgContent,string trueName, bool forceUpdate = false)
        {
            log4net.ILog log = log4net.LogManager.GetLogger("发送钉钉text消息格式");
            ApiTool.EndUpdateAccessToken(forceUpdate);
            IDingTalkClient client = new DefaultDingTalkClient(Urls.corpconversation_asyncsend);
            var txtmsg = new CorpMessageCorpconversationAsyncsendRequest()
            {
                Msgtype = MsgType.text.ToString(),//消息类型
                AgentId = Convert.ToInt64(ConfigHelper.FetchAgentID()),// 	微应用的id
                UseridList = toUser,//接收者的用户userid列表 
                Msgcontent = "{ \"content\": \""+msgContent+"\" } "
            };

              CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(txtmsg, AccessToken.Value);
            var result = string.Empty;// rsp.Body;
            log.Info(new LogContent(ShareApplyClass.GetIPAddress(),HttpContext.Current.Server.UrlDecode(trueName) , "发送钉钉text消息格式", msgContent + "信息如下。" + result));
        }

        /// <summary>
        ///  通过判断个人是否有钉钉审批进行发送钉钉消息
        /// </summary>
        /// <param name="insertType"></param>
        /// <param name="approveID"></param>
        /// <param name="dingTalkID">下一步审批人的钉钉ID</param>
        /// <param name="trueName">下一步审批人的真实姓名</param>
        /// authorName 提交人
        public static void AsyncSendDingTalkOAMessage(string insertType, string approveID, string dingTalkID, string trueName, string authorName, bool forceUpdate = false)
        {
            log4net.ILog log;
            ApiTool.EndUpdateAccessToken(forceUpdate);
            IDingTalkClient client = new DefaultDingTalkClient(Urls.corpconversation_asyncsend);
            using (WHSMDataContext db = new WHSMDataContext())
            {
                var getApproveInfo = db.ApprovePlanOrder_Header.SingleOrDefault(x => x.approveID.Equals(approveID));
                if (getApproveInfo != null)
                {
                    var approveTitle = getApproveInfo.approveTitle;
                    log = log4net.LogManager.GetLogger(approveTitle);
                    //
                    var workName = WorkFlowModule.ReturnTypeName(insertType);
                    //存放是采购、领用等
                    var typeName = string.Empty;
                    var form = new List<OaBodyFormJsonList>() { };
                    switch (insertType)
                    {
                        case "CG":
                            form = new List<OaBodyFormJsonList>()
                            {
                                new OaBodyFormJsonList() {key="流水号：",value=getApproveInfo.approveNo },
                                new OaBodyFormJsonList() {key="申请人：",value=getApproveInfo.approvePlanPeople },
                                new OaBodyFormJsonList() {key = "申请部门：", value = db.Base_Department.SingleOrDefault(x=>x.sortId==getApproveInfo.approveDept).sortName},
                                new OaBodyFormJsonList() {key="申请时间：",value=getApproveInfo.approveDate.ToString() },
                                new OaBodyFormJsonList() {key="入库仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveWh).whname },
                                new OaBodyFormJsonList() {key="备注说明：",value=getApproveInfo.approveNote }
                            };
                            typeName = "采购";
                            break;
                        case "FS":
                            form = new List<OaBodyFormJsonList>()
                            {
                                new OaBodyFormJsonList() {key="流水号：",value=getApproveInfo.approveNo },
                                new OaBodyFormJsonList() {key="申请人：",value=getApproveInfo.approvePlanPeople },
                                new OaBodyFormJsonList() {key = "申请部门：", value = db.Base_Department.SingleOrDefault(x=>x.sortId==getApproveInfo.approveDept).sortName},
                                new OaBodyFormJsonList() {key="申请时间：",value=getApproveInfo.approveDate.ToString() },
                                new OaBodyFormJsonList() {key="入库仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveWh).whname },
                                new OaBodyFormJsonList() {key="归属客户：",value=getApproveInfo.theCustomer},
                                new OaBodyFormJsonList() {key="备注说明：",value=getApproveInfo.approveNote }
                            };
                            typeName = "采购报销";
                            break;
                        case "FLY":
                            form = new List<OaBodyFormJsonList>()
                            {
                                new OaBodyFormJsonList() {key="流水号：",value=getApproveInfo.approveNo },
                                new OaBodyFormJsonList() {key="申请人：",value=getApproveInfo.approvePlanPeople },
                                new OaBodyFormJsonList() {key = "申请部门：", value = db.Base_Department.SingleOrDefault(x=>x.sortId==getApproveInfo.approveDept).sortName},
                                new OaBodyFormJsonList() {key="申请时间：",value=getApproveInfo.approveDate.ToString() },
                                new OaBodyFormJsonList() {key="领用仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveWh).whname },
                                new OaBodyFormJsonList() {key="归属客户：",value=getApproveInfo.theCustomer},
                                new OaBodyFormJsonList() {key="备注说明：",value=getApproveInfo.approveNote }
                            };
                            typeName = "领用";
                            break;
                        case "DB":
                            form = new List<OaBodyFormJsonList>()
                            {
                                new OaBodyFormJsonList() {key="流水号：",value=getApproveInfo.approveNo },
                                new OaBodyFormJsonList() {key="申请人：",value=getApproveInfo.approvePlanPeople },
                                new OaBodyFormJsonList() {key = "申请部门：", value = db.Base_Department.SingleOrDefault(x=>x.sortId==getApproveInfo.approveDept).sortName},
                                new OaBodyFormJsonList() {key="申请时间：",value=getApproveInfo.approveDate.ToString() },
                                new OaBodyFormJsonList() {key="调出仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveWh).whname },
                                new OaBodyFormJsonList() {key="调入仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveInWh).whname},
                                new OaBodyFormJsonList() {key="备注说明：",value=getApproveInfo.approveNote }
                            };
                            typeName = "调拨";
                            break;
                        case "CGC":
                            form = new List<OaBodyFormJsonList>()
                            {
                                new OaBodyFormJsonList() {key="流水号：",value=getApproveInfo.approveNo },
                                new OaBodyFormJsonList() {key="申请人：",value=getApproveInfo.approvePlanPeople },
                                new OaBodyFormJsonList() {key = "申请部门：", value = db.Base_Department.SingleOrDefault(x=>x.sortId==getApproveInfo.approveDept).sortName},
                                new OaBodyFormJsonList() {key="申请时间：",value=getApproveInfo.approveDate.ToString() },
                                new OaBodyFormJsonList() {key="入库仓库：",value=db.Base_WareHouse.SingleOrDefault(x=>x.whcode==getApproveInfo.approveWh).whname },
                                new OaBodyFormJsonList() {key="归属客户：",value=getApproveInfo.theCustomer},
                                new OaBodyFormJsonList() {key="备注说明：",value=getApproveInfo.approveNote }
                            };
                            typeName = "纸箱采购";
                            break;
                    }
                    List<ApprovePlanOrder_Details> lstApproveDetails = new List<ApprovePlanOrder_Details>();
                  var getApproveDetails = db.ApprovePlanOrder_Details.Where(x => x.approveID.Equals(approveID)).ToList();
                    if (getApproveDetails.Count > 0)
                    {
                        lstApproveDetails = getApproveDetails;
                    }
                    else
                    {
//                        SELECT a.*from ApprovePlanOrder_Details a INNER JOIN(
//SELECT b.approveID from ApprovePlanOrder_Header a INNER JOIN(SELECT approveID, CgCostsID from ApprovePlanOrder_Header)
//b on a.approveID = b.CgCostsID where a.approveID = '77634e39-25aa-4cec-8688-5f17c41fb56d')b on a.approveID = b.approveID
                        lstApproveDetails  = (from a in db.ApprovePlanOrder_Details
                                              join b in (
                                                  (from a0 in db.ApprovePlanOrder_Header
                                                   join b in (
                                                     (from approveplanorder_header in db.ApprovePlanOrder_Header
                                                       select new
                                                       {
                                                           approveplanorder_header.approveID,
                                                           approveplanorder_header.CgCostsID
                                                       })) on new { approveID = a0.approveID } equals new { approveID = b.CgCostsID }
                                                   where  a0.approveID.Equals(approveID)
                                                   select new  { b.approveID  })) on a.approveID equals b.approveID
                                              select a
                                               ).ToList();
                    }
                    if (lstApproveDetails.Count > 0)
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.Append("所属流程：" + workName + "\n");
                        sb.Append(typeName + "商品总数：" + lstApproveDetails.Sum(x => x.goodsBuyNo) + " 件(个)\n");
                        sb.Append("上一步办理人给您的提示：" + db.Approve_NextStepInfo.Single(x =>
                        x.Instance_ID.Equals(approveID) && x.Reg_Code == getApproveInfo.approvePeople).App_InnerIdea);
                        var url = System.Configuration.ConfigurationManager.AppSettings["HttpUrl"]+"DingTalkApproval.aspx?insertType=" + insertType + "&approveID=" + approveID + "&DingTalkID=" + dingTalkID + "&TrueName=" + trueName + "";
                        oa newOA = new oa()
                        {
                            message_url = url,
                            pc_message_url = "dingtalk:dingtalkclient/page/link?url=" + HttpContext.Current.Server.UrlEncode(url) + "&pc_slide=true",
                            head = new OaHeadJsonList() { bgcolor = "FFBBBBBB", text = approveTitle },
                            body = new OaBodyJsonList()
                            {
                                title = "流程标题:[" + approveTitle + "]正等待您的审批",
                                form = form,
                                rich = new OaRichJsonList() { num = lstApproveDetails.Sum(x => x.goodsBuyNo * x.goodsPrice).ToString(), unit = "元" },
                                file_count = string.IsNullOrWhiteSpace(getApproveInfo.approveFilePath) == true ? "" : "1",
                                content = sb.ToString(),
                                author = string.IsNullOrWhiteSpace(authorName) == true ?trueName : authorName,
                                //db.Base_SuperAdmin.SingleOrDefault(x => x.dingtalkid == getApproveInfo.approvePeople).truename
                            },
                        };
                        var jsonContent = JsonConvert.SerializeObject(newOA);
                        var txtmsg = new CorpMessageCorpconversationAsyncsendRequest()
                        {
                            Msgtype = MsgType.OA.ToString(),//消息类型
                            AgentId = Convert.ToInt64(ConfigHelper.FetchAgentID()),// 	微应用的id
                            UseridList = dingTalkID,//接收者的用户userid列表 
                            Msgcontent = jsonContent
                        };
                        CorpMessageCorpconversationAsyncsendResponse rsp = client.Execute(txtmsg, AccessToken.Value);
                        var result = string.Empty;// rsp.Body;

                        log.Info(new LogContent(ShareApplyClass.GetIPAddress(), trueName,
      "发送钉钉信息", "发送流程编号为:" + approveID + "的标题[" + approveTitle + "]信息如下。" + result));
                    }
                    else
                    {
                        log.Fatal(new LogContent(ShareApplyClass.GetIPAddress(), trueName,
      "发送钉钉信息", "发送流程编号为:" + approveID + "的标题[" + approveTitle + "]没有明细信息"));
                    }
               
                }
            }
        }
        /// <summary>
        /// 获取是否有钉钉审批权限
        /// </summary>
        /// <param name="dingTalkID"></param>
        /// <returns></returns>
        public static char? GetIfDingtalkAgreeStatus(string dingTalkID)
        {
            char? result='N';
            using (WHSMDataContext db = new WHSMDataContext())
            {
                var getApproveInfo = db.Base_SuperAdmin.SingleOrDefault(x => x.dingtalkid.Equals(dingTalkID) && x.ifdisable == 'N');
                if (getApproveInfo != null)
                {
                    result = getApproveInfo.ifdingtalkagree;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取下一步或者退回人的姓名
        /// </summary>
        /// <param name="dingTalkID"></param>
        /// <returns></returns>
        public static string GetBackNextApproveTureName(string dingTalkID)
        {
           var  result =string.Empty;
            using (WHSMDataContext db = new WHSMDataContext())
            {
                var getApproveInfo = db.Base_SuperAdmin.SingleOrDefault(x => x.dingtalkid.Equals(dingTalkID) && x.ifdisable == 'N');
                if (getApproveInfo != null)
                {
                    result = getApproveInfo.truename;
                }
            }
            return result;
        }
        #region //OA 发送消息内容类
        public class oa
        {
            public string message_url { get; set; }
            public string pc_message_url { get; set; }
            public OaHeadJsonList head { get; set; }
            public OaBodyJsonList body { get; set; }

        }
        public class OaHeadJsonList
        {
            public string bgcolor { get; set; }
            //消息的头部标题（向普通会话发送时有效，向企业会话发送时会被替换为微应用的名字）
            public string text { get; set; }
        }
        public class OaBodyJsonList
        {
            public string title { get; set; }
            public List<OaBodyFormJsonList> form { get; set; }
            public OaRichJsonList rich { get; set; }
            public string content { get; set; }
            public string image { get; set; }
            public string file_count { get; set; }
            public string author { get; set; }
        }
        public class OaBodyFormJsonList
        {
            public string key { get; set; }
            public string value { get; set; }
        }
        public class OaRichJsonList
        {
            public string num { get; set; }
            public string unit { get; set; }
        }
        #endregion

        #endregion
        
  调用示例：
   var sendMessageAlert = "您的" + typeName + ":【" + approveTitle + "】已被【" + trueName + "】审批通过。";
   ShareApplyClass.AsyncSendDingTalkTextMessage(regDingTalkID,  sendMessageAlert,trueName);
   
   var handlerType = ((Button)sender).CommandArgument == "5" ? "已成功退回给【" + drpBackApprovePeople.SelectedItem.Text + "】,等待重新处理后再次审批。" : "正在等待【" + drpApprovePeople.SelectedItem.Text + "】审批,如紧急或等待时间过长,您可点击催办进行催促当前办理人进行审批。";
                        var sendMessageAlert = "【" + txtApproveTtile.Value + "】" + handlerType;
                        var agreeCode = ((Button)sender).CommandArgument == "5" ? drpBackApprovePeople.SelectedValue : drpApprovePeople.SelectedValue;
ShareApplyClass.AsyncSendDingTalkOAMessage(Request["insertType"], Request["ApproveID"], agreeCode, ShareApplyClass.GetBackNextApproveTureName(agreeCode), Request["TrueName"]);
              
