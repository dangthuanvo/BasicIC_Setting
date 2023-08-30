using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class ElasticsearchApiPath
    {
        public static readonly string DELETE_CHATBOT_UNSATISFIED = "elasticsearch/chatbot-unsatisfied-delete";
        public static readonly string DELETE_CHATBOT_FAQ = "elasticsearch/chatbot-faq-delete";
        public static readonly string DELETE_SOCIAL_APPROVED = "elasticsearch/social-approved-delete";
        public static readonly string DELETE_DETECT_CONTENT = "elasticsearch/detect-content-delete";
    }
}
