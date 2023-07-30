namespace Forest.Helper
{
    public static class FileUpload
    {
        public static async Task<string> SaveFileAsync(this IFormFile file,string WebRootPath)
        {
            var path = "/uploads/" + Guid.NewGuid() + file.FileName;
            using FileStream filestream = new(WebRootPath + path, FileMode.Create);
            await file.CopyToAsync(filestream);
            return path;
        }
    }
}
