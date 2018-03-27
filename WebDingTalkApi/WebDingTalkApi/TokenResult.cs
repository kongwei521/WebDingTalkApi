using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDingTalkApi
{
    public class TokenResult : ResultPackage
    {
        public string Access_token { get; set; }
    }
}