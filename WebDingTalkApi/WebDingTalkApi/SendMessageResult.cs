using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    public class SendMessageResult:ResultPackage
    {
        public string receiver { get; set; }
    }
}