using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Models;
using SkillShareHub.Services;
using Xamarin.Forms;
using SkillShareHub.Converters;

namespace SkillShareHub.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        private ObservableCollection<Post> _posts;

        public ObservableCollection<Post> Posts
        {
            get => _posts;
            set => SetProperty(ref _posts, value);
        }

        private User _user;
        public User User
        {
            get => _user;
            set
            {
                _user = value;
                OnPropertyChanged(nameof(User)); // Required for binding updates
            }
        }

      
        private string _mediaUrl;
        public string MediaUrl
        {
            get => _mediaUrl;
            set
            {
                _mediaUrl = value;
                OnPropertyChanged(nameof(MediaUrl)); // Important for UI updates
            }
        }

      
        private bool _isLiked;
        public bool IsLiked
        {
            get => _isLiked;
            set
            {
                if (_isLiked != value)
                {
                    _isLiked = value;
                    OnPropertyChanged(nameof(IsLiked)); // Crucial for UI updates
                                                        
                }
            }
        }

        
        private int _likesCount;
        public int LikesCount
        {
            get => _likesCount;
            set
            {
                if (_likesCount != value)
                {
                    _likesCount = value;
                    OnPropertyChanged(nameof(LikesCount)); // Essential for UI updates
                                                           
                }
            }
        }

       
        private string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content)); // Essential for UI updates
                }
            }
        }

        
        private int _commentsCount;
        public int CommentsCount
        {
            get => _commentsCount;
            set
            {
                if (_commentsCount != value)
                {
                    _commentsCount = value;
                    OnPropertyChanged(nameof(CommentsCount)); // Critical for UI updates
                                                              
                }
            }
        }

       
        private DateTime _createdAt;
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                    
                }
            }
        }

        public ICommand PostSelectedCommand { get; }
        public ICommand LikePostCommand { get; }
        public ICommand CommentPostCommand { get; }

        public HomeViewModel()
        {
            Title = "SkillShareHub";
            Posts = new ObservableCollection<Post>();

            PostSelectedCommand = new Command<Post>(async (post) => await OnPostSelected(post));
            LikePostCommand = new Command<Post>(async (post) => await OnLikePost(post));
            CommentPostCommand = new Command<Post>(async (post) => await OnCommentPost(post));
        }

        public async Task LoadPostsAsync()
        {
            await LoadDataAsync();
        }

        protected override async Task LoadDataAsync()
        {
            try
            {
                var posts = await PostService.GetPostsAsync();

                Posts.Clear();
                foreach (var post in posts)
                {
                    if (post.User != null)
                    {
                        Console.WriteLine($"User: {post.User.Username}, Profile Photo URL: {post.User.ProfilePhotoUrl}");
                    }
                    Posts.Add(post);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load posts. Please try again.", "OK");
                Console.WriteLine($"Error loading posts: {ex.Message}");
            }
        }

        private async Task OnPostSelected(Post post)
        {
            if (post == null)
                return;

            await Shell.Current.GoToAsync($"postdetails?postId={post.Id}");
        }

        private async Task OnLikePost(Post post)
        {
            try
            {
                if (post.IsLiked)
                {
                    bool success = await PostService.UnlikePostAsync(post.Id);
                    if (success)
                    {
                        post.IsLiked = false;
                        post.LikesCount--;
                    }
                }
                else
                {
                    bool success = await PostService.LikePostAsync(post.Id);
                    if (success)
                    {
                        post.IsLiked = true;
                        post.LikesCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to like/unlike post. Please try again.", "OK");
                Console.WriteLine($"Error liking post: {ex.Message}");
            }
        }

        private async Task OnCommentPost(Post post)
        {
            await Shell.Current.GoToAsync($"postdetails?postId={post.Id}&focusComment=true");
        }
    }
}
