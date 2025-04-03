using System;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub
{
    public partial class AppShell : Shell
    {
        private int _previousTabIndex = 0;

        public AppShell()
        {
            InitializeComponent();

            // Register routes for detailed pages (non-tab pages)
            Routing.RegisterRoute("postdetails", typeof(PostDetailsPage));
            Routing.RegisterRoute("userprofile", typeof(UserProfilePage));
            Routing.RegisterRoute("editprofile", typeof(EditProfilePage));
            Routing.RegisterRoute("friendrequests", typeof(FriendRequestsPage));
            Routing.RegisterRoute("suggestedfriends", typeof(SuggestedFriendsPage));
            Routing.RegisterRoute("chat", typeof(ChatPage));

            // Listen for tab selections
            Navigating += OnShellNavigating;
        }

        private void OnShellNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // Check if it's a tab change
            if (e.Source == ShellNavigationSource.ShellItemChanged)
            {
                // Get the target location
                var location = e.Target.Location.ToString();

                // Check if navigating to create post tab
                if (location.Contains("///3")) // index-based check for create post tab
                {
                    // Cancel the automatic navigation
                    e.Cancel();

                    // Navigate back to the previous tab
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        // Show the create post page as a modal
                        await Navigation.PushModalAsync(new NavigationPage(new CreatePostPage()));
                    });
                }

                // Store the previous tab index (except for create post tab)
                if (!location.Contains("///3"))
                {
                    // Extract the tab index from the URI
                    var parts = location.Split('/');
                    if (parts.Length > 3 && int.TryParse(parts[3], out int tabIndex))
                    {
                        _previousTabIndex = tabIndex;
                    }
                }
            }
        }

        // Method to display toast messages
        public async void DisplayToast(string message)
        {
            var page = CurrentPage;
            await page.DisplayAlert("", message, "OK");
        }
    }
}
