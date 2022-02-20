using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace AsureTableStorage1
{
   public  class Item1 : TableEntity
   {
        public string Name { get; set; }
        private Trees tree;
        public Trees Tree {
            get
            {
                if (!string.IsNullOrEmpty(StringTree))
                {
                    return JsonSerializer.Deserialize<Trees>(StringTree);
                }
                else
                {
                    return tree;
                }
            }
            set
            {
                tree = value;
                StringTree = JsonSerializer.Serialize(tree);
            }
        }

        public string StringTree { get; set; } //copy for Tree but in JSON

        public Item1() { }

        public Item1(string name, Trees tree) 
        {
            Name = name;
            Tree = tree;
            this.PartitionKey = this.Name;
            this.RowKey = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + new Random().Next(1000000)).ToString();

        }
    }
}
