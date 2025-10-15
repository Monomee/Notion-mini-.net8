using Microsoft.EntityFrameworkCore;
using NotionMini.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionMini.Services
{
    public class PageService : IPageService
    {
        public async Task<List<Page>> GetByWorkspaceAsync(int workspaceId, string? search = null)
        {
            using var db = new NoteHubDbContext();
            var query = db.Pages
                .Where(p => p.WorkspaceId == workspaceId);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.ToLower();
                query = query.Where(p => p.Title.ToLower().Contains(lower));
            }

            // Sort: pinned desc -> updated desc -> title asc
            return await query
                .OrderByDescending(p => p.IsPinned ?? false)
                .ThenByDescending(p => p.UpdatedAt ?? p.CreatedAt)
                .ThenBy(p => p.Title)
                .ToListAsync();
        }

        public async Task<Page?> GetByIdAsync(int id)
        {
            using var db = new NoteHubDbContext();
            return await db.Pages.FindAsync(id);
        }

        public async Task<Page> CreateAsync(int workspaceId, string title)
        {
            using var db = new NoteHubDbContext();
            var page = new Page
            {
                Title = string.IsNullOrWhiteSpace(title) ? "Untitled" : title.Trim(),
                WorkspaceId = workspaceId,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsPinned = false
            };
            db.Pages.Add(page);
            await db.SaveChangesAsync();
            return page;
        }

        public async Task UpdateAsync(Page page)
        {
            using var db = new NoteHubDbContext();
            var current = await db.Pages.FindAsync(page.PageId);
            if (current == null) return;

            current.Title = page.Title;
            current.Content = page.Content;
            current.IsPinned = page.IsPinned;
            current.UpdatedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int pageId)
        {
            using var db = new NoteHubDbContext();
            var p = await db.Pages.FindAsync(pageId);
            if (p == null) return;
            db.Pages.Remove(p);
            await db.SaveChangesAsync();
        }

        public async Task PinAsync(int pageId, bool pinned)
        {
            using var db = new NoteHubDbContext();
            var p = await db.Pages.FindAsync(pageId);
            if (p == null) return;
            p.IsPinned = pinned;
            p.UpdatedAt = DateTime.Now;
            await db.SaveChangesAsync();
        }
    }
}
