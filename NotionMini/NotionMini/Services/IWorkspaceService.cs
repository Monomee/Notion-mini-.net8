using NotionMini.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionMini.Services
{
    public interface IWorkspaceService
    {
        Task<List<Workspace>> GetAllAsync(int userId);
        Task<Workspace?> GetByIdAsync(int id);
        Task<Workspace> CreateAsync(string name, int userId);
        Task RenameAsync(int id, string newName);
        Task DeleteAsync(int id);
    }
}
