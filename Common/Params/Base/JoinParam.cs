namespace Common.Params.Base
{
    public class JoinParam
    {
        public string primaryColumn { get; set; }
        public string foreignColumn { get; set; }
        public string joinTable { get; set; }
        public string joinType { get; set; } = "LEFT JOIN";
        public string joinFromTable { get; set; }
    }
}
