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
    public class NotificationsViewModel : BaseViewModel
    {
        private ObservableCollection<Notification> _notifications;

        public ObservableCollection<Notification> Notifications
        {
            get => _notifications;
            set => SetProperty(ref _notifications, value);
        }

        public new ICommand RefreshCommand { get; }
        public ICommand MarkAllAsReadCommand { get; }
        public ICommand NotificationTappedCommand { get; }

        public NotificationsViewModel()
        {
            Title = "Notifications";
            Notifications = new ObservableCollection<Notification>();

            RefreshCommand = new Command(async () => await RefreshNotifications());
            MarkAllAsReadCommand = new Command(async () => await OnMarkAllAsRead());
            NotificationTappedCommand = new Command<Notification>(async (notification) => await OnNotificationTapped(notification));

            Task.Run(async () => await LoadNotificationsAsync());
        }

        private async Task RefreshNotifications()
        {
            IsBusy = true;

            try
            {
                await LoadNotificationsAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }

        public async Task LoadNotificationsAsync()
        {
            try
            {
                var notifications = await NotificationService.GetNotificationsAsync();

                Notifications.Clear();
                foreach (var notification in notifications)
                {
                    Notifications.Add(notification);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load notifications. Please try again.", "OK");
                Console.WriteLine($"Error loading notifications: {ex.Message}");
            }
        }

        private async Task OnMarkAllAsRead()
        {
            try
            {
                bool success = await NotificationService.MarkAllAsReadAsync();

                if (success)
                {
                    // Mark all notifications as read in the UI
                    foreach (var notification in Notifications)
                    {
                        notification.IsRead = true;
                    }

                    await DisplayAlert("Success", "All notifications marked as read.", "OK");
                }
                else
                {
                    await DisplayAlert("Error", "Failed to mark notifications as read. Please try again.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "An error occurred. Please try again.", "OK");
                Console.WriteLine($"Error marking notifications as read: {ex.Message}");
            }
        }

        private async Task OnNotificationTapped(Notification notification)
        {
            if (notification == null)
                return;

            try
            {
                // Mark the notification as read
                if (!notification.IsRead)
                {
                    bool success = await NotificationService.MarkAsReadAsync(notification.Id);

                    if (success)
                    {
                        notification.IsRead = true;
                    }
                }

                // Navigate based on notification type
                switch (notification.Type)
                {
                    case "friend_request":
                        await Application.Current.MainPage.Navigation.PushAsync(new FriendRequestsPage());
                        break;

                    case "friend_accepted":
                        await Application.Current.MainPage.Navigation.PushAsync(new FriendsPage());
                        break;

                    case "comment":
                    case "like":
                        // Extract post ID if available in the notification message
                        int postId = ExtractPostIdFromMessage(notification.Message);
                        if (postId > 0)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new PostDetailsPage(postId));
                        }
                        break;

                    case "message":
                        // Extract user ID if available in the notification message
                        int userId = ExtractUserIdFromMessage(notification.Message);
                        if (userId > 0)
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new ChatPage(userId));
                        }
                        else
                        {
                            await Application.Current.MainPage.Navigation.PushAsync(new ConversationsPage());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error handling notification: {ex.Message}");
            }
        }

        private int ExtractPostIdFromMessage(string message)
        {
            // This is a placeholder. In a real implementation, you would extract
            // the post ID from the notification message using a consistent format.
            // For example: "User commented on your post #123"

            try
            {
                if (message.Contains("post #"))
                {
                    int startIndex = message.IndexOf("post #") + 6;
                    int endIndex = message.IndexOf(' ', startIndex);
                    if (endIndex == -1) endIndex = message.Length;

                    string postIdStr = message.Substring(startIndex, endIndex - startIndex);
                    if (int.TryParse(postIdStr, out int postId))
                    {
                        return postId;
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return 0;
        }

        private int ExtractUserIdFromMessage(string message)
        {
            // Similar to ExtractPostIdFromMessage, this is a placeholder

            try
            {
                if (message.Contains("user #"))
                {
                    int startIndex = message.IndexOf("user #") + 6;
                    int endIndex = message.IndexOf(' ', startIndex);
                    if (endIndex == -1) endIndex = message.Length;

                    string userIdStr = message.Substring(startIndex, endIndex - startIndex);
                    if (int.TryParse(userIdStr, out int userId))
                    {
                        return userId;
                    }
                }
            }
            catch
            {
                // Ignore parsing errors
            }

            return 0;
        }
    }
}
