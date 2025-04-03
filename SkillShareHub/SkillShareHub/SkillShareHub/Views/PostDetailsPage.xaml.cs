using SkillShareHub.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PostDetailsPage : ContentPage
    {
        private readonly PostDetailsViewModel _viewModel;

        // Default parameterless constructor
        public PostDetailsPage()
        {
            InitializeComponent();
            // Initialize with default ViewModel or handle differently
            _viewModel = new PostDetailsViewModel();
            BindingContext = _viewModel;
        }

        // Constructor with parameters
        public PostDetailsPage(int postId, bool focusComment = false)
        {
            InitializeComponent();

            _viewModel = new PostDetailsViewModel(postId, focusComment);
            BindingContext = _viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Focus comment entry if requested
            if (_viewModel.FocusComment)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    CommentEntry.Focus();
                });
            }
        }
    }
}
