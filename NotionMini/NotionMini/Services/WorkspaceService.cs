using Microsoft.EntityFrameworkCore;
using NotionMini.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotionMini.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        public async Task<List<Workspace>> GetAllAsync(int userId)
        {
            using var db = new NoteHubDbContext();
            return await db.Workspaces
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync();
        }

        public async Task<Workspace?> GetByIdAsync(int id)
        {
            using var db = new NoteHubDbContext();
            return await db.Workspaces.FindAsync(id);
        }

        public async Task<Workspace> CreateAsync(string name, int userId)
        {
            using var db = new NoteHubDbContext();
            var ws = new Workspace { Name = name, UserId = userId };
            db.Workspaces.Add(ws);
            await db.SaveChangesAsync();
            return ws;
        }

        public async Task RenameAsync(int id, string newName)
        {
            using var db = new NoteHubDbContext();
            var ws = await db.Workspaces.FindAsync(id);
            if (ws == null) return;
            ws.Name = newName;
            await db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            using var db = new NoteHubDbContext();
            var ws = await db.Workspaces.FindAsync(id);
            if (ws == null) return;
            db.Workspaces.Remove(ws);
            await db.SaveChangesAsync();
        }
    }
}

