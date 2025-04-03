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
    public class SuggestedFriendsViewModel : BaseViewModel
    {
        private ObservableCollection<User> _suggestedFriends;

        public ObservableCollection<User> SuggestedFriends
        {
            get => _suggestedFriends;
            set => SetProperty(ref _suggestedFriends, value);
        }

        public new ICommand RefreshCommand { get; }
        public ICommand SendRequestCommand { get; }
        public ICommand UserSelectedCommand { get; }

        public SuggestedFriendsViewModel()
        {
            Title = "Suggested Friends";
            SuggestedFriends = new ObservableCollection<User>();

            RefreshCommand = new Command(async () => await RefreshSuggestions());
            SendRequestCommand = new Command<User>(async (user) => await OnSendRequest(user));
            UserSelectedCommand = new Command<User>(async (user) => await OnUserSelected(user));

            Task.Run(async () => await LoadSuggestedFriendsAsync());
        }

        private async Task RefreshSuggestions()
        {
            IsBusy = true;

            try
            {
                await LoadSuggestedFriendsAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadSuggestedFriendsAsync()
        {
            try
            {
                var suggestedFriends = await UserService.GetSuggestedFriendsAsync();

                SuggestedFriends.Clear();
                foreach (var friend in suggestedFriends)
                {
                    SuggestedFriends.Add(friend);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load suggested friends. Please try again.", "OK");
                Console.WriteLine($"Error loading suggested friends: {ex.Message}");
            }
        }

        private async Task OnSendRequest(User user)
        {
            if (user == null)
                return;

            try
            {
                bool success = await FriendService.SendFriendRequestAsync(user.Id);

                if (success)
                {
                    SuggestedFriends.Remove(user);
                    await DisplayAlert("Success", $"Friend request sent to {user.Username}.", "OK");
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

        private async Task OnUserSelected(User user)
        {
            if (user == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(user.Id));
        }
    }
}
