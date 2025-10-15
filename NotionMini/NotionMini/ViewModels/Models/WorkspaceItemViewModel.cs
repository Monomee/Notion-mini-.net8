using NotionMini.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionMini.ViewModels.Models
{
    public class WorkspaceItemViewModel
    {
        public int WorkspaceId { get; }
        public string Name { get; set; }
        public WorkspaceItemViewModel(Workspace ws)
        {
            WorkspaceId = ws.WorkspaceId;
            Name = ws.Name;
        }
        public override string ToString() => Name;
    }
}
