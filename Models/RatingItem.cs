using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GirlfriendRateApi.Models
{
    public class RatingItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // SQL bu ID'yi kendisi üretecek
       public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Score { get; set; }
        
        // Görselin sunucudaki yolunu veya URL'sini tutacak alan
        public string? ImageUrl { get; set; } 
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}