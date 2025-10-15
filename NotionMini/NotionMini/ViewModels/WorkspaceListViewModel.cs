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
    public class WorkspaceListViewModel: BaseViewModel
    {
        private readonly IWorkspaceService _workspaceService;
        private readonly int _userId; // demo: 1

        public ObservableCollection<WorkspaceItemViewModel> Workspaces { get; } = new();

        private WorkspaceItemViewModel? _selected;
        public WorkspaceItemViewModel? Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public ICommand AddWorkspaceCommand { get; }
        public ICommand DeleteWorkspaceCommand { get; }

        public WorkspaceListViewModel(IWorkspaceService workspaceService, int userId = 1)
        {
            _workspaceService = workspaceService;
            _userId = userId;

            AddWorkspaceCommand = new RelayCommand(async () => await AddWorkspaceAsync());
            DeleteWorkspaceCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => Selected != null);
        }

        public async Task LoadAsync()
        {
            Workspaces.Clear();
            var list = await _workspaceService.GetAllAsync(_userId);
            foreach (var ws in list)
                Workspaces.Add(new WorkspaceItemViewModel(ws));
            if (Workspaces.Count > 0)
                Selected = Workspaces[0];
        }

        private async Task AddWorkspaceAsync()
        {
            var ws = await _workspaceService.CreateAsync("New Workspace", _userId);
            Workspaces.Insert(0, new WorkspaceItemViewModel(ws));
            Selected = Workspaces[0];
        }

        private async Task DeleteSelectedAsync()
        {
            if (Selected == null) return;
            await _workspaceService.DeleteAsync(Selected.WorkspaceId);
            Workspaces.Remove(Selected);
            Selected = Workspaces.Count > 0 ? Workspaces[0] : null;
        }
    }
}
