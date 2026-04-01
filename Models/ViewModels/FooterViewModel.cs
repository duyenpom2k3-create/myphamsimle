using System.Collections.Generic;

namespace MYMVCAPP.Models
{
    public class FooterViewModel
    {
        public List<FooterItem> CompanyItems { get; set; }
        public List<FooterItem> PolicyItems { get; set; }
        public List<FooterItem> FollowItems { get; set; }
        public List<CategoryModel> Categories { get; set; }
    }
}
