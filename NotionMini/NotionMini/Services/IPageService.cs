using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotionMini.Models;

namespace NotionMini.Services
{
    public interface IPageService
    {
        Task<List<Page>> GetByWorkspaceAsync(int workspaceId, string? search = null);
        Task<Page?> GetByIdAsync(int id);
        Task<Page> CreateAsync(int workspaceId, string title);
        Task UpdateAsync(Page page);
        Task DeleteAsync(int pageId);
        Task PinAsync(int pageId, bool pinned);
    }
}
