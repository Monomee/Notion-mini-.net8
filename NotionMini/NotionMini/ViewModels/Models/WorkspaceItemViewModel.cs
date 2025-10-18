using NotionMini.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NotionMini.ViewModels.Models
{
    public class WorkspaceItemViewModel : INotifyPropertyChanged
    {
        public int WorkspaceId { get; }
        
        private string _name = string.Empty;
        public string Name 
        { 
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isEditing;
        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        public WorkspaceItemViewModel(Workspace ws)
        {
            WorkspaceId = ws.WorkspaceId;
            Name = ws.Name;
        }
        
        public override string ToString() => Name;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}
