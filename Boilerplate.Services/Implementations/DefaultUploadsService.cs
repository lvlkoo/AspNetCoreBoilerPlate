using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Boilerplate.DAL;
using Boilerplate.DAL.Entities;
using Boilerplate.Services.Abstractions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Boilerplate.Services.Implementations
{
    public class DefaultUploadsService: BaseDataService, IUploadsService
    {
        private readonly IAuthService _authService;
        private readonly IHostingEnvironment _hostingEnvironment;

        public DefaultUploadsService(IHostingEnvironment hostingEnvironment, IAuthService authService,
            ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
            _authService = authService;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IEnumerable<Guid>> CreateUpload(IEnumerable<IFormFile> files)
        {
            var currentUserId = _authService.GetAuthorizedUserId();

            var uploads = new List<FileUpload>();

            foreach (var file in files)
            {
                if (file.Length == 0)
                    continue;

                var uploadsPath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                var fileId = Guid.NewGuid();
                var filePath = Path.Combine(uploadsPath, fileId.ToString());

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uploads.Add(new FileUpload
                {
                    Id = fileId,
                    FileName = file.FileName,
                    FilePath = filePath,
                    FileLength = file.Length,
                    ContentType = file.ContentType,
                    UploaderId = currentUserId
                });
            }

            DbContext.FileUploads.AddRange(uploads);
            await DbContext.SaveChangesAsync();

            return uploads.Select(_ => _.Id);
        }
    }
}
