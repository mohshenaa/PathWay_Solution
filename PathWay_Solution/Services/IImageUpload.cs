namespace PathWay_Solution.Services
{
    public interface IImageUpload   //interface for loose coupling
    {
        Task<string>? UploadFile(IFormFile file, string folderType, CancellationToken c);
    }
    public class ImageUpload(IWebHostEnvironment webHost) : IImageUpload
    {
        public async Task<string>? UploadFile(IFormFile file, string folderType, CancellationToken c)
        {
            if (file == null) return null;  //1st we need to check file nullablity

            string folder = $"images/{folderType}";    //folder structure 

            string directoryPath = Path.Combine(webHost.WebRootPath, folderType);  //physical directory path C:\Project\wwwroot\images\users

            if (!Directory.Exists(directoryPath))  //if directory not exists then we need to create
            {
                Directory.CreateDirectory(directoryPath);
            }
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";  //for unique file name with extension

            string fileName = $"{folder}/{uniqueFileName}";  //for url
            string uploadPath = Path.Combine(webHost.WebRootPath, fileName);  //to save file

            using var stream = File.Create(uploadPath);
            await file.CopyToAsync(stream, c);

            return $"/{fileName}";
        }
    }   //need to inject it in program.cx
}
