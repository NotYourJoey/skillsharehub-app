using SkillShareHub.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SkillShareHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CreatePostPage : ContentPage
    {
        private readonly CreatePostViewModel _viewModel;

        public CreatePostPage()
        {
            InitializeComponent();
            _viewModel = (CreatePostViewModel)BindingContext;
        }

        protected override bool OnBackButtonPressed()
        {
            // Confirm before leaving if there's content
            if (!string.IsNullOrWhiteSpace(_viewModel.Content) || _viewModel.HasMedia)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    bool leave = await DisplayAlert("Discard Post?",
                        "Are you sure you want to discard this post?",
                        "Discard", "Keep Editing");

                    if (leave)
                        await Navigation.PopAsync();
                });

                return true;
            }

            return base.OnBackButtonPressed();
        }
    }
}
