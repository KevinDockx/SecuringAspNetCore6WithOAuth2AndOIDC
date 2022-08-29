using ImageGallery.API.Entities;

namespace ImageGallery.API.Services
{
    public interface IGalleryRepository
    {
        Task<IEnumerable<Image>> GetImagesAsync();
        Task<bool> IsImageOwnerAsync(Guid id, string ownerId);
        Task<Image?> GetImageAsync(Guid id);
        Task<bool> ImageExistsAsync(Guid id);
        void AddImage(Image image);
        void UpdateImage(Image image);
        void DeleteImage(Image image);
        Task<bool> SaveChangesAsync();
    }
}
