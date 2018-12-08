using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DashDocs.Models;
using Microsoft.Azure.Documents.Linq;

namespace DashDocs.Services
{
    public class DocumentDbService
    {
        private static readonly DocumentClient _documentClient;
        private static readonly string __database = "DashDocsComments";
        private static readonly string _collection = "comments";

        static DocumentDbService()
        {
            var uri = ConfigurationManager.AppSettings["DocumentDb:Uri"].ToString();
            var key = ConfigurationManager.AppSettings["DocumentDb:Key"].ToString();
            _documentClient = new DocumentClient(new Uri(uri), key);

        }

        public async Task CreateCommentAsync(Comments comment)
        {
            comment.CommentDateTime = GetCommentDateTime();
            await _documentClient.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(__database, _collection), comment);
        }

        public async Task<List<Comments>> GetCommentsAsync(Guid documentId, Guid customerId)
        {
            var query = _documentClient.CreateDocumentQuery<Comments>(UriFactory.CreateDocumentCollectionUri(__database, _collection))
                .Where(c => c.DocumentId == documentId && c.CustomerId == customerId).OrderByDescending(c => c.CommentDateTime.Epoch).AsDocumentQuery();

            var comments = new List<Comments>();
            while (query.HasMoreResults)
            {
                comments.AddRange(await query.ExecuteNextAsync<Comments>());
            }
            return comments;
        }

        private CommentDateTime GetCommentDateTime()
        {
            var datetime = DateTime.UtcNow;
            return new CommentDateTime
            {
                DateStamp = datetime,
                Epoch = (int)((datetime - new DateTime(1987, 8, 8)).TotalSeconds)
            };

        }
    }
}