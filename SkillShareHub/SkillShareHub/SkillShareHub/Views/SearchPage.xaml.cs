using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchPage : ContentPage
    {
        private SearchViewModel _viewModel;

        public SearchPage()
        {
            InitializeComponent();
            _viewModel = (SearchViewModel)BindingContext;
        }
    }
}
