using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Azure.Cosmos.Table;

namespace AsureTableStorage1
{
    public class Item2 : TableEntity
    {
        private double[] myDoubles;
        public double[] MyDoubles
        {
            set
            {
                myDoubles = value;
                if (myDoubles != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach(double d in value)
                    {
                        sb.Append(d);
                        sb.Append(';');
                    }
                    myDoubleString = sb.ToString().TrimEnd(';');
                }
                else
                {
                    myDoubleString = null;
                }
            }
            get { return myDoubles; }
        }
            
        private string myDoubleString;
        public string MyDoubleString
        {
            set
            {
                myDoubleString = value;
                if(!string.IsNullOrEmpty(MyDoubleString))
                {
                    string[] doubleStr = myDoubleString.Split(';');
                    myDoubles = new double[doubleStr.Length];
                    for(int i = 0; i < doubleStr.Length; i++)
                    {
                        myDoubles[i] = Convert.ToDouble(doubleStr[i]);
                    }
                }
                else
                {
                    myDoubles = new double[0];
                }
            }
            get { return myDoubleString; }
        }


        public Item2() { }

        
        public Item2(double[] myDoubles)
        {
            this.MyDoubles = myDoubles;
            this.PartitionKey = "DoubleArray";
            this.RowKey = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + new Random().Next(1000000)).ToString();

        }
    }
}
