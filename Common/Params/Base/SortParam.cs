using Common.CustomAttributes;
using System;
using System.Runtime.Serialization;

namespace Common.Params.Base
{
    public class SortParam
    {
        public string name_field { get; set; }
        private string typeSort { get; set; }
        [OneOf(new String[] { "ASC", "DESC", "asc", "desc" }, ErrorMessage = "type_sort has to be ASC or DESC")]
        public string type_sort
        {
            get
            {
                return typeSort;
            }
            set
            {
                typeSort = value;
                isAccessding = value == "ASC";
            }
        }
        [IgnoreDataMember]
        public bool isAccessding { get; set; }
    }
}
