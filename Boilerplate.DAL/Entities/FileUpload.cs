using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Boilerplate.DAL.Entities
{
    public class FileUpload
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string ContentType { get; set; }
        public long FileLength { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.Now;
        public Guid UploaderId { get; set; }
        
        public ApplicationUser Uploader { get; set; }
    }
}