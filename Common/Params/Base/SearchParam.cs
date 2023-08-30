using Common.CustomAttributes;
using System;

namespace Common.Params.Base
{
    public class SearchParam
    {
        public string name_field { get; set; }
        [OneOf(new String[] { "AND", "OR", "and", "or", null }, ErrorMessage = "wrong format")]
        public string conjunction { get; set; } = "AND";
        public Object value_search { get; set; }
        public Object upper_bound { get; set; }
        public string type { get; set; }
        public int is_open_parenthesis { get; set; } = 0;
        public int is_close_parenthesis { get; set; } = 0;
        public string logical_operator { get; set; } = "=";
    }
}
