using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChatPage : ContentPage
    {
        private ChatViewModel _viewModel;

        public ChatPage(int userId)
        {
            InitializeComponent();
            _viewModel = new ChatViewModel(userId);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadMessagesAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Prevent back navigation during busy operations like sending messages
            if (_viewModel.IsBusy)
                return true;

            return base.OnBackButtonPressed();
        }
    }
}
