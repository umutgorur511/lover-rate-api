using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GirlfriendRateApi.Data;
using GirlfriendRateApi.Models;

namespace GirlfriendRateApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RatingsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RatingsController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment; // Sunucu klasörlerine erişmek için
        }

        // 1. LİSTELEME - GET: api/Ratings
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RatingItem>>> GetRatings()
        {
            return await _context.RatingItems.ToListAsync();
        }

        // 2. TEK KAYIT GETİR - GET: api/Ratings/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RatingItem>> GetRating(int id)
        {
            var ratingItem = await _context.RatingItems.FindAsync(id);
            if (ratingItem == null) return NotFound();
            return ratingItem;
        }

        // 3. EKLEME (Görsel Yükleme Destekli) - POST: api/Ratings
        [HttpPost]
        public async Task<ActionResult<RatingItem>> PostRating([FromForm] RatingItem ratingItem, IFormFile? imageFile)
        {
            // SQL Server'ın Identity (otomatik artan) hatası vermemesi için ID'yi sıfırlıyoruz
            ratingItem.Id = 0;

            // Eğer bir görsel yüklenmişse işlemleri başlatıyoruz
            if (imageFile != null && imageFile.Length > 0)
            {
                // 1. Benzersiz bir dosya adı oluştur (Örn: a1b2c3d4.jpg)
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);

                // 2. Dosyanın kaydedileceği fiziksel yolu belirle (wwwroot/images/...)
                var filePath = Path.Combine(_environment.WebRootPath, "images", fileName);

                // 3. Dosyayı fiziksel olarak klasöre kaydet
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await imageFile.CopyToAsync(stream);
                }

                // 4. Veritabanına kaydedilecek olan erişim yolunu modele ata
                ratingItem.ImageUrl = "/images/" + fileName;
            }

            // Verileri veritabanına ekle ve kaydet
            _context.RatingItems.Add(ratingItem);
            await _context.SaveChangesAsync();

            // İşlem başarılıysa, yeni oluşan kaydı ID'si ile birlikte geri dön
            return CreatedAtAction(nameof(GetRating), new { id = ratingItem.Id }, ratingItem);
        }

        // 4. GÜNCELLEME - PUT: api/Ratings/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRating(int id, [FromForm] RatingItem ratingItem)
        {
            if (id != ratingItem.Id) return BadRequest();

            _context.Entry(ratingItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.RatingItems.Any(e => e.Id == id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. SİLME - DELETE: api/Ratings/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRating(int id)
        {
            var ratingItem = await _context.RatingItems.FindAsync(id);
            if (ratingItem == null) return NotFound();

            // Eğer bir görseli varsa onu da sunucudan silelim (Opsiyonel)
            if (!string.IsNullOrEmpty(ratingItem.ImageUrl))
            {
                var filePath = Path.Combine(_environment.WebRootPath, ratingItem.ImageUrl.TrimStart('/'));
                if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
            }

            _context.RatingItems.Remove(ratingItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}