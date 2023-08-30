using System.ComponentModel.DataAnnotations;

namespace Common.Params.Base
{
    public class PagingParamGlobalSearch : BaseParam
    {
        public int page { get; set; }
        public int limit { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string value { get; set; } = "";
    }
}
