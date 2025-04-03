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
    public class ConversationsViewModel : BaseViewModel
    {
        private ObservableCollection<object> _conversations;

        public ObservableCollection<object> Conversations
        {
            get => _conversations;
            set => SetProperty(ref _conversations, value);
        }

        public new ICommand RefreshCommand { get; }
        public ICommand ConversationSelectedCommand { get; }

        public ConversationsViewModel()
        {
            Title = "Messages";
            Conversations = new ObservableCollection<object>();

            RefreshCommand = new Command(async () => await RefreshConversations());
            ConversationSelectedCommand = new Command<object>(async (conversation) => await OnConversationSelected(conversation));

            Task.Run(async () => await LoadConversationsAsync());
        }

        private async Task RefreshConversations()
        {
            IsBusy = true;

            try
            {
                await LoadConversationsAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadConversationsAsync()
        {
            try
            {
                var conversations = await MessageService.GetConversationsAsync();

                Conversations.Clear();
                foreach (var conversation in conversations)
                {
                    Conversations.Add(conversation);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load conversations. Please try again.", "OK");
                Console.WriteLine($"Error loading conversations: {ex.Message}");
            }
        }

        private async Task OnConversationSelected(object conversation)
        {
            if (conversation == null)
                return;

            try
            {
                // Extract user info from the conversation object
                // This depends on the structure of your conversation objects
                // Assuming it has a property called User with an Id
                int userId = GetUserIdFromConversation(conversation);

                if (userId > 0)
                {
                    await Application.Current.MainPage.Navigation.PushAsync(new ChatPage(userId));
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to open conversation. Please try again.", "OK");
                Console.WriteLine($"Error opening conversation: {ex.Message}");
            }
        }

        private int GetUserIdFromConversation(object conversation)
        {
            try
            {
                // This depends on the structure of your conversation objects
                // Using reflection to extract the user ID
                var property = conversation.GetType().GetProperty("User");
                if (property != null)
                {
                    var user = property.GetValue(conversation);
                    var idProperty = user.GetType().GetProperty("Id");
                    if (idProperty != null)
                    {
                        return (int)idProperty.GetValue(user);
                    }
                }
            }
            catch
            {
                // Ignore reflection errors
            }

            return 0;
        }
    }
}
