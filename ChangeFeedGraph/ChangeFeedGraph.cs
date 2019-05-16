
namespace ChangeFeedFunctions
{
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Azure.Documents;
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

            // Iterate through modified documents from change feed.
            foreach (var doc in documents)
            {
                // Convert documents to json.
                string json = JsonConvert.SerializeObject(doc);

                // Use Event Hub client to send the change events to event hub.
                System.Diagnostics.Debug.WriteLine("document to send notification for: " + doc);
            }
        }
    }
}