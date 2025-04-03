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
    public class FriendRequestsViewModel : BaseViewModel
    {
        private ObservableCollection<Friend> _receivedRequests;
        private ObservableCollection<Friend> _sentRequests;
        private bool _isReceivedSelected;

        public ObservableCollection<Friend> ReceivedRequests
        {
            get => _receivedRequests;
            set => SetProperty(ref _receivedRequests, value);
        }

        public ObservableCollection<Friend> SentRequests
        {
            get => _sentRequests;
            set => SetProperty(ref _sentRequests, value);
        }

        public bool IsReceivedSelected
        {
            get => _isReceivedSelected;
            set => SetProperty(ref _isReceivedSelected, value);
        }

        public bool IsSentSelected => !IsReceivedSelected;

        public new ICommand RefreshCommand { get; }
        public ICommand AcceptRequestCommand { get; }
        public ICommand RejectRequestCommand { get; }
        public ICommand CancelRequestCommand { get; }
        public ICommand ToggleViewCommand { get; }
        public ICommand UserSelectedCommand { get; }

        public FriendRequestsViewModel()
        {
            Title = "Friend Requests";
            ReceivedRequests = new ObservableCollection<Friend>();
            SentRequests = new ObservableCollection<Friend>();
            IsReceivedSelected = true;

            RefreshCommand = new Command(async () => await RefreshRequests());
            AcceptRequestCommand = new Command<Friend>(async (request) => await OnAcceptRequest(request));
            RejectRequestCommand = new Command<Friend>(async (request) => await OnRejectRequest(request));
            CancelRequestCommand = new Command<Friend>(async (request) => await OnCancelRequest(request));
            ToggleViewCommand = new Command<string>(OnToggleView);
            UserSelectedCommand = new Command<User>(async (user) => await OnUserSelected(user));

            Task.Run(async () => await LoadRequestsAsync());
        }

        private async Task RefreshRequests()
        {
            IsBusy = true;

            try
            {
                await LoadRequestsAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadRequestsAsync()
        {
            try
            {
                var receivedRequests = await FriendService.GetFriendRequestsAsync();
                var sentRequests = await FriendService.GetSentRequestsAsync();

                ReceivedRequests.Clear();
                foreach (var request in receivedRequests)
                {
                    ReceivedRequests.Add(request);
                }

                SentRequests.Clear();
                foreach (var request in sentRequests)
                {
                    SentRequests.Add(request);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load friend requests. Please try again.", "OK");
                Console.WriteLine($"Error loading friend requests: {ex.Message}");
            }
        }

        private async Task OnAcceptRequest(Friend request)
        {
            if (request == null)
                return;

            try
            {
                bool success = await FriendService.AcceptFriendRequestAsync(request.Id);

                if (success)
                {
                    ReceivedRequests.Remove(request);
                    await DisplayAlert("Success", "Friend request accepted.", "OK");
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

        private async Task OnRejectRequest(Friend request)
        {
            if (request == null)
                return;

            try
            {
                bool success = await FriendService.DeleteFriendRequestAsync(request.Id);

                if (success)
                {
                    ReceivedRequests.Remove(request);
                    await DisplayAlert("Success", "Friend request rejected.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to reject friend request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error rejecting friend request: {ex.Message}");
            }
        }

        private async Task OnCancelRequest(Friend request)
        {
            if (request == null)
                return;

            try
            {
                bool success = await FriendService.DeleteFriendRequestAsync(request.Id);

                if (success)
                {
                    SentRequests.Remove(request);
                    await DisplayAlert("Success", "Friend request cancelled.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to cancel friend request. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error cancelling friend request: {ex.Message}");
            }
        }

        private void OnToggleView(string view)
        {
            if (view == "received")
            {
                IsReceivedSelected = true;
            }
            else if (view == "sent")
            {
                IsReceivedSelected = false;
            }

            OnPropertyChanged(nameof(IsSentSelected));
        }

        private async Task OnUserSelected(User user)
        {
            if (user == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(user.Id));
        }
    }
}
