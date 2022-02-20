using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsureTableStorage1
{
    public class Trees : TableEntity {
        public Trees(string treeName, int age, int height, string treeType)
        {
            TreeName = treeName;
            Age = age;
            Height = height;
            TreeType = treeType;

            this.PartitionKey = TreeType;
            this.RowKey = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + new Random().Next(1000000)).ToString();
        }

        public Trees() { }
        public string TreeName { get; set; }
        public int Age { get; set; }
        public int Height { get; set; }
        public string TreeType { get; set; }
    }
}
