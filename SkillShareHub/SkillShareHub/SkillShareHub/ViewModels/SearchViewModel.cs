using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using SkillShareHub.Models;
using SkillShareHub.Services;
using SkillShareHub.Views;
using Xamarin.Forms;

namespace SkillShareHub.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private string _searchQuery;
        private string _selectedSkill;
        private ObservableCollection<User> _searchResults;
        private ObservableCollection<string> _popularSkills;
        private bool _isSearching;

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (SetProperty(ref _searchQuery, value))
                {
                    // Consider adding a delay or debounce mechanism here
                    ExecuteSearchCommand();
                }
            }
        }

        public string SelectedSkill
        {
            get => _selectedSkill;
            set
            {
                if (SetProperty(ref _selectedSkill, value))
                {
                    ExecuteSearchCommand();
                }
            }
        }

        public ObservableCollection<User> SearchResults
        {
            get => _searchResults;
            set => SetProperty(ref _searchResults, value);
        }

        public ObservableCollection<string> PopularSkills
        {
            get => _popularSkills;
            set => SetProperty(ref _popularSkills, value);
        }

        public bool IsSearching
        {
            get => _isSearching;
            set => SetProperty(ref _isSearching, value);
        }

        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand UserSelectedCommand { get; }
        public ICommand SkillSelectedCommand { get; }

        public SearchViewModel()
        {
            Title = "Search";
            SearchResults = new ObservableCollection<User>();
            PopularSkills = new ObservableCollection<string>();

            SearchCommand = new Command(async () => await ExecuteSearch());
            ClearSearchCommand = new Command(ClearSearch);
            UserSelectedCommand = new Command<User>(async (user) => await OnUserSelected(user));
            SkillSelectedCommand = new Command<string>(OnSkillSelected);

            LoadPopularSkills();
        }

        // This ensures initialization logic runs synchronously in the constructor
        private async void LoadPopularSkills()
        {
            try
            {
                // This would come from an API call in a real app
                var skills = new[]
                {
                    "Photography", "Cooking", "Programming",
                    "Design", "Writing", "Music",
                    "Painting", "Dancing", "Sport"
                };

                PopularSkills.Clear();
                foreach (var skill in skills)
                {
                    PopularSkills.Add(skill);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading popular skills: {ex.Message}");
            }
        }

        private async Task ExecuteSearch()
        {
            if (IsSearching || (string.IsNullOrWhiteSpace(SearchQuery) && string.IsNullOrWhiteSpace(SelectedSkill)))
            {
                SearchResults.Clear();
                return;
            }

            IsSearching = true;

            try
            {
                var results = await UserService.SearchUsersAsync(SearchQuery, SelectedSkill) ?? new List<User>();

                SearchResults.Clear();
                foreach (var user in results)
                    SearchResults.Add(user);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Search failed: {ex.Message}", "OK");
                Console.WriteLine($"Search error: {ex.Message}");
            }
            finally
            {
                IsSearching = false;
            }
        }

        private void ClearSearch()
        {
            SearchQuery = string.Empty;  // Ensures two-way binding clears this in the UI
            SelectedSkill = null;
            SearchResults.Clear();
        }

        private async Task OnUserSelected(User user)
        {
            if (user == null)
                return;

            await Application.Current.MainPage.Navigation.PushAsync(new UserProfilePage(user.Id));
        }

        private void OnSkillSelected(string skill)
        {
            SelectedSkill = skill;
        }

        private void ExecuteSearchCommand()
        {
            if (SearchCommand.CanExecute(null))
            {
                SearchCommand.Execute(null);
            }
        }
    }
}
