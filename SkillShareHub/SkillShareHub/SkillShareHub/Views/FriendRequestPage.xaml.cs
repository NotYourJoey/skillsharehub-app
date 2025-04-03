using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FriendRequestsPage : ContentPage
    {
        private FriendRequestsViewModel _viewModel;

        public FriendRequestsPage()
        {
            InitializeComponent();
            _viewModel = (FriendRequestsViewModel)BindingContext;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadRequestsAsync();
        }
    }
}
