using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    [Flags]
    public enum IncomeMode
    {
        Automatic = 0x00,
        Manual = 0x01,
    }

    public enum BCType  // Budget Category
    {
        Categorical,
        Recurring,
        Extraneous
    }

    public enum ExportType  // Budget Category
    {
        Print,
        Preview,
        PDFCurrent,
        PDFAll,
        ExcelCurrent,
        ExcelAll
    }

    public class BudgetEntry
    {

        public int Month { get; set; }
        public int Year { get; set; }
        public IncomeMode IncomeMode { get; set; }
        public double TargetBudget { get; set; }
        private List<BudgetCategory> CategoryList { get; set; }

        public double ActualSum { get; set; } // internal use only
        public double BudgetSum { get; set; } // internal use only

        public override string ToString()
        {
            return String.Format("IncomeMode: {0}, Month: {1}, Year: {2}, Target$: {3}", IncomeMode, Month, Year,
                "$" + String.Format("{0:0.00}", TargetBudget));
        }

        public List<BudgetCategory> GetCategoryList()
        {
            return CategoryList;
        }

        public void ResetCategoryList()
        {
            CategoryList = new List<BudgetCategory>();
        }

        public BudgetEntry(int month, int year, IncomeMode incomeMode, double targetBudget)
        {
            Month = month;
            Year = year;
            IncomeMode = incomeMode;
            TargetBudget = targetBudget;
            CategoryList = new List<BudgetCategory>();
        }

        public void SetBudgetForBc(BCType bcType, string name, double amount)
        {
            BudgetCategory bCTemp = GetBudgetCategory(bcType, name);
            if (CategoryList.Contains(bCTemp))
            {
                bCTemp.TargetAmount = amount;
            }
            else
            {
                Diagnostics.WriteLine(String.Format("Adding new budget for name '{0}'", name));
                AddBudgetCategory(bcType, name, amount);
            }
        }

        public double GetBudgetAmount(BCType bcType, string name)
        {
            BudgetCategory BC = CategoryList.FirstOrDefault(x => x.GetBCType() == bcType && x.GetName().Trim() == name.Trim());
            //return BC?.TargetAmount ?? 0; // simplified from if BC == null return 0 else BC.TargetAmount
            return BC == null ? 0 : Math.Round(BC.TargetAmount, 2);
        }

        public double GetActualAmount(BCType bcType, string name)
        {
            BudgetCategory BC = CategoryList.FirstOrDefault(x => x.GetBCType() == bcType && x.GetName().Trim() == name.Trim());
            //return BC?.TargetAmount ?? 0; // simplified from if BC == null return 0 else BC.TargetAmountnull return 0 else BC.TargetAmount
            return BC == null ? 0 : Math.Round(BC.ActualAmount, 2);
        }

        public double GetBalanceAmount(BCType bcType, string name)
        {
            BudgetCategory BC = CategoryList.FirstOrDefault(x => x.GetBCType() == bcType && x.GetName().Trim() == name.Trim());
            //return BC?.TargetAmount ?? 0; // simplified from if BC == null return 0 else BC.TargetAmountnull return 0 else BC.TargetAmount
            return BC == null ? 0 : Math.Round(BC.TargetAmount - BC.ActualAmount, 2);
        }

        public BudgetCategory GetBudgetCategory(BCType bcType, string name)
        {
            return CategoryList.FirstOrDefault(x => x.GetBCType() == bcType && x.GetName() == name);
        }

        public void AddBudgetCategory(BCType bcType, string name, double targetAmount)
        {
            BudgetCategory RefBC = CategoryList.FirstOrDefault(x => x.GetBCType() == bcType && x.GetName() == name);

            if (RefBC != null) // If existing, overwrite
            {
                RefBC.TargetAmount = targetAmount;
                return;
            }

            CategoryList.Add(new BudgetCategory(bcType, name, targetAmount));
        }

        public void AddBudgetCategory(string bcType, string name, double targetAmount)
        {
            BudgetCategory RefBC = CategoryList.FirstOrDefault(x => x.GetBCType().ToString() == bcType && x.GetName() == name);

            if (RefBC != null) // If existing, overwrite
            {
                RefBC.TargetAmount = targetAmount;
                return;
            }

            CategoryList.Add(new BudgetCategory(bcType, name, targetAmount));
        }

        public void DelBudgetCategory(BCType bcType, string name)
        {
            CategoryList = CategoryList.Where(x => x.GetName() != name && x.GetBCType() != bcType).ToList();
        }
    }

    public class BudgetCategory
    {
        private BCType BcType { get; set; }
        private string Name { get; set;  }
        public double TargetAmount { get; set; }
        public double ActualAmount { get; set; } // internal use only

        public override string ToString()
        {
            return String.Format("BCType: {0}, Name: {1}, TargetAmount: {2}", BcType, Name,
                "$" + String.Format("{0:0.00}", TargetAmount));
        }

        // BCType bcType
        public BudgetCategory(BCType bcType, string name, double targetAmount = 0)
        {
            BcType = bcType;
            Name = name;
            TargetAmount = targetAmount; // preset 0
        }

        // string bcType
        public BudgetCategory(string bcType, string name, double targetAmount = 0)
        {
            BcType = bcType == "C" ? BCType.Categorical : (bcType == "R" ? BCType.Recurring : BCType.Extraneous);
            Name = name;
            TargetAmount = targetAmount; // preset 0
        }

        public string GetName()
        {
            return Name;
        }

        public BCType GetBCType()
        {
            return BcType;
        }
    }
}
