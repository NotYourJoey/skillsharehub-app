using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Services;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class CreatePostViewModel : BaseViewModel
    {
        private string _content;
        private ImageSource _mediaSource;
        private byte[] _mediaBytes;
        private string _mediaType;
        private bool _hasMedia;

        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        public ImageSource MediaSource
        {
            get => _mediaSource;
            set => SetProperty(ref _mediaSource, value);
        }

        public bool HasMedia
        {
            get => _hasMedia;
            set => SetProperty(ref _hasMedia, value);
        }

        public ICommand PickPhotoCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand PickVideoCommand { get; }
        public ICommand TakeVideoCommand { get; }
        public ICommand RemoveMediaCommand { get; }
        public ICommand CreatePostCommand { get; }
        public ICommand CancelCommand { get; }

        public CreatePostViewModel()
        {
            Title = "Create Post";

            PickPhotoCommand = new Command(async () => await PickPhoto());
            TakePhotoCommand = new Command(async () => await TakePhoto());
            PickVideoCommand = new Command(async () => await PickVideo());
            TakeVideoCommand = new Command(async () => await TakeVideo());
            RemoveMediaCommand = new Command(RemoveMedia);
            CreatePostCommand = new Command(async () => await CreatePost());
            CancelCommand = new Command(async () => await Cancel());
        }

        private async Task PickPhoto()
        {
            try
            {
                _mediaBytes = await MediaService.PickPhotoAsync();

                if (_mediaBytes != null)
                {
                    MediaSource = ImageSource.FromStream(() => new MemoryStream(_mediaBytes));
                    _mediaType = "image";
                    HasMedia = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while selecting the image.", "OK");
                Console.WriteLine($"Error picking photo: {ex.Message}");
            }
        }

        private async Task TakePhoto()
        {
            try
            {
                _mediaBytes = await MediaService.TakePhotoAsync();

                if (_mediaBytes != null)
                {
                    MediaSource = ImageSource.FromStream(() => new MemoryStream(_mediaBytes));
                    _mediaType = "image";
                    HasMedia = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while taking the photo.", "OK");
                Console.WriteLine($"Error taking photo: {ex.Message}");
            }
        }

        private async Task PickVideo()
        {
            try
            {
                _mediaBytes = await MediaService.PickVideoAsync();

                if (_mediaBytes != null)
                {
                    // Set a video thumbnail (this is simplified - in a real app,
                    // you might want to generate a thumbnail from the video)
                    MediaSource = ImageSource.FromFile("video_placeholder.png");
                    _mediaType = "video";
                    HasMedia = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while selecting the video.", "OK");
                Console.WriteLine($"Error picking video: {ex.Message}");
            }
        }

        private async Task TakeVideo()
        {
            try
            {
                _mediaBytes = await MediaService.TakeVideoAsync();

                if (_mediaBytes != null)
                {
                    // Set a video thumbnail
                    MediaSource = ImageSource.FromFile("video_placeholder.png");
                    _mediaType = "video";
                    HasMedia = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred while recording the video.", "OK");
                Console.WriteLine($"Error taking video: {ex.Message}");
            }
        }

        private void RemoveMedia()
        {
            MediaSource = null;
            _mediaBytes = null;
            _mediaType = null;
            HasMedia = false;
        }

        private async Task CreatePost()
        {
            if (string.IsNullOrWhiteSpace(Content) && !HasMedia)
            {
                await DisplayAlert("Error", "Please add some content or media to your post.", "OK");
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    bool success = await PostService.CreatePostAsync(Content, _mediaBytes, _mediaType);

                    if (success)
                    {
                        await DisplayAlert("Success", "Your post has been created successfully.", "OK");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to create post. Please try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "An error occurred while creating your post.", "OK");
                    Console.WriteLine($"Error creating post: {ex.Message}");
                }
            });
        }

        private async Task Cancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
