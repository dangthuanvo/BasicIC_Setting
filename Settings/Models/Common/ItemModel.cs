using System;
using System.ComponentModel.DataAnnotations;

namespace BasicIC_Setting.Models.Common
{
    public class ItemModel
    {
        [Required]
        public string id { get; set; }

        public ItemModel()
        {
            this.id = null;
        }

        public ItemModel(string id)
        {
            this.id = id;
        }

        public ItemModel(Guid id)
        {
            this.id = id.ToString();
        }
    }
}