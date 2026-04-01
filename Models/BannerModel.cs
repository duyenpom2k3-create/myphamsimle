using System.ComponentModel.DataAnnotations;

namespace MYMVCAPP.Models
{
    public class BannerModel
    {
        public int Id { get; set; }

        [Required, StringLength(255)]
        public string Title { get; set; }

        [StringLength(255)]
        public string Sub_Title { get; set; }

        public string Description { get; set; }

        [Required, StringLength(255)]
        public string Image_Url { get; set; }

        [StringLength(255)]
        public string Secondary_Image_Url { get; set; }

        [StringLength(50)]
        public string Button_Text { get; set; }

        [StringLength(255)]
        public string Button_Link { get; set; }

        public bool? Is_Active { get; set; }
        public int? Sort_Order { get; set; }
    }
}
