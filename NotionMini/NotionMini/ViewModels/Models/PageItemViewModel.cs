using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotionMini.Models;

namespace NotionMini.ViewModels.Models
{
    public class PageItemViewModel: BaseViewModel
    {
        public int PageId { get; }
        private string _title;
        private bool _isPinned;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public bool IsPinned { get => _isPinned; set => SetProperty(ref _isPinned, value); }

        public PageItemViewModel(Page p)
        {
            PageId = p.PageId;
            _title = p.Title;
            _isPinned = p.IsPinned ?? false;
        }
        public override string ToString() => Title;
    }
}
