using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Boilerplate.Services.Abstractions
{
    public interface IUploadsService
    {
        Task<IEnumerable<Guid>> CreateUpload(IEnumerable<IFormFile> files);
    }
}
