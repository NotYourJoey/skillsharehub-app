using SkillShareHub.Helpers;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Register services for dependency injection
            RegisterServices();

            // Set up main page based on auth status
            if (Settings.IsAuthenticated)
            {
                MainPage = new AppShell();
            }
            else
            {
                MainPage = new NavigationPage(new LoginPage());
            }
        }

        private void RegisterServices()
        {
            // Network and API services
            DependencyService.Register<ApiService>();

            // Authentication services
            DependencyService.Register<AuthService>();

            // Data services
            DependencyService.Register<UserService>();
            DependencyService.Register<PostService>();
            DependencyService.Register<CommentService>();
            DependencyService.Register<FriendService>();
            DependencyService.Register<MessageService>();
            DependencyService.Register<NotificationService>();

            // Media and file handling
            DependencyService.Register<MediaService>();

           

        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
