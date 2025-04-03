using System;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Services;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class ForgotPasswordViewModel : BaseViewModel
    {
        private string _email;
        private string _errorMessage;
        private string _successMessage;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public string SuccessMessage
        {
            get => _successMessage;
            set => SetProperty(ref _successMessage, value);
        }

        public ICommand ResetPasswordCommand { get; }
        public ICommand BackToLoginCommand { get; }

        public ForgotPasswordViewModel()
        {
            Title = "Forgot Password";

            ResetPasswordCommand = new Command(async () => await ExecuteResetPasswordCommand());
            BackToLoginCommand = new Command(async () => await GoBackToLoginAsync());
        }

        private async Task ExecuteResetPasswordCommand()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                if (!ValidateInput())
                    return;

                ErrorMessage = string.Empty;
                SuccessMessage = string.Empty;

           
                await Task.Delay(1000); // Simulate network delay

                SuccessMessage = "Password reset link sent to your email. Please check your inbox.";

                // Clear email field after success
                Email = string.Empty;
            });
        }

        private bool ValidateInput()
        {
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

            return true;
        }

        private async Task GoBackToLoginAsync()
        {
            await Application.Current.MainPage.Navigation.PopAsync();
        }
    }
}
