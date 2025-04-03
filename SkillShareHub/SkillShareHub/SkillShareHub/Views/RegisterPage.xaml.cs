using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        private RegisterViewModel _viewModel;

        public RegisterPage()
        {
            InitializeComponent();
            _viewModel = new RegisterViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Clear error message when page appears
            _viewModel.ErrorMessage = string.Empty;
        }

        protected override bool OnBackButtonPressed()
        {
            // If we're busy processing, prevent back button
            if (_viewModel.IsBusy)
                return true;

            return base.OnBackButtonPressed();
        }
    }
}
