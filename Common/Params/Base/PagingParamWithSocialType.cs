using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Common.Params.Base
{
    public class PagingParamWithSocialType : BaseParam
    {
        [Required]
        public string channel_type;
        public int page { get; set; }
        public int limit { get; set; }
        public List<SortParam> sorts { get; set; }
        public string search_param { get; set; }
    }
}