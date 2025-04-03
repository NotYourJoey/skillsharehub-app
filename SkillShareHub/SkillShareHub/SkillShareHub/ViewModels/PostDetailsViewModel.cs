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
    public class PostDetailsViewModel : BaseViewModel
    {
        private int _postId;
        private Post _post;
        private string _newComment;
        private bool _focusComment;
        private ObservableCollection<Comment> _comments;

        public Post Post
        {
            get => _post;
            set => SetProperty(ref _post, value);
        }

        public string NewComment
        {
            get => _newComment;
            set => SetProperty(ref _newComment, value);
        }

        public bool FocusComment
        {
            get => _focusComment;
            set => SetProperty(ref _focusComment, value);
        }

        public ObservableCollection<Comment> Comments
        {
            get => _comments;
            set => SetProperty(ref _comments, value);
        }

        public ICommand LikeCommand { get; }
        public ICommand AddCommentCommand { get; }
        public ICommand UserTappedCommand { get; }
        public new ICommand RefreshCommand { get; }

        // Default parameterless constructor for initialization without data
        public PostDetailsViewModel()
        {
            Title = "Post Details";
            _postId = 0; // Set to 0 or another value to indicate a default or initial state
            _focusComment = false;
            Comments = new ObservableCollection<Comment>();

            LikeCommand = new Command(async () => await OnLikePost());
            AddCommentCommand = new Command(async () => await OnAddComment());
            UserTappedCommand = new Command<User>(async (user) => await OnUserTapped(user));
            RefreshCommand = new Command(async () => await RefreshDataAsync());

            // You may choose to initialize default data or a generic state here
        }

        // Constructor with parameters for specific post loading
        public PostDetailsViewModel(int postId, bool focusComment = false) : this()
        {
            _postId = postId;
            _focusComment = focusComment;

            // Initiate loading given a specific postId
            _ = LoadPostAsync();
        }

        private async Task LoadPostAsync()
        {
            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    Post = await PostService.GetPostAsync(_postId);
                    if (Post != null)
                    {
                        await LoadCommentsAsync();
                    }
                    else
                    {
                        // Post is not found - handle accordingly
                        await DisplayAlert("Error", "Post not found. Please check the ID and try again.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to load post. Please try again.", "OK");
                    Console.WriteLine($"Error loading post: {ex.Message}");
                }
            });
        }

        private async Task LoadCommentsAsync()
        {
            try
            {
                var comments = await CommentService.GetCommentsAsync(_postId);
                Comments.Clear();
                foreach (var comment in comments)
                {
                    Comments.Add(comment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading comments: {ex.Message}");
            }
        }

        private async Task OnLikePost()
        {
            try
            {
                if (Post.IsLiked)
                {
                    bool success = await PostService.UnlikePostAsync(Post.Id);
                    if (success)
                    {
                        Post.IsLiked = false;
                        Post.LikesCount--;
                    }
                }
                else
                {
                    bool success = await PostService.LikePostAsync(Post.Id);
                    if (success)
                    {
                        Post.IsLiked = true;
                        Post.LikesCount++;
                    }
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to like/unlike post. Please try again.", "OK");
                Console.WriteLine($"Error liking post: {ex.Message}");
            }
        }

        private async Task OnAddComment()
        {
            if (string.IsNullOrWhiteSpace(NewComment))
                return;

            await ExecuteWithBusyIndicator(async () =>
            {
                try
                {
                    var comment = await CommentService.AddCommentAsync(_postId, NewComment);
                    if (comment != null)
                    {
                        Comments.Add(comment);
                        Post.CommentsCount++;
                        NewComment = string.Empty;
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", "Failed to add comment. Please try again.", "OK");
                    Console.WriteLine($"Error adding comment: {ex.Message}");
                }
            });
        }

        private async Task OnUserTapped(User user)
        {
            if (user == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(user.Id));
        }

        public async Task RefreshDataAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await LoadPostAsync();
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}

