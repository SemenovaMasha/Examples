using System;
using System.Collections.Generic;
using System.Text;

namespace EF_vs_Dapper_vs_Hibernate
{
    public class Product
    {
        
        public virtual int ID { get; set; }
        public virtual string Name { get; set; }
        public virtual int Price { get; set; }
    }
}
