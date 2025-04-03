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
    public class ChatViewModel : BaseViewModel
    {
        private readonly int _userId;
        private User _user;
        private string _newMessage;
        private ObservableCollection<Message> _messages;

        public User User
        {
            get => _user;
            set => SetProperty(ref _user, value);
        }

        public string NewMessage
        {
            get => _newMessage;
            set => SetProperty(ref _newMessage, value);
        }

        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set => SetProperty(ref _messages, value);
        }

        // Explicitly shadow the base class properties or commands that might be causing the warning
        // For example, if RefreshCommand exists in BaseViewModel
        public new ICommand RefreshCommand { get; }

        public ICommand SendMessageCommand { get; }
        public ICommand ViewProfileCommand { get; }

        public ChatViewModel(int userId)
        {
            _userId = userId;
            Messages = new ObservableCollection<Message>();

            SendMessageCommand = new Command(async () => await OnSendMessage());
            RefreshCommand = new Command(async () => await RefreshMessages());
            ViewProfileCommand = new Command(async () => await OnViewProfile());

            Task.Run(async () => await LoadUserAndMessages());
        }

        private async Task LoadUserAndMessages()
        {
            try
            {
                // Load user info
                User = await UserService.GetUserAsync(_userId);

                if (User != null)
                {
                    // Set the page title to the username
                    Title = User.Username;

                    // Load messages
                    await LoadMessagesAsync();
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load conversation. Please try again.", "OK");
                Console.WriteLine($"Error loading conversation: {ex.Message}");
            }
        }

        // This method might be causing the warning if LoadDataAsync is in BaseViewModel
        // Either rename it or use the new keyword
        private async Task RefreshMessages()
        {
            IsBusy = true;

            try
            {
                await LoadMessagesAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        // Make this a new method if there's a similar one in the base class
        public new async Task LoadDataAsync()
        {
            await LoadMessagesAsync();
        }

        public async Task LoadMessagesAsync()
        {
            try
            {
                var messages = await MessageService.GetMessagesWithUserAsync(_userId);

                Messages.Clear();
                foreach (var message in messages)
                {
                    Messages.Add(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading messages: {ex.Message}");
            }
        }

        private async Task OnSendMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessage))
                return;

            string messageToSend = NewMessage.Trim();
            NewMessage = string.Empty; // Clear input field immediately for better UX

            try
            {
                var message = await MessageService.SendMessageAsync(_userId, messageToSend);

                if (message != null)
                {
                    Messages.Add(message);
                }
                else
                {
                    // If API call failed, show error and restore the message text
                    await DisplayAlert("Error", "Failed to send message. Please try again.", "OK");
                    NewMessage = messageToSend;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error sending message: {ex.Message}");
                NewMessage = messageToSend; // Restore message text
            }
        }

        private async Task OnViewProfile()
        {
            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(_userId));
        }
    }
}
