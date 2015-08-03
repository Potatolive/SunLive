using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using Sunlive.Entities;

namespace Test
{
    class Program
    {
        

        static void Main(string[] args)
        {
            string connectionString = "mongodb://52.10.10.147:27017";
            MongoClient client = new MongoClient(connectionString);
            IMongoDatabase myDB = client.GetDatabase("Testing");

            dailyMessage(myDB, "2015-06-16", "2015-07-16");
            dailyMessage5MinBucket(myDB, "2015-06-16", "2015-07-16");
            dailyUser(myDB, "2015-06-16", "2015-07-16");
        }

        static void dailyMessage(IMongoDatabase myDB, string startDate, string endDate)
        {
            var coll = myDB.GetCollection<FanPost>("fanposts");

            var result = coll.Aggregate()
                .Match("{ PublishedOn: { $gt: \"" + startDate + "\", $lte: \"" + endDate + "\" } }")
                .Project("{_id : \"$_id\", dt : { $substr: [\"$PublishedOn\", 0, 10] } }")
                .Group("{ _id: \"$dt\", messages: { $sum: 1 } }")
                .Sort("{ _id : 1 }");

            var list = result.ToListAsync();

            foreach (var item in list.Result)
            {
                Console.WriteLine(item.ToString());
            }
        }

        static void dailyUser(IMongoDatabase myDB, string startDate, string endDate)
        {
            var coll = myDB.GetCollection<FanPost>("fanposts");

            var result = coll.Aggregate()
                .Match("{ PublishedOn: { $gt: \"" + startDate + "\", $lte: \"" + endDate + "\" } }")
                .Project("{_id : \"$_id\",user: \"$PublishedBy\",dt : { $substr: [\"$PublishedOn\", 0, 10] }}")
                .Group("{_id : { dt: \"$dt\", user: \"$user\" },count: { $sum: 1 }}")
                .Group("{_id: { dt: \"$_id.dt\" },uniqueUsers : { $sum: 1 }}")
                .Sort("{ _id : 1 }");

            var list = result.ToListAsync();

            foreach (var item in list.Result)
            {
                Console.WriteLine(item.ToString());
            }
        }

        static void dailyMessage5MinBucket(IMongoDatabase myDB, string startDate, string endDate)
        {
            var coll = myDB.GetCollection<FanPost>("fanposts");

            var result = coll.Aggregate()
                .Match("{ PublishedOnDateTime: { $gt: ISODate(\"" + startDate + "\"), $lte: ISODate(\"" + endDate + "\") } }")
                .Project("{_id : \"$_id\",dt : { $substr: [\"$PublishedOnDateTime\", 0, 13] },bucket: \"$bucket12\"}")
                .Group("{_id: { dt: \"$dt\", bucket: \"$bucket\"},messages: { $sum: 1 }}")
                .Sort("{ _id : 1 }");

            var list = result.ToListAsync();

            foreach (var item in list.Result)
            {
                Console.WriteLine(item.ToString());
            }
        }
    }
}
