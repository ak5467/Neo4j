using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WebApplicationNeo4j
{
    public class GraphConfig
    {
        public static IGraphClient GraphClient { get; private set; }

        public static IGraphClient ConfigGraph()
        {
            //Use an IoC container and register as a Singleton
            var url = ConfigurationManager.AppSettings["GraphDBUrl"];
            var user = ConfigurationManager.AppSettings["GraphDBUser"];
            var password = ConfigurationManager.AppSettings["GraphDBPassword"];
            var client = new GraphClient(new Uri(url), user, password);
            client.Connect();

            GraphClient = client;
            return GraphClient;
        }
    }
}