using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
        private string _firstName;
        private string _lastName;
        private string _username;
        private string _email;
        private string _password;
        private string _confirmPassword;
        private string _location;
        private string _skills;
        private ImageSource _profileImage;
        private byte[] _profileImageBytes;
        private bool _isPasswordVisible;
        private bool _isConfirmPasswordVisible;
        private string _errorMessage;

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

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set => SetProperty(ref _confirmPassword, value);
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

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        public bool IsConfirmPasswordVisible
        {
            get => _isConfirmPasswordVisible;
            set => SetProperty(ref _isConfirmPasswordVisible, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand RegisterCommand { get; }
        public ICommand PickImageCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand ToggleConfirmPasswordVisibilityCommand { get; }
        public ICommand LoginCommand { get; }

        public RegisterViewModel()
        {
            Title = "Create Account";

            RegisterCommand = new Command(async () => await ExecuteRegisterCommand());
            PickImageCommand = new Command(async () => await PickProfileImage());
            TogglePasswordVisibilityCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
            ToggleConfirmPasswordVisibilityCommand = new Command(() => IsConfirmPasswordVisible = !IsConfirmPasswordVisible);
            LoginCommand = new Command(async () => await GoToLoginPageAsync());

            // Set default profile image
            ProfileImage = ImageSource.FromFile("default_profile.png");
        }

        private async Task ExecuteRegisterCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (!ValidateInput())
                    return;

                ErrorMessage = string.Empty;

                var (success, errorMessage) = await AuthService.RegisterAsync(
                    FirstName, LastName, Username, Email, Password,
                    ConfirmPassword, Location, Skills, _profileImageBytes);

                if (success)
                {
                    // Navigate to login page after successful registration
                    await Application.Current.MainPage.Navigation.PopAsync();
                    await DisplayAlert("Success", "Account created successfully. Please log in.", "OK");
                }
                else
                {
                    ErrorMessage = errorMessage ?? "Registration failed. Please check your information and try again.";
                }
            });
        }


        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(FirstName))
            {
                ErrorMessage = "First name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(LastName))
            {
                ErrorMessage = "Last name is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "Username is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required.";
                return false;
            }

            if (!Email.Contains("@") || !Email.Contains("."))
            {
                ErrorMessage = "Please enter a valid email address.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return false;
            }

            if (Password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return false;
            }

            if (Password != ConfirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Location))
            {
                ErrorMessage = "Location is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Skills))
            {
                ErrorMessage = "Skills are required.";
                return false;
            }

            return true;
        }

        private async Task PickProfileImage()
        {
            try
            {
                _profileImageBytes = await MediaService.PickPhotoAsync();

                if (_profileImageBytes != null)
                {
                    // Display the selected image
                    ProfileImage = ImageSource.FromStream(() => new MemoryStream(_profileImageBytes));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Image picking error: {ex.Message}");
                await DisplayAlert("Error", "An error occurred while selecting the image.", "OK");
            }
        }

        private async Task GoToLoginPageAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
