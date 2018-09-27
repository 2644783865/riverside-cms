using System;
using System.Collections.Generic;
using System.Text;

namespace Riverside.Cms.Utilities.Net.Mail
{
    public class EmailContent
    {
        public string Subject { get; set; }
        public string HtmlBody { get; set; }
        public string PlainTextBody { get; set; }
    }
}
