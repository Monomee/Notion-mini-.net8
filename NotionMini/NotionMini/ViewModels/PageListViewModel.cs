using NotionMini.Commands;
using NotionMini.Services;
using NotionMini.ViewModels.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotionMini.ViewModels
{
    public class PageListViewModel : BaseViewModel
    {
        private readonly IPageService _pageService;

        // danh sách tất cả page
        public ObservableCollection<PageItemViewModel> Pages { get; } = new();

        // danh sách page sau khi filter (dùng cho ListBox)
        private ObservableCollection<PageItemViewModel> _filteredPages = new();
        public ObservableCollection<PageItemViewModel> FilteredPages
        {
            get => _filteredPages;
            set => SetProperty(ref _filteredPages, value);
        }

        private int _workspaceId;
        public int WorkspaceId
        {
            get => _workspaceId;
            set
            {
                if (SetProperty(ref _workspaceId, value))
                {
                    _ = LoadAsync();
                    (AddPageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private PageItemViewModel? _selected;
        public PageItemViewModel? Selected
        {
            get => _selected;
            set
            {
                if (SetProperty(ref _selected, value))
                {
                    (DeletePageCommand as RelayCommand)?.RaiseCanExecuteChanged();
                    (PinToggleCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                    ApplySearch();
            }
        }

        public ICommand AddPageCommand { get; }
        public ICommand DeletePageCommand { get; }
        public ICommand PinToggleCommand { get; }

        public PageListViewModel(IPageService pageService)
        {
            _pageService = pageService;

            AddPageCommand = new RelayCommand(async () => await AddPageAsync(), () => WorkspaceId > 0);
            DeletePageCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => Selected != null);
            PinToggleCommand = new RelayCommand(async () => await TogglePinAsync(), () => Selected != null);
        }

        public async Task LoadAsync()
        {
            if (WorkspaceId <= 0) return;

            Pages.Clear();
            var list = await _pageService.GetByWorkspaceAsync(WorkspaceId, SearchText);

            foreach (var p in list)
                Pages.Add(new PageItemViewModel(p));

            ApplySearch();
            Selected = FilteredPages.Count > 0 ? FilteredPages[0] : null;
        }

        private async Task AddPageAsync()
        {
            var p = await _pageService.CreateAsync(WorkspaceId, "Untitled");
            var vm = new PageItemViewModel(p);
            Pages.Insert(0, vm);
            ApplySearch();
            Selected = vm;
        }

        private async Task DeleteSelectedAsync()
        {
            if (Selected == null) return;

            await _pageService.DeleteAsync(Selected.PageId);
            Pages.Remove(Selected);
            ApplySearch();
            Selected = FilteredPages.Count > 0 ? FilteredPages[0] : null;
        }

        private async Task TogglePinAsync()
        {
            if (Selected == null) return;

            var newState = !Selected.IsPinned;
            await _pageService.PinAsync(Selected.PageId, newState);
            await LoadAsync(); // reload để pin lên đầu
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredPages = new ObservableCollection<PageItemViewModel>(Pages);
            }
            else
            {
                FilteredPages = new ObservableCollection<PageItemViewModel>(
                    Pages.Where(p => p.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                );
            }
        }

    }
}
