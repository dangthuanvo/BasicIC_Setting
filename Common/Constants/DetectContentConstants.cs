namespace Common
{
    public static class CorrectLevel
    {
        public const string High = "high";
        public const string Medium = "medium";
        public const string Low = "low";
        public const string Custom = "custom";
    }
    public static class DetectContentActionType
    {
        public const string AddVipCustomer = "add_vip_customer";
        public const string BlockCustomer = "block_customer";
        public const string AddTag = "add_tag";
        public const string AddAgent = "add_agent";
        public const string AddAgentGroup = "add_agent_group";
        public const string AddNotification = "add_notification";
    }
    public static class DetectContentNotificationType
    {
        public const string AgentOwner = "agent_owner";
        public const string AgentOwnerAndSupervisor = "agent_owner_and_supervisor";
        public const string AgentInConversationAndSupervisor = "agent_in_conversation_and_supervisor";
        public const string Supervisor = "supervisor";
    }
    public static class AddTagTo
    {
        public const string Message = "message";
        public const string Interaction = "interaction";
        public const string Both = "both";
    }
}