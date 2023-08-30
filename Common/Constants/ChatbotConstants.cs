namespace Common
{
    public static class ActionType
    {
        public const string TEXT = "text";
        public const string QUESTION = "question";
        public const string IMAGE = "image";
        public const string VIDEO = "video";
        public const string AUDIO = "audio";
        public const string QUICK_REPLY = "quick_reply";
        public const string CAROUSEL = "carousel";
        public const string BUTTON_SELECT = "button_select";
        public const string DISTRIBUTION_AGENT = "distribution_agent";
        public const string REQUEST_API = "request_api";
        public const string COMPLETE_INTERACTION = "complete_interaction";
    }
    public static class ValidateType
    {
        public const string NONE = "none";
        public const string EMAIL = "email";
        public const string PHONE = "phone";
        public const string NUMBER = "number";
        public const string DATE = "date";
        public const string DATETIME = "datetime";
    }
    public static class ButtonDirectType
    {
        public const string GOTO_NODE = "goto_node";
        public const string OPEN_URL = "open_url";
        public const string PHONE_CALL = "phone_call";
    }
    public static class ZaloButtonAction
    {
        public const string GOTO_NODE = "oa.query.show";
        public const string OPEN_URL = "oa.open.url";
        public const string PHONE_CALL = "oa.open.phone";
    }
    public static class FacebookButtonAction
    {
        public const string GOTO_NODE = "postback";
        public const string OPEN_URL = "web_url";
        public const string PHONE_CALL = "phone_number";
    }
    public static class ViberButtonAction
    {
        public const string GOTO_NODE = "reply";
        public const string OPEN_URL = "open-url";
    }
    public static class DistributionType
    {
        public const string TO_DISTRIBUTION_RULE = "to_distribution_rule";
        public const string TO_AGENT_GROUP = "to_agent_group";
    }
    public static class MaximumButtonSelectButtons
    {
        public const int VIBER = 40;
        public const int ZALO = 5;
        public const int FACEBOOK = 3;
    }
    public static class MaximumQuickReplyButtons
    {
        public const int FACEBOOK = 13;
    }
    public static class MaximumCarouselItems
    {
        public const int VIBER = 6;
        public const int FACEBOOK = 10;
    }
    public static class MaximumCarouselButtons
    {
        public const int VIBER = 3;
        public const int FACEBOOK = 3;
    }
    public static class ChatbotRequestAuthenType
    {
        public const string BEARER_TOKEN = "bearer_token";
        public const string BASIC_AUTH = "basic_auth";
        public const string NO_AUTH = "no_auth";
    }
}