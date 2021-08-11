using System;
using Microsoft.WindowsAzure.Storage.Table;
//using Microsoft.WindowsAzure.Storage.Table;

namespace Camera2Basic
{
    public class Locker : TableEntity
    {
        public int Id { get; set; }
        public Boolean available { get; set; } = true;
        public Boolean locked { get; set; } = true;
        public Double price_per_hour { get; set; } = 0;
        public string user_key { get; set; } = "";
        public System.DateTimeOffset release_time { get; set; }

    }
}