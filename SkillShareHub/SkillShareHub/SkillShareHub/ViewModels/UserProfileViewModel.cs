using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Helpers;
using SkillShareHub.Models;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class UserProfileViewModel : BaseViewModel
    {
        private readonly int _userId;
        private User _user;
        private ObservableCollection<Post> _posts;
        private bool _isCurrentUser;
        private bool _isFriend;
        private bool _hasPendingSent;
        private bool _hasPendingReceived;

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

        public bool IsCurrentUser
        {
            get => _isCurrentUser;
            set => SetProperty(ref _isCurrentUser, value);
        }

        public bool IsFriend
        {
            get => _isFriend;
            set => SetProperty(ref _isFriend, value);
        }

        public bool HasPendingSent
        {
            get => _hasPendingSent;
            set => SetProperty(ref _hasPendingSent, value);
        }

        public bool HasPendingReceived
        {
            get => _hasPendingReceived;
            set => SetProperty(ref _hasPendingReceived, value);
        }

        public bool CanSendRequest => !IsCurrentUser && !IsFriend && !HasPendingSent && !HasPendingReceived;

        public new ICommand RefreshCommand { get; }
        public ICommand SendFriendRequestCommand { get; }
        public ICommand AcceptFriendRequestCommand { get; }
        public ICommand MessageCommand { get; }
        public ICommand EditProfileCommand { get; }
        public ICommand PostSelectedCommand { get; }

        public UserProfileViewModel(int userId)
        {
            Title = "Profile";
            _userId = userId;
            Posts = new ObservableCollection<Post>();

            RefreshCommand = new Command(async () => await RefreshProfile());
            SendFriendRequestCommand = new Command(async () => await OnSendFriendRequest());
            AcceptFriendRequestCommand = new Command(async () => await OnAcceptFriendRequest());
            MessageCommand = new Command(async () => await OnMessage());
            EditProfileCommand = new Command(async () => await OnEditProfile());
            PostSelectedCommand = new Command<Post>(async (post) => await OnPostSelected(post));

            // Check if this is the current user's profile
            IsCurrentUser = userId == Settings.UserId;

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
                // Load user info
                User = await UserService.GetUserAsync(_userId);

                if (User != null)
                {
                    // Update title
                    Title = User.Username;

                    // Load user's posts
                    var posts = await PostService.GetUserPostsAsync(_userId);

                    Posts.Clear();
                    foreach (var post in posts)
                    {
                        Posts.Add(post);
                    }

                    // Check friendship status if not viewing own profile
                    if (!IsCurrentUser)
                    {
                        var friendshipStatus = await FriendService.GetFriendshipStatusAsync(_userId);

                        IsFriend = friendshipStatus.IsFriend;
                        HasPendingSent = friendshipStatus.HasPendingSent;
                        HasPendingReceived = friendshipStatus.HasPendingReceived;

                        OnPropertyChanged(nameof(CanSendRequest));
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load profile. Please try again.", "OK");
                Console.WriteLine($"Error loading profile: {ex.Message}");
            }
        }

        private async Task OnSendFriendRequest()
        {
            try
            {
                bool success = await FriendService.SendFriendRequestAsync(_userId);

                if (success)
                {
                    HasPendingSent = true;
                    OnPropertyChanged(nameof(CanSendRequest));
                    await DisplayAlert("Success", $"Friend request sent to {User.Username}.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to send friend request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error sending friend request: {ex.Message}");
            }
        }

        private async Task OnAcceptFriendRequest()
        {
            try
            {
                bool success = await FriendService.AcceptFriendRequestAsync(_userId);

                if (success)
                {
                    HasPendingReceived = false;
                    IsFriend = true;
                    OnPropertyChanged(nameof(CanSendRequest));
                    await DisplayAlert("Success", $"You are now friends with {User.Username}.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to accept friend request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error accepting friend request: {ex.Message}");
            }
        }

        private async Task OnMessage()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new ChatPage(_userId));
        }

        private async Task OnEditProfile()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new EditProfilePage());
        }

        private async Task OnPostSelected(Post post)
        {
            if (post == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new PostDetailsPage(post.Id));
        }
    }
}
