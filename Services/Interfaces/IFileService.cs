namespace CommentApp.Services.Interfaces
{
    public interface IFileService
    {
        Task<string?> SaveImageAsync(IFormFile file);
        Task<string?> SaveTextFileAsync(IFormFile file);
    }
}
