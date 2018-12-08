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
    public class SearchController : DashDocsControllerBase
    {
        public async Task<ActionResult> Index(string search)
        {
            Guid customerId = Guid.Parse("82CEAD1F-E3FA-4DAB-BFFA-276845FB7E72");
            var documents = new List<DocumentIndex>();
            if (Request.QueryString["search"] != null)
            {
                var searchService = new SearchService();
                documents = await searchService.SearchAsync(search, customerId);

            }
            return View(documents);
        }
    }
}