using DashDocs.Models;
using DashDocs.Services;
using DashDocs.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    // [Authorize]
    public class HomeController : DashDocsControllerBase
    {
        public async Task<ActionResult> Index()
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");

            var redisService = new RedisService();
            var collection = await redisService.GetRecentDocumentsForCustomerAsync(customerId);
            return View(collection);

          
        }

        public async Task<ActionResult> Upload(HttpPostedFileBase document)
        {
            // Ids used in the seed method
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");
            Guid userId = Guid.Parse("2A37108E-56AF-4C18-99C4-415191591CD9");

            var blobStorageService = new BlobStorageService();
            var documentId = Guid.NewGuid();

            var path = await blobStorageService.UploadDocumentAsync(document, customerId, documentId);

            var dbContext = new DashDocsContext();

            var documentModel = new Document
            {
                Id = documentId,
                DocumentName = Path.GetFileName(document.FileName).ToLower(),
                OwnerId = userId,
                CreatedOn = DateTime.UtcNow,
                BlobPath = path
            };

            dbContext.Documents.Add(documentModel);
            await dbContext.SaveChangesAsync();

            var doc = new DocumentViewModel
            {
                DocumentId = documentModel.Id,
                Owner = customerId.ToString(),
                CreatedOn = documentModel.CreatedOn,
                DocumentName = documentModel.DocumentName,
            };

            var redisService = new RedisService();
            await redisService.UpdateDocumentCacheAsync(customerId, doc);

            return RedirectToAction("Index");
        }

        public async Task<FileResult> Download(Guid documentId)
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");

            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var blobStorageService = new BlobStorageService();
            var content = await blobStorageService.DownloadDocument(documentId, customerId);

            return File(content.Item1, System.Net.Mime.MediaTypeNames.Application.Octet, content.Item2);
        }

        public ActionResult About()
        {
            ViewBag.Message = "DashDocs Application";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

    
    }
}