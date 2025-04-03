using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Services;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions; 

namespace SkillShareHub.ViewModels
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private bool _isBusy;
        private bool _isRefreshing;
        private string _title;

        // Services
        protected ApiService ApiService => DependencyService.Get<ApiService>();
        protected AuthService AuthService => DependencyService.Get<AuthService>();
        protected UserService UserService => DependencyService.Get<UserService>();
        protected PostService PostService => DependencyService.Get<PostService>();
        protected CommentService CommentService => DependencyService.Get<CommentService>();
        protected FriendService FriendService => DependencyService.Get<FriendService>();
        protected MessageService MessageService => DependencyService.Get<MessageService>();
        protected NotificationService NotificationService => DependencyService.Get<NotificationService>();
        protected MediaService MediaService => DependencyService.Get<MediaService>();

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        public ICommand RefreshCommand { get; set; }

        public BaseViewModel()
        {
            RefreshCommand = new Command(async () => await ExecuteRefreshCommand());
        }

        protected virtual async Task ExecuteRefreshCommand()
        {
            if (IsRefreshing)
                return;

            IsRefreshing = true;

            try
            {
                await LoadDataAsync();
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        protected virtual Task LoadDataAsync()
        {
            return Task.CompletedTask;
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual Task InitializeAsync(object parameter)
        {
            return Task.CompletedTask;
        }

        protected async Task ExecuteWithBusyIndicator(Func<Task> action)
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task<TResult> ExecuteWithBusyIndicator<TResult>(Func<Task<TResult>> action)
        {
            if (IsBusy)
                return default;

            IsBusy = true;

            try
            {
                return await action();
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected async Task DisplayAlert(string title, string message, string cancel)
        {
            await Shell.Current.DisplayAlert(title, message, cancel);
        }

        protected async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
        {
            return await Shell.Current.DisplayAlert(title, message, accept, cancel);
        }

        // Update DisplayToast to use Xamarin.CommunityToolkit
        protected async Task DisplayToast(string message)
        {
            await Shell.Current.DisplayToastAsync(message, 3000); 
        }
    }
}
