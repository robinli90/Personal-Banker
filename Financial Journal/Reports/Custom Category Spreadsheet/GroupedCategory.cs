using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public enum GroupedType
    {
        Category,
        Expense
    }

    public class GroupedCategory
    {
        public string _ProfileName { get; set; }
        public string _GroupName { get; set; }
        public List<string> SubCategoryList { get; set; }
        public List<string> SubExpenseList { get; set; }

        public GroupedCategory(string ProfileName, string GroupName)
        {
            _ProfileName = ProfileName;
            _GroupName = GroupName;
            SubCategoryList = new List<string>();
            SubExpenseList = new List<string>();
        }
    }
}
