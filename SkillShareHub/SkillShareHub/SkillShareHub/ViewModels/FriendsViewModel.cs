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
    public class FriendsViewModel : BaseViewModel
    {
        private ObservableCollection<Friend> _friends;

        public ObservableCollection<Friend> Friends
        {
            get => _friends;
            set => SetProperty(ref _friends, value);
        }

        public new ICommand RefreshCommand { get; }
        public ICommand FriendSelectedCommand { get; }
        public ICommand ViewFriendRequestsCommand { get; }
        public ICommand ViewSuggestedFriendsCommand { get; }
        public ICommand RemoveFriendCommand { get; }
        public ICommand MessageFriendCommand { get; }

        public FriendsViewModel()
        {
            Title = "Friends";
            Friends = new ObservableCollection<Friend>();

            RefreshCommand = new Command(async () => await RefreshFriends());
            FriendSelectedCommand = new Command<Friend>(async (friend) => await OnFriendSelected(friend));
            ViewFriendRequestsCommand = new Command(async () => await OnViewFriendRequests());
            ViewSuggestedFriendsCommand = new Command(async () => await OnViewSuggestedFriends());
            RemoveFriendCommand = new Command<Friend>(async (friend) => await OnRemoveFriend(friend));
            MessageFriendCommand = new Command<Friend>(async (friend) => await OnMessageFriend(friend));

            Task.Run(async () => await LoadFriendsAsync());
        }

        private async Task RefreshFriends()
        {
            IsBusy = true;

            try
            {
                await LoadFriendsAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadFriendsAsync()
        {
            try
            {
                var friends = await FriendService.GetFriendsAsync();

                Friends.Clear();
                foreach (var friend in friends)
                {
                    Friends.Add(friend);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load friends. Please try again.", "OK");
                Console.WriteLine($"Error loading friends: {ex.Message}");
            }
        }

        private async Task OnFriendSelected(Friend friend)
        {
            if (friend == null || friend.User == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(friend.User.Id));
        }

        private async Task OnViewFriendRequests()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new FriendRequestsPage());
        }

        private async Task OnViewSuggestedFriends()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new SuggestedFriendsPage());
        }

        private async Task OnRemoveFriend(Friend friend)
        {
            if (friend == null)
                return;

            bool confirm = await Application.Current.MainPage.DisplayAlert(
                "Remove Friend",
                $"Are you sure you want to remove {friend.User.Username} from your friends?",
                "Yes", "No");

            if (confirm)
            {
                try
                {
                    bool success = await FriendService.RemoveFriendAsync(friend.Id);

                    if (success)
                    {
                        Friends.Remove(friend);
                        await DisplayAlert("Success", "Friend removed successfully.", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Error", "Failed to remove friend. Please try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                    Console.WriteLine($"Error removing friend: {ex.Message}");
                }
            }
        }

        private async Task OnMessageFriend(Friend friend)
        {
            if (friend == null || friend.User == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new ChatPage(friend.User.Id));
        }
    }
}
