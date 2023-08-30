using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Common
{
    public class Constants
    {
        public static readonly string CONF_CROSS_ORIGIN = "CROSS_ORIGIN";
        public static readonly string CONF_HOST_MONGO_DB = "HOST_MONGO_DB";
        public static readonly string CONF_KAFKA_BOOSTRAP_SERVER = "KAFKA_BOOSTRAP_SERVER";
        public static readonly string CONF_KAFKA_GROUP_ID = "KAFKA_GROUP_ID";
        public static readonly string CONF_KAFKA_SESSION_TIME_OUT = "KAFKA_SESSION_TIME_OUT";
        public static readonly string CONF_ADDRESS_DISCOVERY_SERVICE = "ADDRESS_DISCOVERY_SERVICE";
        public static readonly string CONF_ADDRESS_SERVICE = "ADDRESS_SERVICE";
        public static readonly string CONF_PORT_SERVICE = "PORT_SERVICE";
        public static readonly string CONF_PROTOCOL_SERVICE = "PROTOCOL_SERVICE";
        public static readonly string CONF_HEALTH_CHECK_SERVICE = "HEALTH_CHECK_SERVICE";
        public static readonly string CONF_SOURCE_FABIO_SERVICE = "SOURCE_FABIO_SERVICE";// for fabio
        public static readonly string CONF_HOST_FABIO_SERVICE = "HOST_FABIO_SERVICE";
        public static readonly string CONF_MAX_ERROR_MESS = "MAX_ERROR_MESS";
        public static readonly string CONF_STATE_SOURCE = "STATE_SOURCE";

        public static readonly string SALT_KEY = "BASEBS";
        public static readonly string KEY_SESSION_TENANT_ID = "KEY_SESSION_TENANT_ID";
        public static readonly string KEY_SESSION_EMAIL = "KEY_SESSION_EMAIL";
        public static readonly string KEY_SESSION_TOKEN = "KEY_SESSION_TOKEN";
        public static readonly string KEY_SESSION_IS_ADMIN = "KEY_SESSION_IS_ADMIN";
        public static readonly string KEY_SESSION_IS_ROOT = "KEY_SESSION_IS_ROOT";
        public static readonly string KEY_SESSION_IS_SUPERVISOR = "KEY_SESSION_IS_SUPERVISOR";
        public static readonly string KEY_SESSION_IS_AGENT = "KEY_SESSION_IS_AGENT";
        public static readonly string KEY_SESSION_CUSTOMER_TYPE = "KEY_SESSION_CUSTOMER_TYPE";
        public static readonly string KEY_SESSION_IS_THIRD_PARTY = "KEY_SESSION_IS_THIRD_PARTY";
        public static readonly string KEY_SESSION_IS_SECRET_KEY = "KEY_SESSION_IS_SECRET_KEY";


        public static readonly string FORMAT_DATETIME_ENTITY = "yyyy-MM-dd HH:mm:ss";

        public static readonly string FIELD_ID = "_id";
        public static readonly int HOUR_START_CALL = 7;

        public static readonly string CORE_SECRET_KEY_LCM = "bpZC9cqp54RqycnMQMUMunjnZ9uph/qY";
        public enum OPERATOR_FILTER { EQ, CMP, GT, GTE, LT, LTE, NE, EXIST, AVG, FIRST, LAST, MAX, MIN, SUM }
        public enum TYPE_DATA_CAMPARE { STRING, DATE, DATE_TIME, INT, FLOAT, BOOL }

        public static readonly string[] PROPERTY_HIDE_LCM = { "tenant_id", "create_by", "last_modify_time", "last_modify_by" };

        public static readonly string[] PROPERTY_HIDE_CRM = { "tenant_id", "create_by", "last_modify_time", "last_modify_by", "Created_By", "Created_Date", "Modify_By", "Modify_Time" };
        public static readonly string[] PROHIBITED_EXTENSIONS = { ".dll", ".exe", ".lnk", ".swf", ".sys", ".jar", ".gzquar", ".zix",
                                                                    ".scr", ".js", ".vbs", ".bat", ".ws", ".ocx", ".com", ".bin",
                                                                    ".class", ".aru", ".ozd", ".drv", ".wmf", ".shs", ".chm", ".pgm",
                                                                    ".pif", ".dev", ".xlm", ".vbe", ".xnxx", ".vba", ".boo",
                                                                    ".0_full_0_tgod_signed", ".vxd", ".tps", ".pcx", ".tsa", ".sop",
                                                                    ".386", ".hlp", ".vb", ".bkd", ".rhk", ".exe1", ".exe_renamed",
                                                                    ".lik", ".vbx", ".osa", ".mjz", ".cih", ".dyz", ".dlb", ".wsc",
                                                                    ".mfu", ".dom", ".mjg", ".dxz", ".kcd", ".dyv", ".php3", ".hlw",
                                                                    ".s7p", ".9", ".cla", ".bup", ".rsc_tmp", ".upa", ".bhx", ".mcq",
                                                                    ".txs", ".dli", ".scr", ".wsh", ".bxz", ".xlv", ".xir", ".fnr",
                                                                    ".xdu", ".cxq", ".wlpginstall", ".ska", ".cfxxe", ".xtbl", ".qrn",
                                                                    ".vexe", ".tti", ".spam", ".dllx", ".smtmp", ".fag", ".ceo",
                                                                    ".tko", ".uzy", ".oar", ".bll", ".plc", ".dbd", ".ssy", ".smm",
                                                                    ".zvz", ".blf", ".ce0", ".cc", ".ctbl", ".iws", ".vzr", ".nls",
                                                                    ".hsq", ".lkh", ".rna", ".ezt", ".aepl", ".hts", ".let", ".delf",
                                                                    ".aut", ".buk", ".atm", ".fuj", ".fjl", ".bmw", ".cyw", ".crypt1",
                                                                    ".capxml", ".iva", ".dx", ".pid", ".bps", ".bqf", ".qit", ".pr",
                                                                    ".lpaq5", ".xnt", ".lok",
                                                                    ".asp", ".aspx", ".config", ".ashx",
                                                                    ".asmx", ".aspq", ".axd", ".cshtm",
                                                                    ".cshtml", ".rem", ".soap", ".vbhtm",
                                                                    ".vbhtml", ".asa", ".cer", ".shtml"};

        public static readonly List<string> ACCEPT_FILE_EXTENTION = new List<string>() { ".xls", ".xlsx" };

        public static readonly string[] PUBLIC_COMMON_SETTINGS = { "use_tracking", "default_after_feedback_message" };
        public static readonly string STATUS_CALL_SUCCESS = "0";

        public static readonly string ID_DROP_LIST_CONFIG_ITEM_DAIKIN = "daikin_droplist_teamsale";
        public static readonly string ID_CAMPAIGN_CONFIG_ITEM_DAIKIN = "daikin_campaign_teamsale";
        public static readonly string NAME_AGENT_GROUP_ITEM_DAIKIN = "daikin_agent_group_teamsale";

        public static readonly string GROUP_YEAR = "year";
        public static readonly string GROUP_MONTH = "month";
        public static readonly string GROUP_DAY = "day";
        public static readonly string GROUP_HOUR = "hour";
        public static readonly string GROUP_MINUTE = "minute";

        public static readonly string PRE_BUSINESS_RESULT = "biz_result";
        public static readonly string PRE_BUSINESS_INFOR = "biz_infor";

        public enum CONSUMER_TYPE { SEND_MESS, EDIT_MESS };

        public static readonly string SERVICE_CONSULT_GOOD_HEALTH = "passing";
        public static readonly string STATE_SOURCE_PRODUCTION = "production";
        public static readonly string STATE_SOURCE_DEV = "dev";

        public static readonly int SERVICE_CODE = 3;

        //Config service
        public static readonly string SOURCE_FABIO_SETTINGS = "BasicIC_Setting";
        public static readonly string SOURCE_FABIO_USER = "user";
        public static readonly string SOURCE_FABIO_SERVICE_FIRST = "service-first";
        public static readonly string SOURCE_FABIO_INTERACTION_DATA_STORE = "idstore";

        public static readonly string ENCRYPTION_KEY = "83JUea#ivuZj#AK9@KyT295L@h^PP6qQ";

        public static ModuleBuilder MODULE_BUILDER = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("Dynamic Assembly"), AssemblyBuilderAccess.Run).DefineDynamicModule("MainModule");
        public static Dictionary<string, Type> CREATED_DYNAMIC_TYPE = new Dictionary<string, Type>();

        // Log types
        public static readonly string LOG_USER_CREATE = "Log User Create";
        public static readonly string LOG_USER_UPDATE = "Log User Update";
        public static readonly string LOG_USER_DELETE = "Log User Delete";

        public static readonly char[] SPECIAL_CHARACTERS = { '~', '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '_', '+', '=', '{', '}', '[', ']', ':', ';', '"', ',', '.', '/', '<', '>', '|', '|' };
        public static readonly string[] ACTION_NOT_APPLY_FOR_ZALO = { "video", "audio", "carousel", "quick_reply" };


        #region Sheet name
        public static readonly string CHATBOT_SHEET_NAME = "List";
        #endregion
        public static readonly string CONF_MAX_DEGREE_PARALLELISM = "MAX_DEGREE_PARALLELISM";
        // Redis hashtag
        public const string HASHTAG_CHATBOT_SESSION = "chatbot-session";
        public const string HASHTAG_CHATBOT_CONFIG = "chatbot-config";
        public const string HASHTAG_SESION = "session";
        public const string HASHTAG_PACKAGE_CONFIG = "package-config";
    }
}
