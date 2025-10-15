using NotionMini.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotionMini.Models;

namespace NotionMini.ViewModels
{
    public class EditorViewModel: BaseViewModel
    {
        private readonly IPageService _pageService;

        private int _pageId;
        private string _title = string.Empty;
        private string? _content;
        private bool _isDirty;
        private DateTime? _lastSavedAt;

        // debounce
        private Timer? _timer;
        private readonly int _debounceMs = 1500;

        public int PageId { get => _pageId; private set => SetProperty(ref _pageId, value); }
        public string Title
        {
            get => _title;
            set
            {
                if (SetProperty(ref _title, value))
                    ScheduleSave();
            }
        }
        public string? Content
        {
            get => _content;
            set
            {
                if (SetProperty(ref _content, value))
                    ScheduleSave();
            }
        }
        public bool IsDirty { get => _isDirty; private set => SetProperty(ref _isDirty, value); }
        public DateTime? LastSavedAt { get => _lastSavedAt; private set => SetProperty(ref _lastSavedAt, value); }

        public EditorViewModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        public async Task LoadAsync(int pageId)
        {
            var p = await _pageService.GetByIdAsync(pageId);
            if (p == null) return;
            PageId = p.PageId;
            Title = p.Title;
            Content = p.Content;
            IsDirty = false;
            LastSavedAt = p.UpdatedAt ?? p.CreatedAt;
        }

        private void ScheduleSave()
        {
            IsDirty = true;
            _timer?.Dispose();
            _timer = new Timer(async _ => await SaveAsync(), null, _debounceMs, Timeout.Infinite);
        }

        private async Task SaveAsync()
        {
            try
            {
                var p = new Page
                {
                    PageId = PageId,
                    Title = string.IsNullOrWhiteSpace(Title) ? "Untitled" : Title.Trim(),
                    Content = Content,
                    IsPinned = null // giữ nguyên trong service
                };
                await _pageService.UpdateAsync(p);
                IsDirty = false;
                LastSavedAt = DateTime.Now;
                Raise(nameof(LastSavedAt));
            }
            catch
            {
                // ở bản skeleton ta bỏ qua toast/log, sẽ thêm ở polish phase
                IsDirty = true;
            }
        }
    }
}
