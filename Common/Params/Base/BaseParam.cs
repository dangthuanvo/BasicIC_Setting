using System;

namespace Common.Params.Base
{
    public class BaseParam
    {
        public string tenant_id { get; set; }
    }

    public class TopicParam
    {
        public Object data { get; set; }
        public string topic { get; set; }
    }
}
