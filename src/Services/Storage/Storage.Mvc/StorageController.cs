using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Riverside.Cms.Services.Storage.Domain;
using Riverside.Cms.Services.Core.Common;

namespace Riverside.Cms.Services.Storage.Mvc
{
    [MultiTenant()]
    public class StorageController : ControllerBase
    {
        private readonly IStorageService _storageService;

        public StorageController(IStorageService storageService)
        {
            _storageService = storageService;
        }

        private long TenantId => (long)RouteData.Values["tenantId"];

        private bool PostIsCreateAction(IFormFile file, long? sourceBlobId, ResizeMode? mode, int? width, int? height)
        {
            return file != null && sourceBlobId == null && mode == null && width == null && height == null;
        }

        private bool PostIsResizeAction(IFormFile file, long? sourceBlobId, ResizeMode? mode, int? width, int? height)
        {
            return file == null && sourceBlobId != null && mode != null && width != null && height != null;
        }

        private async Task<IActionResult> CreateBlobWorkAsync(long tenantId, string path, IFormFile file)
        {
            Blob blob = new Blob
            {
                TenantId = tenantId,
                ContentType = file.ContentType,
                Name = file.FileName,
                Path = path
            };
            Stream stream = file.OpenReadStream();
            long blobId = await _storageService.CreateBlobAsync(tenantId, blob, stream);
            return CreatedAtAction(nameof(ReadBlobAsync), new { tenantId, blobId }, null);
        }

        private async Task<IActionResult> ResizeBlobAsync(long tenantId, string path, long sourceBlobId, ResizeMode mode, int width, int height)
        {
            ResizeOptions options = new ResizeOptions
            {
                Width = width,
                Height = height,
                Mode = mode
            };
            long destinationBlobId = await _storageService.ResizeBlobAsync(tenantId, sourceBlobId, path, options);
            return CreatedAtAction(nameof(ReadBlobAsync), new { tenantId, blobId = destinationBlobId }, null);
        }

        [HttpGet]
        [Route("api/v1/storage/blobs")]
        [ProducesResponseType(typeof(IEnumerable<Blob>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ListBlobsAsync([FromQuery]string blobIds, [FromQuery]string path)
        {
            IEnumerable<Blob> blobs = null;
            if (blobIds != null)
            {
                IEnumerable<long> blobIdCollection = !string.IsNullOrWhiteSpace(blobIds) ? blobIds.Split(',').Select(long.Parse) : null;
                blobs = await _storageService.ListBlobsAsync(TenantId, blobIdCollection);
            }
            else
            {
                blobs = await _storageService.SearchBlobsAsync(TenantId, path);
            }
            return Ok(blobs);
        }

        [HttpGet]
        [Route("api/v1/storage/blobs/{blobId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Blob), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> ReadBlobAsync(long blobId)
        {
            Blob blob = await _storageService.ReadBlobAsync(TenantId, blobId);
            if (blob == null)
                return NotFound();
            return Ok(blob);
        }

        [HttpGet]
        [Route("api/v1/storage/blobs/{blobId:int}/content")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReadBlobContentAsync(long blobId, [FromQuery(Name = "path")]string path)
        {
            BlobContent blobContent = await _storageService.ReadBlobContentAsync(TenantId, blobId, path);
            if (blobContent == null)
                return NotFound();
            return File(blobContent.Stream, blobContent.Type, blobContent.Name);
        }

        [HttpPost]
        [Route("api/v1/storage/blobs")]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> CreateBlobAsync(string path, IFormFile file, [FromQuery(Name = "source")]long? sourceBlobId, [FromQuery(Name = "resize")]ResizeMode? mode, [FromQuery]int? width, [FromQuery]int? height)
        {
            bool create = PostIsCreateAction(file, sourceBlobId, mode, width, height);
            bool resize = PostIsResizeAction(file, sourceBlobId, mode, width, height);
            if ((create && resize) || (!create && !resize))
                return BadRequest();

            if (create)
                return await CreateBlobWorkAsync(TenantId, path, file);
            else
                return await ResizeBlobAsync(TenantId, path, sourceBlobId.Value, mode.Value, width.Value, height.Value);
        }

        [HttpDelete]
        [Route("api/v1/storage/blobs/{blobId:int}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteBlobAsync(long blobId)
        {
            Blob blob = await _storageService.ReadBlobAsync(TenantId, blobId);
            if (blob == null)
                return NotFound();
            await _storageService.DeleteBlobAsync(TenantId, blobId);
            return NoContent();
        }
    }
}
