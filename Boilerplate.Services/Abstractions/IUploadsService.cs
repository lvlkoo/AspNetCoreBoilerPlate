using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Boilerplate.Entities;
using Microsoft.AspNetCore.Http;

namespace Boilerplate.Services.Abstractions
{
    public interface IUploadsService
    {
        Task<FileUpload> Get(Guid id);
        Task<IEnumerable<Guid>> CreateUpload(IEnumerable<IFormFile> files);
    }
}
