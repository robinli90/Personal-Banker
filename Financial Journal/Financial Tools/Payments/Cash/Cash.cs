using System;
using System.Collections.Generic;
using System.Linq;

namespace Objects
{
    public enum TransactionType
    {
        Deposit,
        Withdraw
    }

    public static class Cash
    {
        private static double _CurrentBalance { get; set; }
        public static List<CashHistory> _CashHistories { get; set; }

        static Cash()
        {
            _CurrentBalance = 0;
            ClearCashHistory();
        }

        public static void AddCashHistory(DateTime date, string memo, double netChange, string linkID, bool calculateBalance=true)
        {
            _CashHistories.Add(new CashHistory(date, memo, netChange, linkID));

            if (!calculateBalance) return;

            CalculateBalances();
        }

        public static void UpdateCashHistoryByID(string linkID, double netChange)
        {

            if (_CashHistories.All(x => x.GetID() != linkID)) return;

            _CashHistories.First(x => x.GetID() == linkID).SetNetAmount(netChange);

            CalculateBalances();
        }

        public static bool DeleteCashHistryByID(string linkID, bool calculateBalance = true)
        {
            if (_CashHistories.All(x => x.GetID() != linkID)) return false;

            _CashHistories.Remove(_CashHistories.First(x => x.GetID() == linkID));

            if (calculateBalance)
                CalculateBalances();

            return true;
        }

        public static double GetCurrentBalance(bool calculateBalance = true)
        {
            if (calculateBalance)
                CalculateBalances();

            return _CurrentBalance;
        }

        public static string GetCurrentBalanceStr(bool calculateBalance = true)
        {
            if (calculateBalance)
                CalculateBalances();

            return (GetCurrentBalance() < 0 ? "-" : "") + "$" + String.Format("{0:0.00}", Math.Abs(GetCurrentBalance()));
        }

        public static List<CashHistory> GetHistoriesBetweenDates(DateTime fromDate, DateTime toDate, bool newestFirst = false)
        {
            List<CashHistory> tempCHList = _CashHistories.Where(x => x.GetDate().Date <= toDate.Date && x.GetDate().Date >= fromDate).ToList();

            if (newestFirst) tempCHList.Reverse();

            return tempCHList;
        }

        public static List<CashHistory> GetHistories()
        {
            return _CashHistories.ToList();
        }

        public static void ClearCashHistory() 
        {
            _CashHistories = new List<CashHistory>();
        }

        public static void CalculateBalances()
        {
            _CurrentBalance = 0;
            _CashHistories = _CashHistories.OrderBy(x => x.GetDate()).ToList();

            foreach (CashHistory CH in GetHistories())
            {
                if (CH.GetID() == "SB") // SET BALANCE
                {
                    _CurrentBalance = CH.GetAmount();
                }
                else
                {
                    _CurrentBalance += CH.GetAmount();
                }

                CH.SetBalance(_CurrentBalance);
            }
        }
    }

    public class CashHistory
    {
        private DateTime _Date;
        private string _Memo;
        private double _NetChange;
        private string _LinkID;
        private double _CurrentBalance;

        public CashHistory(DateTime date, string memo, double netChange, string linkID)
        {
            _Date = date;
            _Memo = memo;
            _NetChange = netChange;
            _LinkID = linkID;
        }

        public void SetBalance(double amount)
        {
            _CurrentBalance = amount;
        }

        public void SetNetAmount(double netAmountChange)
        {
            _NetChange += netAmountChange;
        }

        public double GetBalance()
        {
            return _CurrentBalance;
        }

        public DateTime GetDate()
        {
            return _Date;
        }

        public string GetAmountStr()
        {
            return (GetAmount() < 0 ? "-" : "") + "$" + String.Format("{0:0.00}", Math.Abs(GetAmount()));
        }

        public string GetBalanceStr()
        {
            return (GetBalance() < 0 ? "-" : "") + "$" + String.Format("{0:0.00}", Math.Abs(GetBalance()));
        }

        public double GetAmount()
        {
            return _NetChange;
        }

        public string GetID()
        {
            return _LinkID;
        }

        public string GetMemo()
        {
            return _Memo;
        }

        public static double operator +(CashHistory a, CashHistory b)
        {
            return a.GetAmount() + b.GetAmount();
        }
    }
}
