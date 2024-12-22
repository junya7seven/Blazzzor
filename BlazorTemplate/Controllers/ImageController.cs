using BlazorTemplate;
using BlazorTemplate.Models;
using Entities.Interfaces;
using Entities.Models;
using Infrasrtucture.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace BlazorTemplateAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly MyDbContext _context;
        public ImageController(IWebHostEnvironment environment, UserManager<ApplicationUser> userManager, MyDbContext context)
        {
            _environment = environment;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost("uploadImage")]
        public async Task<IActionResult> UploadProfileImage(IFormFile file, [FromQuery] Guid userId)
        {
            var existUser = _userManager.GetUserByIdAsync(userId);
            if (existUser == null)
            {
                return BadRequest("User not found");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("Invalid file");
            }

            var uploadFolder = Path.Combine(_environment.WebRootPath, "uploads", "profile-images");
            if (!Directory.Exists(uploadFolder))
            {
                Directory.CreateDirectory(uploadFolder);
            }
            var uniqueFileName = $"{userId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadFolder, uniqueFileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            var fileUrl = $"/uploads/profile-images/{uniqueFileName}";
            var imageProfile = new UserProfileImage
            {
                UserId = userId,
                ProfileImagePath = filePath,
            };
            await _context.ProfileImages.AddAsync(imageProfile);
            await _context.SaveChangesAsync();
            return Ok(fileUrl);
        }
        [HttpGet]
        public async Task<IActionResult> GetProfileImage([FromQuery] Guid userId)
        {
            var imageUrl = await _context.ProfileImages.FirstOrDefaultAsync(x => x.UserId == userId);
            if(imageUrl == null || string.IsNullOrEmpty(imageUrl.ProfileImagePath))
            {
                return BadRequest("Пользователь не найден или отсутствует изображение");
            }

            var filePath = Path.Combine(_environment.WebRootPath, imageUrl.ProfileImagePath);
            var contentTypeProvider = new FileExtensionContentTypeProvider();
            if (!contentTypeProvider.TryGetContentType(filePath, out var contentType))
            {
                contentType = "application/octet-stream";
            }
            return PhysicalFile(filePath, contentType);

        }
    }
}
