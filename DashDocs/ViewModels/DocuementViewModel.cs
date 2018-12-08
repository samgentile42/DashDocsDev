using System;

namespace DashDocs.ViewModels
{
    public class DocumentViewModel
    {
        public Guid DocumentId { get; set; }
        public string DocumentName { get; set; }
        public string Owner { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}