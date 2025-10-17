using NotionMini.Commands;
using NotionMini.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NotionMini.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public WorkspaceListViewModel WorkspaceVM { get; }
        public PageListViewModel PageVM { get; }
        public EditorViewModel EditorVM { get; }

        private readonly IPageService _pageService;

        private bool _isBusy;
        public bool IsBusy { get => _isBusy; set => SetProperty(ref _isBusy, value); }

        public ICommand InitializeCommand { get; }
        public ICommand SelectWorkspaceCommand { get; }
        public ICommand SelectPageCommand { get; }

        public MainViewModel() : this(new WorkspaceService(), new PageService())
        {

        }
    

        public MainViewModel(IWorkspaceService workspaceService, IPageService pageService)
        {
            _pageService = pageService;

            WorkspaceVM = new WorkspaceListViewModel(workspaceService);
            PageVM = new PageListViewModel(pageService);
            EditorVM = new EditorViewModel(pageService);

            InitializeCommand = new RelayCommand(async () => await InitializeAsync());
            SelectWorkspaceCommand = new RelayCommand(async () => await OnWorkspaceChangedAsync(), () => WorkspaceVM.Selected != null);
            SelectPageCommand = new RelayCommand(async () => await OnPageChangedAsync(), () => PageVM.Selected != null);

            // auto-react khi Selected thay đổi (simple approach)
            WorkspaceVM.PropertyChanged += async (_, e) =>
            {
                if (e.PropertyName == nameof(WorkspaceListViewModel.Selected))
                    await OnWorkspaceChangedAsync();
            };
            PageVM.PropertyChanged += async (_, e) =>
            {
                if (e.PropertyName == nameof(PageListViewModel.Selected))
                    await OnPageChangedAsync();
            };
        }

        public async Task InitializeAsync()
        {
            IsBusy = true;
            await WorkspaceVM.LoadAsync();
            await OnWorkspaceChangedAsync();
            IsBusy = false;
        }

        private async Task OnWorkspaceChangedAsync()
        {
            var ws = WorkspaceVM.Selected;
            PageVM.WorkspaceId = ws?.WorkspaceId ?? 0;
            // PageVM.LoadAsync() sẽ tự chạy trong setter
        }

        private async Task OnPageChangedAsync()
        {
            var p = PageVM.Selected;
            if (p == null) return;
            await EditorVM.LoadAsync(p.PageId);
        }

    }
}
