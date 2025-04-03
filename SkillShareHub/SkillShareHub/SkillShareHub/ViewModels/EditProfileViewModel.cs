using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Models;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class EditProfileViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _location;
        private string _skills;
        private ImageSource _profileImage;
        private byte[] _profileImageBytes;
        private bool _isImageChanged;

        public string FirstName
        {
            get => _firstName;
            set => SetProperty(ref _firstName, value);
        }

        public string LastName
        {
            get => _lastName;
            set => SetProperty(ref _lastName, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public string Skills
        {
            get => _skills;
            set => SetProperty(ref _skills, value);
        }

        public ImageSource ProfileImage
        {
            get => _profileImage;
            set => SetProperty(ref _profileImage, value);
        }

        public ICommand PickImageCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LogoutCommand { get; }

        public EditProfileViewModel()
        {
            Title = "Edit Profile";

            PickImageCommand = new Command(async () => await OnPickImage());
            SaveCommand = new Command(async () => await OnSave());
            CancelCommand = new Command(async () => await OnCancel());
            LogoutCommand = new Command(async () => await OnLogout());

            Task.Run(async () => await LoadProfileDataAsync());
        }

        private async Task LoadProfileDataAsync()
        {
            try
            {
                var user = await UserService.GetProfileAsync();

                if (user != null)
                {
                    FirstName = user.FirstName;
                    LastName = user.LastName;
                    Location = user.Location;
                    Skills = user.Skills;

                    if (!string.IsNullOrEmpty(user.ProfilePhotoUrl))
                    {
                        ProfileImage = user.ProfilePhotoUrl;
                    }
                    else
                    {
                        ProfileImage = "default_profile.png";
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load profile data. Please try again.", "OK");
                Console.WriteLine($"Error loading profile data: {ex.Message}");
            }
        }

        private async Task OnPickImage()
        {
            try
            {
                _profileImageBytes = await MediaService.PickPhotoAsync();

                if (_profileImageBytes != null)
                {
                    ProfileImage = ImageSource.FromStream(() => new MemoryStream(_profileImageBytes));
                    _isImageChanged = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to pick image. Please try again.", "OK");
                Console.WriteLine($"Error picking image: {ex.Message}");
            }
        }

        private async Task OnSave()
        {
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Location) || string.IsNullOrWhiteSpace(Skills))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    bool success = await UserService.UpdateProfileAsync(
                        FirstName, LastName, Location, Skills,
                        _isImageChanged ? _profileImageBytes : null);

                    if (success)
                    {
                        await DisplayAlert("Success", "Profile updated successfully.", "OK");
                        await Application.Current.MainPage.Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to update profile. Please try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                    Console.WriteLine($"Error updating profile: {ex.Message}");
                }
            });
        }

        private async Task OnCancel()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        private async Task OnLogout()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", "Are you sure you want to log out?", "Yes", "No");

            if (confirm)
            {
                AuthService.Logout();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }
    }
}
