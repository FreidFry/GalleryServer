using Gallery.Server.Controllers.Image.Dto;
using Gallery.Server.Data.db;
using Gallery.Server.Models.Files.Image;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Gallery.Server.Controllers.Image
{
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly AppDbContext _AppDbContext;

        public ImageController(AppDbContext AppDbContext)
        {
            _AppDbContext = AppDbContext;
        }

        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> Upload([FromForm] ImageUploadDto request)
        {
            string userId = User.FindFirstValue("uid");

            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("No image file provided.");

            if (string.IsNullOrEmpty(request.Name))
                return BadRequest("Image name is required.");

            var user = await _AppDbContext.Users.FindAsync(Guid.Parse(userId));

            if (user == null)
                return NotFound("User not found.");

            var imageModel = ImageModel.Create(request.Name, request.Image.FileName, request.Description, request.Publicity, user);

            var dir = Path.GetDirectoryName(imageModel.ImageFilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir!);

            using (var stream = new FileStream(imageModel.ImageFilePath, FileMode.Create))
                await request.Image.CopyToAsync(stream);

            _AppDbContext.Images.Add(imageModel);
            await _AppDbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{TargetUid}")]
        [Authorize]
        public async Task<IActionResult> GetAll([FromRoute] string TargetUid)
        {
            string cookieUid = User.FindFirstValue("uid");
            bool isOwner = false;
            var user = await _AppDbContext.Users.FindAsync(Guid.Parse(TargetUid));

            if (cookieUid.ToUpper() == TargetUid.ToUpper())
                isOwner = true;

            if (isOwner)
            {
                var imgs = await _AppDbContext.Images
                    .Where(x => x.UserId == Guid.Parse(cookieUid))
                    .ToListAsync();
                return Ok(imgs);
            }
            var images = await _AppDbContext.Images
                .Where(x => x.UserId == Guid.Parse(TargetUid) && x.Publicity == false)
                .ToListAsync();

            return Ok(images);
        }
    }
}
