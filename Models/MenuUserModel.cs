using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{
    [Table("MenuUser")]
    public class MenuUserModel
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        public string? Slug { get; set; }

        public bool? Is_active { get; set; }
    }
}
