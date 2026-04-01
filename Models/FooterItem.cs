using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    [Table("FooterItems")]
    public class FooterItem
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("item_type")]
        public string ItemType { get; set; } = string.Empty;

        [Column("item_key")]
        public string? ItemKey { get; set; }

        [Column("item_title")]
        public string ItemTitle { get; set; } = string.Empty;

        [Column("Slug")]
        public string? Slug { get; set; }

        [Column("display_order")]
        public int? DisplayOrder { get; set; }
        [Column("content")]
        public string? Content { get; set; }

    }
}
