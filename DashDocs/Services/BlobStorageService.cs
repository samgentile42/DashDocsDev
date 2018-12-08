using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Services
{
    public class BlobStorageService
    {
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<string> UploadDocumentAsync(
            HttpPostedFileBase documentFile, Guid customerId, Guid documentId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager
                .ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());
            var blobRelativePath = documentId.ToString().ToLower() + "/" +
                Path.GetFileName(documentFile.FileName).ToLower();
            var block = container.GetBlockBlobReference(blobRelativePath);
            await block.UploadFromStreamAsync(documentFile.InputStream);
            return blobRelativePath;
        }


        public async Task<Tuple<MemoryStream, string>> DownloadDocument(Guid documentId, Guid customerId)
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["DocumentStore"].ConnectionString);
            var blobClient = storageAccount.CreateCloudBlobClient();

            var container = blobClient.GetContainerReference(customerId.ToString().ToLower());

            var dbContext = new DashDocsContext();
            var document = await dbContext.Documents.SingleAsync(d => d.Id == documentId);

            var block = container.GetBlockBlobReference(document.BlobPath);

            var stream = new MemoryStream();
            await block.DownloadToStreamAsync(stream);

            stream.Position = 0;

            var content = new Tuple<MemoryStream, string>(stream, document.DocumentName);

            return content;
        }
    }
}