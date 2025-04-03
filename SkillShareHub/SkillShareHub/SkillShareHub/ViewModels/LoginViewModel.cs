using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string _email;
        private string _password;
        private bool _isPasswordVisible;
        private string _errorMessage;

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

        public bool IsPasswordVisible
        {
            get => _isPasswordVisible;
            set => SetProperty(ref _isPasswordVisible, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public ICommand LoginCommand { get; }
        public ICommand TogglePasswordVisibilityCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand ForgotPasswordCommand { get; }

        public LoginViewModel()
        {
            Title = "Login";

            LoginCommand = new Command(async () => await ExecuteLoginCommand());
            TogglePasswordVisibilityCommand = new Command(() => IsPasswordVisible = !IsPasswordVisible);
            RegisterCommand = new Command(async () => await GoToRegisterPageAsync());
            ForgotPasswordCommand = new Command(async () => await GoToForgotPasswordPageAsync());
        }

        private async Task ExecuteLoginCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (!ValidateInput())
                    return;

                ErrorMessage = string.Empty;

                var (success, errorMessage) = await AuthService.LoginAsync(Email, Password);

                if (success)
                {
                    // Navigate to main page after successful login
                    Application.Current.MainPage = new AppShell();
                }
                else
                {
                    ErrorMessage = errorMessage ?? "Invalid email or password. Please try again.";
                }
            });
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                ErrorMessage = "Email is required.";
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Password is required.";
                return false;
            }

            return true;
        }

        private async Task GoToRegisterPageAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new RegisterPage());
        }

        private async Task GoToForgotPasswordPageAsync()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ForgotPasswordPage());
        }
    }
}
