using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
//using Microsoft.WindowsAzure.Storage.Table;

namespace CounterFunctions
{
    public class UserBalance : TableEntity
    {

        public string user_key { get; set; } = "";
        public double balance { get; set; } = 0;
       
    }
}
