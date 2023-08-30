namespace Common.Params.Base
{
    public class CustomQueryString
    {
        public string queryString { get; set; }
        // name of field in Query class
        // Ex: select, conditions, innerSelect, from,...
        public string fieldName { get; set; }
    }
}
