using DashDocs.Models;
using DashDocs.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DashDocs.Controllers
{
    public class DocumentController : DashDocsControllerBase
    {
        // GET: Document
        public async Task<ActionResult> Index()
        {
            Guid documentId = Guid.Empty;

            if (Request.QueryString["documentId"] != null
                && Guid.TryParse(Request.QueryString["documentId"], out documentId))
            {
                var dbContext = new DashDocsContext();
                var document = dbContext.Documents.Single(d => d.Id == documentId);

                var documentDbContext = new DocumentDbService();
                Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");
                var comments = await documentDbContext.GetCommentsAsync(documentId, customerId);

                var result = new KeyValuePair<Document, List<Comments>>(document, comments);
                return View(result);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }




            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Comment(string content, Guid documentId)
        {
            var comment = new Comments
            {
                Content = content,
                Author = "Author",
                CustomerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72"),
                Id = Guid.NewGuid().ToString()
            };

            var documentDbContext = new DocumentDbService();
            await documentDbContext.CreateCommentAsync(comment);

            return RedirectToAction("Index", new { documentId = documentId });
        }
    }
}