using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Cosmos.Table;

namespace CounterFunctions
{
    class Photo : TableEntity
    {
        public int Id
        {
            get; set;
        }
        public Byte[] base64 { get; set; } = { };
    }
}
