using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShopWebApp
{
    public class AdminModel : BaseViewModel
    {
        public AdminModel(string title = "") : base(title)
        {
            Table = new AdminList();
            Dict = new Dictionary<string, string>();
        }
        public string Tablename { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public Dictionary<string, string> Dict { get; set; }
        public AdminList Table { get; set; }
        public int Page { get; set; }
        public int LastPage { get; set; }


        public class AdminList
        {
            public AdminList()
            {
                Names = new List<string>();
                Path = "";
            }

            public List<string> Names { get; }
            public string Path { get; set; }
        }
    }
}
