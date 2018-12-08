using DashDocs.ViewModels;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DashDocs.Services
{
    public class RedisService
    {
        private static Lazy<ConnectionMultiplexer> _lazyConnection;

        private static ConnectionMultiplexer cacheConnection
        {
            get
            {
                return _lazyConnection.Value;
            }
        }

        static RedisService()
        {
            _lazyConnection = new Lazy<ConnectionMultiplexer>(() =>
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager
                    .ConnectionStrings["Redis"].ConnectionString);
            });
        }

        private readonly IDatabase _cache;

        public RedisService()
        {
            _cache = cacheConnection.GetDatabase();
        }

        public async Task UpdateDocumentCacheAsync(Guid customerId, DocumentViewModel document)
        {
            var customerKey = customerId.ToString();
            var documentJson = JsonConvert.SerializeObject(document);

            // this will insert the element at the head of the list
            // If the list does not exist for the specified key
            // a new empty list will be created
            await _cache.ListLeftPushAsync(customerKey, documentJson);
            // here we do the trimming of the list - A Redis operation
            // 0-9 elements to keep from the head: keeps 10 documents
            _cache.ListTrimAsync(customerKey, 0, 9, CommandFlags.FireAndForget);
        }

        public async Task<List<DocumentViewModel>> GetRecentDocumentsForCustomerAsync(Guid customerId)
        {
            var customerKey = customerId.ToString().ToString();
            var documentJsons = await _cache.ListRangeAsync(customerKey, 0, -1, CommandFlags.None);

            var documents = new List<DocumentViewModel>();
            foreach(var json in documentJsons)
            {
                documents.Add(JsonConvert.DeserializeObject<DocumentViewModel>(json));

            }
            return documents;
        }
    }
}