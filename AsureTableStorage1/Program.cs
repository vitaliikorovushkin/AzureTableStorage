using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AsureTableStorage1
{
    class Program
    {
        static List<Trees> trees = null;
        static async Task Main(string[] args)
        {
            CloudTable table = await CreateOpenTable("TestTable");
            //await FillTable(table);
            //Item1 item = new Item1("AccessedSolvedComplexEntity", trees[0]);
            //Item2 item = new Item2(new double[] {3.141592653, 2.718281828459045 });
            //await AddItem<Item2>("TestTable", item);
            //ShowTableData<Item2>(await GetTableData<Item2>(table));
            //await AddItem<Trees>("TestTable", trees[0]);
            await AddManyItems<Trees>("TestTable", trees);
            //ShowTableData<Trees>(await GetTableData<Trees>(table));
            //await UpdateTableData<Trees>(table, "deciduous", "1639679168677");
            //await RemoveEntity<Item1>(table, "AccessedSolvedComplexEntity", "1640109431431");
        }
        //"deciduous" 1639679168677 12

        static async Task<CloudTable> CreateOpenTable(string tableName)
        {
            var configuration = new ConfigurationBuilder()
            .AddJsonFile("config.json")
            .Build();
            string connectionString = configuration["StorageConnectionString"];
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = account.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference(tableName);
            await table.CreateIfNotExistsAsync();
            return table;
        }

        static async Task FillTable(CloudTable table)
        {
            trees = new List<Trees>();
            
            //trees.Add(new Trees("pine", 120, 30, "coniferous"));
            //trees.Add(new Trees("fir", 15, 45, "coniferous"));
            trees.Add(new Trees("acacia", 50, 20, "deciduous"));
            trees.Add(new Trees("baobab", 360, 40, "deciduous"));
            trees.Add(new Trees("beech", 100, 15, "deciduous"));
            trees.Add(new Trees("walnut", 120, 10, "deciduous"));
            trees.Add(new Trees("oak", 250, 33, "deciduous"));
            //trees.Add(new Trees("spruce", 50, 27, "coniferous"));
            trees.Add(new Trees("cedar", 110, 41, "deciduous"));
            trees.Add(new Trees("maple", 69, 15, "deciduous"));
            trees.Add(new Trees("palm", 15, 16, "deciduous"));
            trees.Add(new Trees("poplar", 70, 37, "deciduous"));
            trees.Add(new Trees("sequoia", 300, 50, "deciduous"));
            //trees.Add(new Trees("pine", 115, 31, "coniferous"));
            //trees.Add(new Trees("fir", 14, 40, "coniferous"));
            trees.Add(new Trees("acacia", 36, 22, "deciduous"));
            trees.Add(new Trees("baobab", 330, 28, "deciduous"));
            trees.Add(new Trees("beech", 70, 19, "deciduous"));
            trees.Add(new Trees("walnut", 99, 13, "deciduous"));
            trees.Add(new Trees("oak", 200, 26, "deciduous"));
            //trees.Add(new Trees("spruce", 56, 30, "coniferous"));
            trees.Add(new Trees("cedar", 90, 36, "deciduous"));
            trees.Add(new Trees("maple", 80, 21, "deciduous"));
            trees.Add(new Trees("palm", 10, 19, "deciduous"));
            trees.Add(new Trees("poplar", 75, 45, "deciduous"));
            trees.Add(new Trees("sequoia", 270, 49, "deciduous"));
        }

        private static async Task AddItem<T>(string tableName, T item) where T:ITableEntity
        {
            CloudTable table = await CreateOpenTable(tableName);
            TableOperation insert = TableOperation.Insert(item);
            await table.ExecuteAsync(insert);

        }
        private static async Task AddManyItems<T>(string tableName, List<T> items) where T : ITableEntity
        {
            CloudTable table = await CreateOpenTable(tableName);
            TableBatchOperation bo = new TableBatchOperation();
            items.ForEach(x => bo.Insert(x));
            await table.ExecuteBatchAsync(bo);
            Console.WriteLine("New items added!");
        }

        private static async Task<List<T>> GetTableData<T>(CloudTable table) where T : ITableEntity, new()
        {
            List<T> list = new List<T>();
            //version 1
            TableQuery<T> query = new TableQuery<T>();
            //version 2
            //TableQuery<T> query = new TableQuery<T>()
            //    .Where(TableQuery.GenerateFilterConditionForInt("Age", QueryComparisons.LessThan, 50));
            //version 3
            /*TableQuery<T> query = new TableQuery<T>()
               .Where(TableQuery.CombineFilters(
                   TableQuery.GenerateFilterConditionForInt("Age", QueryComparisons.LessThan, 50),
                   TableOperators.And,
                   TableQuery.GenerateFilterCondition("TreeName", QueryComparisons.Equal, "palm")
                        )

                   );
            */
            TableQuerySegment<T> segment = null;
            do
            {
                segment = await table.ExecuteQuerySegmentedAsync(query, segment?.ContinuationToken);
                list.AddRange(segment.Results);

            } while (segment.ContinuationToken != null);

            return list;
        }


        private static void ShowTableData<T>(List<T> list) where T : ITableEntity, new()
        {
            foreach(T item in list)
            {
                if (item is Trees)
                {
                    Console.WriteLine((item as Trees).TreeName + ", " + (item as Trees).TreeType 
                        + ", "+ (item as Trees).Age);
                }
               else if(item is Item1) 
                {
                    Console.WriteLine((item as Item1).Name + ", " + (item as Item1).StringTree
                        + ", " + (item as Item1).Tree.Age + ", " + (item as Item1).Tree.Height);
                }
                else if (item is Item2)
                {
                    Console.WriteLine( string.Join(";", (item as Item2).MyDoubleString) );
                }
                else
                {
                    Console.WriteLine(item.PartitionKey + ", " + item.RowKey);
                }
            }
        }

        //"deciduous" 1639679168677 12
        private static async Task UpdateTableData<T>(CloudTable table, string pk, string rk) where T : ITableEntity
        {
            TableOperation find = TableOperation.Retrieve<T>(pk, rk);
            TableResult result = await table.ExecuteAsync(find);
            T item = (T)result.Result;

            if (item != null && item is Trees)
            {
                (item as Trees).Height = 12;
                TableOperation update = TableOperation.Replace(item);
                await table.ExecuteAsync(update);
            }
        }

        private static async Task RemoveEntity<T>(CloudTable table, string pk, string rk) where T : ITableEntity, new()
        {
            try
            {
                TableOperation find = TableOperation.Retrieve<T>(pk, rk);
                TableResult result = await table.ExecuteAsync(find);
                T item = (T)result.Result;
                if (item != null)
                {
                    TableOperation delteOperation = TableOperation.Delete(item);
                    table.Execute(delteOperation);
                }
            }
            catch (Exception ex)
            {



            }
        }

    }
}
