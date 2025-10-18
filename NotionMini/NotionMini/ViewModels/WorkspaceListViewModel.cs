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
        private bool _isDeleting = false;

        // danh sách tất cả workspace
        public ObservableCollection<WorkspaceItemViewModel> Workspaces { get; } = new();

        // danh sách workspace sau khi filter (dùng cho ListBox)
        private ObservableCollection<WorkspaceItemViewModel> _filteredWorkspaces = new();
        public ObservableCollection<WorkspaceItemViewModel> FilteredWorkspaces
        {
            get => _filteredWorkspaces;
            set => SetProperty(ref _filteredWorkspaces, value);
        }

        private WorkspaceItemViewModel? _selected;
        public WorkspaceItemViewModel? Selected
        {
            get => _selected;
            set 
            {
                if (SetProperty(ref _selected, value))
                {
                    (DeleteWorkspaceCommand as RelayCommand)?.RaiseCanExecuteChanged();
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

        public ICommand AddWorkspaceCommand { get; }
        public ICommand DeleteWorkspaceCommand { get; }
        public ICommand RenameWorkspaceCommand { get; }

        public WorkspaceListViewModel(IWorkspaceService workspaceService, int userId = 1)
        {
            _workspaceService = workspaceService;
            _userId = userId;

            AddWorkspaceCommand = new RelayCommand(async () => await AddWorkspaceAsync());
            DeleteWorkspaceCommand = new RelayCommand(async () => await DeleteSelectedAsync(), () => Selected != null);
            RenameWorkspaceCommand = new RelayCommand(async (workspace) => 
            {
                if (workspace is WorkspaceItemViewModel ws)
                    await RenameWorkspaceAsync(ws);
            });
        }

        public async Task LoadAsync()
        {
            Workspaces.Clear();
            var list = await _workspaceService.GetAllAsync(_userId);
            foreach (var ws in list)
                Workspaces.Add(new WorkspaceItemViewModel(ws));
            
            // If no workspaces exist, create a default one
            if (Workspaces.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine("No workspaces found, creating default workspace");
                var defaultWs = await _workspaceService.CreateAsync("My Workspace", _userId);
                Workspaces.Add(new WorkspaceItemViewModel(defaultWs));
            }
            
            ApplySearch();
            if (FilteredWorkspaces.Count > 0)
                Selected = FilteredWorkspaces[0];
                
            System.Diagnostics.Debug.WriteLine($"Loaded {Workspaces.Count} workspaces");
        }

        private async Task AddWorkspaceAsync()
        {
            var ws = await _workspaceService.CreateAsync("New Workspace", _userId);
            Workspaces.Insert(0, new WorkspaceItemViewModel(ws));
            ApplySearch();
            Selected = FilteredWorkspaces.Count > 0 ? FilteredWorkspaces[0] : null;
        }

        private async Task DeleteSelectedAsync()
        {
            System.Diagnostics.Debug.WriteLine($"DeleteSelectedAsync called. Selected: {Selected?.Name}");
            
            // Prevent multiple simultaneous delete operations
            if (_isDeleting)
            {
                System.Diagnostics.Debug.WriteLine("Delete operation already in progress, ignoring");
                return;
            }
            
            if (Selected == null) 
            {
                System.Diagnostics.Debug.WriteLine("No workspace selected for deletion");
                return;
            }
            
            _isDeleting = true;
            
            try
            {
                // Show confirmation dialog
                var result = System.Windows.MessageBox.Show(
                    $"Are you sure you want to delete workspace '{Selected.Name}'? This action cannot be undone.",
                    "Confirm Delete",
                    System.Windows.MessageBoxButton.YesNo,
                    System.Windows.MessageBoxImage.Warning);
                    
                if (result != System.Windows.MessageBoxResult.Yes)
                {
                    System.Diagnostics.Debug.WriteLine("User cancelled deletion");
                    return;
                }
                
                System.Diagnostics.Debug.WriteLine($"Deleting workspace: {Selected.Name} (ID: {Selected.WorkspaceId})");
                await _workspaceService.DeleteAsync(Selected.WorkspaceId);
                Workspaces.Remove(Selected);
                ApplySearch();
                Selected = FilteredWorkspaces.Count > 0 ? FilteredWorkspaces[0] : null;
                System.Diagnostics.Debug.WriteLine($"Workspace deleted successfully. Remaining workspaces: {FilteredWorkspaces.Count}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error deleting workspace: {ex.Message}");
                System.Windows.MessageBox.Show(
                    $"Error deleting workspace: {ex.Message}",
                    "Delete Error",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
            }
            finally
            {
                _isDeleting = false;
            }
        }

        private async Task RenameWorkspaceAsync(WorkspaceItemViewModel workspace)
        {
            System.Diagnostics.Debug.WriteLine($"RenameWorkspaceAsync called for: {workspace?.Name}");
            
            if (workspace == null) 
            {
                System.Diagnostics.Debug.WriteLine("Workspace is null!");
                return;
            }
            
            // Enter editing mode
            workspace.IsEditing = true;
            System.Diagnostics.Debug.WriteLine($"IsEditing set to: {workspace.IsEditing}");
            
            // The actual rename will be handled by the TextBox LostFocus event
            // This method just triggers the editing mode
        }

        public async Task SaveRenameAsync(WorkspaceItemViewModel workspace, string newName)
        {
            if (workspace == null || string.IsNullOrWhiteSpace(newName)) return;
            
            try
            {
                await _workspaceService.RenameAsync(workspace.WorkspaceId, newName.Trim());
                workspace.Name = newName.Trim();
                workspace.IsEditing = false;
                ApplySearch(); // Reapply search after rename
            }
            catch (Exception ex)
            {
                // Handle error - you might want to show a message to user
                System.Diagnostics.Debug.WriteLine($"Error renaming workspace: {ex.Message}");
                workspace.IsEditing = false;
            }
        }

        private void ApplySearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                FilteredWorkspaces = new ObservableCollection<WorkspaceItemViewModel>(Workspaces);
            }
            else
            {
                FilteredWorkspaces = new ObservableCollection<WorkspaceItemViewModel>(
                    Workspaces.Where(w => w.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                );
            }
        }
    }
}
