using System.ComponentModel.DataAnnotations.Schema;

namespace MYMVCAPP.Models
{

    [Table("Header")]
    public class HeaderModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Logo { get; set; }
        public string Link { get; set; }
        public string Note { get; set; }
    }
}
