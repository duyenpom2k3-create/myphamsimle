using System;
using System.ComponentModel.DataAnnotations;

namespace MYMVCAPP.Models
{
    public class FAQModel
    {
        [Key]
    public int Id { get; set; } // Khóa chính (Primary Key)
    
    public string Question { get; set; } // Nội dung câu hỏi
    
    public string Answer { get; set; } // Nội dung câu trả lời
    
    public int Order { get; set; } // Để sắp xếp thứ tự hiển thị
    }
}
