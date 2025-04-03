using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Models;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private User _user;
        private ObservableCollection<Post> _posts;
        private int _friendsCount;
        private int _postsCount;

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public ObservableCollection<Post> Posts
        {
            get => _posts;
            set => SetProperty(ref _posts, value);
        }

        public int FriendsCount
        {
            get => _friendsCount;
            set => SetProperty(ref _friendsCount, value);
        }

        public int PostsCount
        {
            get => _postsCount;
            set => SetProperty(ref _postsCount, value);
        }

        public new ICommand RefreshCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand ViewFriendsCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand PostSelectedCommand { get; }

        public ProfileViewModel()
        {
            Title = "Profile";
            Posts = new ObservableCollection<Post>();

            RefreshCommand = new Command(async () => await RefreshProfile());
            EditProfileCommand = new Command(async () => await OnEditProfile());
            ViewFriendsCommand = new Command(async () => await OnViewFriends());
            LogoutCommand = new Command(async () => await OnLogout());
            PostSelectedCommand = new Command<Post>(async (post) => await OnPostSelected(post));

            Task.Run(async () => await LoadProfileAsync());
        }

        private async Task RefreshProfile()
        {
            IsBusy = true;

            try
            {
                await LoadProfileAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadProfileAsync()
        {
            try
            {
                // Load user profile data
                User = await UserService.GetProfileAsync();

                if (User != null)
                {
                    // Load user posts
                    var posts = await PostService.GetUserPostsAsync(User.Id);

                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }

                    PostsCount = Posts.Count;

                    // Load friends count
                    var friends = await FriendService.GetFriendsAsync();
                    FriendsCount = friends.Count;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load profile. Please try again.", "OK");
                Console.WriteLine($"Error loading profile: {ex.Message}");
            }
        }



        private async Task OnEditProfile()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditProfilePage());
        }

        private async Task OnViewFriends()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new FriendsPage());
        }

        private async Task OnLogout()
        {
            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Logout", "Are you sure you want to log out?", "Yes", "No");

            if (confirm)
            {
                AuthService.Logout();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
            }
        }

        private async Task OnPostSelected(Post post)
        {
            if (post == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new PostDetailsPage(post.Id));
        }
    }
}
