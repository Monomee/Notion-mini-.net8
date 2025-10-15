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

        public ObservableCollection<PageItemViewModel> Pages { get; } = new();

        private int _workspaceId;
        public int WorkspaceId
        {
            get => _workspaceId;
            set
            {
                if (SetProperty(ref _workspaceId, value))
                {
                    _ = LoadAsync(); // fire & forget
                }
            }
        }

        private PageItemViewModel? _selected;
        public PageItemViewModel? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public ICommand AddPageCommand { get; }
        public ICommand DeletePageCommand { get; }
        public ICommand PinToggleCommand { get; }
        public ICommand ApplySearchCommand { get; }

        public PageListViewModel(IPageService pageService)
        {
            _pageService = pageService;
            AddPageCommand = new RelayCommand(async () => await AddPageAsync(), () => WorkspaceId > 0);
            DeletePageCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => Selected != null);
            PinToggleCommand = new RelayCommand(async () => await TogglePinAsync(), () => Selected != null);
            ApplySearchCommand = new RelayCommand(async () => await LoadAsync());
        }

        public async Task LoadAsync()
        {
            if (WorkspaceId <= 0) return;
            Pages.Clear();
            var list = await _pageService.GetByWorkspaceAsync(WorkspaceId, SearchText);
            foreach (var p in list)
                Pages.Add(new PageItemViewModel(p));

            Selected = Pages.Count > 0 ? Pages[0] : null;
        }

        private async Task AddPageAsync()
        {
            var p = await _pageService.CreateAsync(WorkspaceId, "Untitled");
            Pages.Insert(0, new PageItemViewModel(p));
            Selected = Pages[0];
        }

        private async Task DeleteSelectedAsync()
        {
            if (Selected == null) return;
            await _pageService.DeleteAsync(Selected.PageId);
            Pages.Remove(Selected);
            Selected = Pages.Count > 0 ? Pages[0] : null;
        }

        private async Task TogglePinAsync()
        {
            if (Selected == null) return;
            var newState = !Selected.IsPinned;
            await _pageService.PinAsync(Selected.PageId, newState);
            await LoadAsync();
        }
    }
}
