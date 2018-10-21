using System.Collections.Generic;

namespace Models
{
    public class OrderData
    {
        public IList<ProductData> Products { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public int Number { get; set; }
    }
}
