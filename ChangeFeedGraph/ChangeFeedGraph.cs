
namespace ChangeFeedFunctions
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Host;
    using Newtonsoft.Json;

    /// <summary>
    /// Processes events using Cosmos DB Change Feed.
    /// </summary>
    public static class ChangeFeedGraph
    {

        [FunctionName("ChangeFeedGraph")]
        public static void Run(
            //change database name below if different than specified in the lab
            [CosmosDBTrigger(databaseName: "graphdb",
            //change the collection name below if different than specified in the lab
            collectionName: "graph1",
            ConnectionStringSetting = "DBconnection",
            LeaseConnectionStringSetting = "DBconnection",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true)]IReadOnlyList<Document> documents, TraceWriter log)
        {
            var Endpoint = Environment.GetEnvironmentVariable("TargetGraphURL");
            var Key = Environment.GetEnvironmentVariable("TargetGraphKey");
            var TargetGraphDB = Environment.GetEnvironmentVariable("TargetGraphDB");
            var TargetGraph = Environment.GetEnvironmentVariable("TargetGraph");

            DocumentClient client;
            client = new DocumentClient(new Uri(Endpoint), Key, new ConnectionPolicy { EnableEndpointDiscovery = false });

            // Iterate through modified documents from change feed.
            foreach (var doc in documents)
            {
                // Convert documents to json.
                string json = JsonConvert.SerializeObject(doc);
                var serialisedjson = JsonConvert.DeserializeObject<RootObject>(json);
                if (serialisedjson.active.Equals("true"))
                {
                    try
                    {
                        client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TargetGraphDB, TargetGraph), doc,
                      new RequestOptions { PartitionKey = new PartitionKey("pk") });
                    }
                    catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine("Exception: " + e);
                    }
                   
                }
                if (serialisedjson.active.Equals("false"))
                {
                    client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(TargetGraphDB, TargetGraph, doc.Id), 
                        new RequestOptions { PartitionKey = new PartitionKey("pk") });
                }
                //client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TargetGraphDB, TargetGraph, doc.Id), doc,
                //new RequestOptions { PartitionKey = new PartitionKey("pk") });
                System.Diagnostics.Debug.WriteLine("document to send notification  for: " + doc);
            }
        }
    }
    public class RootObject
    {
        public string vertex1 { get; set; }
        public string edge { get; set; }
        public string vertex2 { get; set; }
        public string Label { get; set; }
        public string Name { get; set; }
        public string active { get; set; }
        public string id { get; set; }
        public string pk { get; set; }
    }
}