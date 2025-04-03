using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using SkillShareHub.Helpers;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkillShareHub.Services
{
    public class MediaService
    {
        private readonly IApiService _apiService;

        public MediaService()
        {
            _apiService = DependencyService.Get<IApiService>();
        }

        public async Task<byte[]> PickPhotoAsync()
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select Photo"
                });

                if (result != null)
                {
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking photo: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> TakePhotoAsync()
        {
            try
            {
                var result = await MediaPicker.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take Photo"
                });

                if (result != null)
                {
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking photo: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> PickVideoAsync()
        {
            try
            {
                var result = await MediaPicker.PickVideoAsync(new MediaPickerOptions
                {
                    Title = "Select Video"
                });

                if (result != null)
                {
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error picking video: {ex.Message}");
                return null;
            }
        }

        public async Task<byte[]> TakeVideoAsync()
        {
            try
            {
                var result = await MediaPicker.CaptureVideoAsync(new MediaPickerOptions
                {
                    Title = "Capture Video"
                });

                if (result != null)
                {
                    using (var stream = await result.OpenReadAsync())
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await stream.CopyToAsync(memoryStream);
                            return memoryStream.ToArray();
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error taking video: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> SaveImageToGalleryAsync(byte[] imageData, string filename)
        {
            try
            {
                string path = Path.Combine(FileSystem.CacheDirectory, filename);
                File.WriteAllBytes(path, imageData);

                // Use Share.RequestAsync instead of MediaPicker.SavePhotoAsync
                await Share.RequestAsync(new ShareFileRequest
                {
                    Title = "Save Image",
                    File = new ShareFile(path)
                });

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving image to gallery: {ex.Message}");
                return false;
            }
        }

        public async Task<string> GetCachedMediaPathAsync(string url)
        {
            if (string.IsNullOrEmpty(url))
                return null;

            try
            {
                // Use the URL's hashcode as a unique identifier for caching
                string filename = url.GetHashCode().ToString() + Path.GetExtension(url);
                string localPath = Path.Combine(FileSystem.CacheDirectory, filename);

                // If file exists in cache and is not older than 24 hours, return it
                if (File.Exists(localPath))
                {
                    FileInfo fileInfo = new FileInfo(localPath);
                    if ((DateTime.Now - fileInfo.CreationTime).TotalHours < 24)
                    {
                        return localPath;
                    }
                }

                // If file doesn't exist in cache or is too old, download it
                var response = await _apiService.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    byte[] data = await response.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(localPath, data);
                    return localPath;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error caching media: {ex.Message}");
                return null;
            }
        }

        public Task<bool> ClearMediaCacheAsync()
        {
            try
            {
                var cacheDir = new DirectoryInfo(FileSystem.CacheDirectory);
                foreach (var file in cacheDir.GetFiles())
                {
                    file.Delete();
                }

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error clearing media cache: {ex.Message}");
                return Task.FromResult(false);
            }
        }

        public bool IsImageUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            string extension = Path.GetExtension(url).ToLowerInvariant();
            return extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".gif";
        }

        public bool IsVideoUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            string extension = Path.GetExtension(url).ToLowerInvariant();
            return extension == ".mp4" || extension == ".mov" || extension == ".wmv" || extension == ".avi";
        }

        public Task<byte[]> ResizeImageAsync(byte[] imageData)
        {
            
            return Task.FromResult(imageData);
        }
    }
}
