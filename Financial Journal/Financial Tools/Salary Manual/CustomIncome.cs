using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class CustomIncome
    {
        /*
        public string Save_Manual_Income()
        {
            string line = "";
            foreach (CustomIncome CI in Income_Company_List)
            {
                line += "[MANUAL_INCOME_COMPANY]=" + CI.Company +
                        "||[MANUAL_FIRST_DATE]=" + CI.First_Period_Date.ToShortDateString() +
                        "||[MANUAL_STOP_DATE]=" + CI.Stop_Date.ToShortDateString() +
                        "||[MANUAL_FREQUENCY]=" + CI.Frequency +
                        "||[MANUAL_DEFAULT]=" + (CI.Default ? "1" : "0") +
                        "||[MANUAL_INTERVALS]=" + CI.Export_Intervals() + Environment.NewLine;
            }
            return line + Environment.NewLine;
         }
         */

        public string Deposit_Account { get; set; }
        public bool Default { get; set; }
        public DateTime First_Period_Date { get; set; }
        public DateTime Stop_Date { get; set; }
        public string Frequency { get; set; }   // Monthly
                                                // Bi-weekly
                                                // Weekly
        public string Company { get; set; }
        public List<PayPeriod> Intervals { get; set; }

        public CustomIncome() { Deposit_Account = ""; }

        /// <summary>
        /// Populate the intervals using the info parsed from "SALARY_INTERVALS=" value
        /// 
        /// i.e.: period1,DateTime1.toShortDateString(),amount1,period2,DateTime2.toShortDateString(),amount2,...
        /// 
        /// </summary>
        /// <param name="Interval_String"></param>

        public string Export_Intervals()
        {
            string return_str = "";
            foreach (PayPeriod PP in Intervals)
            {
                return_str += PP.Pay_Period + "," + PP.Pay_Date.ToShortDateString() + "," + PP.Amount + ",";
            }
            return return_str.Trim(',');
        }

        public List<PayPeriod> GetIntervalsPlusNext(int repeatCount = 1)
        {
            List<PayPeriod> tempRefList = new List<PayPeriod>();

            Intervals.ForEach(x => tempRefList.Add(x.Copy_Item()));

            for (int i = 0; i < repeatCount; i++)
            {
                if (tempRefList[tempRefList.Count - 1].Pay_Date >= Stop_Date && Stop_Date.Year > 1900) break;

                switch (Frequency)
                {
                    case "Weekly":
                        tempRefList.Add(new PayPeriod()
                        {
                            Amount = 0,
                            Pay_Period = tempRefList[tempRefList.Count - 1].Pay_Period + 1,
                            Pay_Date = tempRefList[tempRefList.Count - 1].Pay_Date.AddDays(7)
                        });
                        break;
                    case "Bi-weekly":
                        tempRefList.Add(new PayPeriod()
                        {
                            Amount = 0,
                            Pay_Period = tempRefList[tempRefList.Count - 1].Pay_Period + 1,
                            Pay_Date = tempRefList[tempRefList.Count - 1].Pay_Date.AddDays(14)
                        });
                        break;
                    case "Monthly":
                        tempRefList.Add(new PayPeriod()
                        {
                            Amount = 0,
                            Pay_Period = tempRefList[tempRefList.Count - 1].Pay_Period + 1,
                            Pay_Date = tempRefList[tempRefList.Count - 1].Pay_Date.AddMonths(1)
                        });
                        break;
                    default:
                        break;
                }
            }
            return tempRefList;
        }

        public void Populate_Intervals(string Interval_String = "")
        {
            Intervals = new List<PayPeriod>();

            DateTime Ref_Date = First_Period_Date;
            int Period = 1;

            DateTime Create_Till_Date = DateTime.Now;

            // If stopped, we don't create past that date
            if (Stop_Date.Year > 1801) Create_Till_Date = Stop_Date;

            // Create blank master
            while (Ref_Date <= Create_Till_Date)
            {
                Intervals.Add(new PayPeriod()
                {
                    Pay_Period = Period,
                    Pay_Date = Convert.ToDateTime(Ref_Date),
                    Amount = 0
                });

                // Add appropriate days
                switch (Frequency)
                {
                    case "Weekly":
                        Ref_Date = Ref_Date.AddDays(7);
                        break;
                    case "Bi-weekly":
                        Ref_Date = Ref_Date.AddDays(14);
                        break;
                    case "Monthly":
                        Ref_Date = Ref_Date.AddMonths(1);
                        break;
                    default:
                        break;
                }
                Period ++;
            }

            string[] Temp = Interval_String.Split(new string[] { "," }, StringSplitOptions.None);

            // Only if pre-existing values have been set
            if (Temp.Count() > 1)
            {
                // Add pre-existing pay periods that have been set
                for (int i = 0; i < Temp.Count(); i += 3)
                {
                    try
                    {
                        Intervals[Convert.ToInt32(Temp[i]) - 1].Amount = Convert.ToDouble(Temp[i + 2]);
                    }
                    catch
                    {
                        // Invalid string parse length of modulus 3 or Period does not exist (latter should never happen)
                    }
                }
            }                  
        }

        /// <summary>
        /// Return previous pay period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public PayPeriod Get_Prev_Period(int Period)
        {
            return Period <= 1 ? Intervals[0] : Intervals[Period - 2];
        }

        /// <summary>
        /// Return next pay period
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public PayPeriod Get_Next_Period(int Period)
        {
            return Intervals[Period];
        }

        public double Get_Monthly_Amount(int month, int year)
        {

            List<PayPeriod> Current_Month_Period_List = Intervals.Where(x => x.Pay_Date.Month == month && x.Pay_Date.Year == year).ToList();

            DateTime Ref_Date = new DateTime(year, month, 1);

            // Find a relevent pay period to the month provided
            while (Current_Month_Period_List.Count == 0)
            {
                if (Ref_Date.Year < 1900 || (Stop_Date.Year > 1801 && Ref_Date > Stop_Date))
                {
                    // Error cannot find date
                    return 0;
                }

                Current_Month_Period_List = Intervals.Where(x => x.Pay_Date.Month == Ref_Date.Month && x.Pay_Date.Year == Ref_Date.Year && !(Stop_Date.Year > 1801 && Ref_Date > Stop_Date)).ToList();
                Ref_Date = Ref_Date.AddMonths(-1);

            }

            // if current month > any existing month, get latest pay date and assume that paydate
            if (Current_Month_Period_List.Count(x => x.Pay_Date.Month == month && x.Pay_Date.Year == year) == 0)
            {
                // get the latest paydate and use this amount and stretch across provided month
                PayPeriod refPP = Intervals[Intervals.Count - 1];

                double ppAmount = refPP.Amount;

                // Convert pay to pay-per-day
                switch (Frequency)
                {
                    case "Weekly":
                        ppAmount = ppAmount / 7;
                        break;
                    case "Bi-weekly":
                        ppAmount = ppAmount / 14;
                        break;
                    case "Monthly":
                        ppAmount = ppAmount / DateTime.DaysInMonth(year, month);
                        break;
                    default:
                        break;
                }

                return ppAmount * DateTime.DaysInMonth(year, month);
            }
            else // else there is a pay month relevant
            {
                // Sort days by period value
                Current_Month_Period_List = Current_Month_Period_List.OrderBy(x => x.Pay_Period).ToList();

                DateTime Start_Of_Month = new DateTime(year, month, 1);
                DateTime End_Of_Month = Start_Of_Month.AddMonths(1).AddDays(-1);

                if (Stop_Date.Month == month && Stop_Date.Year == year)
                {
                    End_Of_Month = Stop_Date;
                }

                // Get the days from inner month payperiod to start/end
                double Days_From_Start = (Current_Month_Period_List[0].Pay_Date.Date - Start_Of_Month.Date).TotalDays;
                double Days_From_End = (End_Of_Month.Date - Current_Month_Period_List[Current_Month_Period_List.Count - 1].Pay_Date.Date).TotalDays;

                // Salary for above days
                double Amount_Et_Start = 0;
                double Amount_Et_End = 0;

                #region Start_Pay_Period
                int Current_Period = Current_Month_Period_List[0].Pay_Period;

                // Find next available pay period
                while (Amount_Et_Start == 0 && Current_Period > 0)
                {
                    Amount_Et_Start = Get_Prev_Period(Current_Period).Amount;
                    if (Amount_Et_Start == 0) Current_Period--;
                }
                #endregion

                #region End_Pay_Period
                Current_Period = Current_Month_Period_List[Current_Month_Period_List.Count - 1].Pay_Period;

                Amount_Et_End = Current_Month_Period_List[Current_Month_Period_List.Count - 1].Amount;
                while (Amount_Et_End == 0 && Current_Period > 0)
                {
                    Amount_Et_End = Get_Prev_Period(Current_Period).Amount;
                    if (Amount_Et_End == 0) Current_Period--;
                }
                #endregion

                // Convert pay to pay-per-day
                switch (Frequency)
                {
                    case "Weekly":
                        Amount_Et_Start = Amount_Et_Start / 7;
                        Amount_Et_End = Amount_Et_End / 7;
                        break;
                    case "Bi-weekly":
                        Amount_Et_Start = Amount_Et_Start / 14;
                        Amount_Et_End = Amount_Et_End / 14;
                        break;
                    case "Monthly":
                        Amount_Et_Start = Amount_Et_Start / DateTime.DaysInMonth(year, month);
                        Amount_Et_End = Amount_Et_End / DateTime.DaysInMonth(year, month);
                        break;
                    default:
                        break;
                }

                // Multiply by missing days
                Amount_Et_Start = Amount_Et_Start >= 0 ? Amount_Et_Start * Days_From_Start : 0;
                Amount_Et_End = Amount_Et_End >= 0 ? Amount_Et_End * Days_From_End : 0;

                double Inner_Amount = 0;

                // Calculate inner days 
                // Minimum 2 periods within this list. We ignore the last one because we used that to calculate the end and hence - 2
                for (int i = 0; i <= Current_Month_Period_List.Count - 2; i++)
                {
                    Inner_Amount += Current_Month_Period_List[i].Amount;
                }


                return Amount_Et_Start + Inner_Amount + Amount_Et_End;
            }
        }
    }

    public class PayPeriod
    {
        public DateTime Pay_Date { get; set; }
        public int Pay_Period { get; set; }
        public double Amount { get; set; }
        public string Name_IUO { get; set; } // Name internal use only

        public PayPeriod Copy_Item()
        {
            return System.MemberwiseClone.Copy(this);
        }
    }
}
