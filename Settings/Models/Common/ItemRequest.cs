using System.ComponentModel.DataAnnotations;

namespace BasicIC_Setting.Models.Common
{
    public class ItemRequest
    {
        [Required]
        public string item { get; set; }
    }
}