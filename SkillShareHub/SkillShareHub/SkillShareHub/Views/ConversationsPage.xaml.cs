using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConversationsPage : ContentPage
    {
        private ConversationsViewModel _viewModel;

        public ConversationsPage()
        {
            InitializeComponent();
            _viewModel = (ConversationsViewModel)BindingContext;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await _viewModel.LoadConversationsAsync();
        }
    }
}
