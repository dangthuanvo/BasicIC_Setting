using System.ComponentModel.DataAnnotations;

namespace BasicIC_Setting.Models.Common
{
    public class ActiveModel
    {
        [Required]
        public string id { get; set; }
        [Required]
        public bool active { get; set; }
    }
}