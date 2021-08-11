using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;
//using Microsoft.WindowsAzure.Storage.Table;

namespace CounterFunctions
{
    public class Locker : TableEntity
    {
        public int Id { get; set; }
        public Boolean available { get; set; } = true;
        public Boolean locked { get; set; } = true;
        public Boolean photo_taken { get; set; } = false;
        public System.DateTimeOffset release_time { get; set; }
        public Double price_per_hour { get; set; } = 0;
        public string user_key { get; set; } = "";
    }
}
