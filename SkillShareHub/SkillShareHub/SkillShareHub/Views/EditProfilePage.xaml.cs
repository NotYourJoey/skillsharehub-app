using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditProfilePage : ContentPage
    {
        private EditProfileViewModel _viewModel;

        public EditProfilePage()
        {
            InitializeComponent();
            _viewModel = (EditProfileViewModel)BindingContext;
        }

        protected override bool OnBackButtonPressed()
        {
            
            if (_viewModel.IsBusy)
                return true;

            return base.OnBackButtonPressed();
        }
    }
}
