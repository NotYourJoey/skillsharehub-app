using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        private LoginViewModel _viewModel;

        public LoginPage()
        {
            InitializeComponent();
            _viewModel = new LoginViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Clear any existing values when page appears
            _viewModel.Email = string.Empty;
            _viewModel.Password = string.Empty;
            _viewModel.ErrorMessage = string.Empty;
        }
    }
}
