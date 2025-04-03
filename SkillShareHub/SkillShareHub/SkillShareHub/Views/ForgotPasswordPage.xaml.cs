using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ForgotPasswordPage : ContentPage
    {
        private ForgotPasswordViewModel _viewModel;

        public ForgotPasswordPage()
        {
            InitializeComponent();
            _viewModel = new ForgotPasswordViewModel();
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Clear messages when page appears
            _viewModel.ErrorMessage = string.Empty;
            _viewModel.SuccessMessage = string.Empty;
        }
    }
}
