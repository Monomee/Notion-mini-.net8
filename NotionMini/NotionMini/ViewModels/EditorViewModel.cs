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
        private Timer? _debounceTimer;
        private readonly int _debounceMs = 2000; // 2s delay
        private Page? _currentPage;

        private int _pageId;
        private string _title = string.Empty;
        private string? _content;
        private bool _isDirty;
        private DateTime? _lastSavedAt;

        public int PageId
        {
            get => _pageId;
            private set => SetProperty(ref _pageId, value);
        }

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

        public bool IsDirty
        {
            get => _isDirty;
            private set => SetProperty(ref _isDirty, value);
        }

        public DateTime? LastSavedAt
        {
            get => _lastSavedAt;
            private set => SetProperty(ref _lastSavedAt, value);
        }

        public EditorViewModel(IPageService pageService)
        {
            _pageService = pageService;
        }

        public void LoadPage(Page page)
        {
            _currentPage = page;
            PageId = page.PageId;
            Title = page.Title;
            Content = page.Content;
            LastSavedAt = page.UpdatedAt ?? page.CreatedAt ?? DateTime.Now;
            IsDirty = false;
        }

        private void ScheduleSave()
        {
            if (_currentPage == null) return;

            IsDirty = true;
            _debounceTimer?.Dispose();
            _debounceTimer = new Timer(async _ => await SaveAsync(),
                                       null,
                                       _debounceMs,
                                       Timeout.Infinite);
        }

        private async Task SaveAsync()
        {
            try
            {
                if (_currentPage == null) return;

                _currentPage.Title = string.IsNullOrWhiteSpace(Title) ? "Untitled" : Title.Trim();
                _currentPage.Content = Content;
                _currentPage.UpdatedAt = DateTime.Now;

                await _pageService.UpdateAsync(_currentPage);

                IsDirty = false;
                LastSavedAt = _currentPage.UpdatedAt.Value;
                Raise(nameof(LastSavedAt));
            }
            catch
            {
                // Có thể log hoặc hiển thị thông báo nếu cần
                IsDirty = true;
            }
        }
        public async Task LoadAsync(int pageId)
        {
            var p = await _pageService.GetByIdAsync(pageId);
            if (p != null)
            {
                _currentPage = p;
                PageId = p.PageId;
                Title = p.Title;
                Content = p.Content;
                LastSavedAt = p.UpdatedAt ?? p.CreatedAt ?? DateTime.Now;
                IsDirty = false;
            }
        }

    }
}
