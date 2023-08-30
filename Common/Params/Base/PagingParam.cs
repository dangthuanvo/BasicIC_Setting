using System.Collections.Generic;
using System.Linq;

namespace Common.Params.Base
{
    public class PagingParam : BaseParam
    {
        public int page { get; set; }
        public int limit { get; set; }
        public List<SortParam> sorts { get; set; }
        public List<JoinParam> join_list { get; set; }
        public List<SearchParam> search_list { get; set; }
        public List<SearchParam> field_get_list { get; set; }
        public string[] table_search_list { get; set; }
        public List<CustomQueryString> custom_query_list { get; set; }
        public bool is_inner_join { get; set; } = false;

        public PagingParam()
        {
            sorts = new List<SortParam>();
            search_list = new List<SearchParam>();
            field_get_list = new List<SearchParam>();
            custom_query_list = new List<CustomQueryString>();
            join_list = new List<JoinParam>();
        }

        public void ReplaceSpecialCharacterSearch()
        {
            foreach (var item in search_list.Where(q => !string.IsNullOrWhiteSpace(q.value_search?.ToString())))
                item.value_search = item.value_search.ToString().Replace("\n", "").Replace(" ", "");
        }
    }
}
