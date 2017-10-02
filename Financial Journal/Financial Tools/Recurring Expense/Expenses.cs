using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Expenses
    {
        public string Expense_Type { get; set; }
        public string Expense_Name { get; set; }
        public string Expense_Payee { get; set; }
        public string Expense_Frequency { get; set; }
        public string Expense_Status { get; set; }
        public double Expense_Amount { get; set; }
        public List<DateTime> Date_Sequence { get; set; } // For sequencing hold and release dates
        public DateTime Expense_Start_Date { get; set; }
        public double Temp_Exp { get; set; }
        public DateTime Alert_Off_Date { get; set; }
        public DateTime Next_Pay_Date_IUO { get; set; } // next pay date internal use only

        // Static weeks per x
        public static double Weeks_In_BiWeekly = 2.1726190476190492262;
        public static double Weeks_In_Monthly = 4.3452380952380940116;
        public static double Weeks_In_BiMonthly = 8.6904761904761933522;
        public static double Weeks_In_Quarterly = 13.03571;
        public static double Weeks_In_SemiAnually = 26.0714;
        public static double Weeks_In_Anually = 52.1429;

        // Autodebit information
        public string AutoDebit { get; set; }
        public string Payment_Company { get; set; }
        public string Payment_Last_Four { get; set; }
        public DateTime Last_Pay_Date { get; set; }

        public Expenses()
        {
            
        }

        public void Process_Payments(Receipt parent, ref Payment payment)
        {
            // If Autodebit is enabled and expense is active
            if (AutoDebit == "1" && Expense_Status == "1")
            {
                // While we haven't paid, charge to payment and reduce balance
                while (Last_Pay_Date <= DateTime.Now)
                {
                    // Reduce balance of payment
                    payment.Balance -= Expense_Amount;

                    // Add history in payment_options
                    parent.Payment_Options_List.Add(new Payment_Options()
                    {
                        Type = "Payment",
                        Amount = Expense_Amount,
                        Date = Last_Pay_Date,
                        Note = ("Recurring Exp.: " + Expense_Name + " to " + Expense_Payee),
                        Ending_Balance = payment.Balance,// Adjust balance
                        Payment_Bank = payment.Bank,
                        Hidden_Note = Expense_Name, // hidden so program can identify this is recurring expense
                        Payment_Company = payment.Company,
                        Payment_Last_Four = payment.Last_Four
                    });

                    // add converted dates to last pay date and change last pay date
                    if (this.Expense_Frequency == "Weekly") { Last_Pay_Date = Last_Pay_Date.AddDays(7); }
                    else if (this.Expense_Frequency == "Bi-Weekly") { Last_Pay_Date = Last_Pay_Date.AddDays(14); }
                    else if (this.Expense_Frequency == "Monthly") { Last_Pay_Date = Last_Pay_Date.AddMonths(1); }
                    else if (this.Expense_Frequency == "Bi-Monthly") { Last_Pay_Date = Last_Pay_Date.AddMonths(2); }
                    else if (this.Expense_Frequency == "Quarterly") { Last_Pay_Date = Last_Pay_Date.AddMonths(3); }
                    else if (this.Expense_Frequency == "Semi-Annually") { Last_Pay_Date = Last_Pay_Date.AddMonths(6); }
                    else if (this.Expense_Frequency == "Annually") { Last_Pay_Date = Last_Pay_Date.AddYears(1); }
                }
            }
        }

        /// <summary>
        /// Check if expense is due (only for non-auto-debit expenses)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="Check_Days_Before"></param>
        /// <param name="Ref_Date_"></param>
        /// <returns></returns>
        public bool Check_Expenses(Receipt parent, int Check_Days_Before = 4, DateTime Ref_Date_ = new DateTime())
        {
            if (Expense_Name.Contains("3GB")) Console.Write("");

            DateTime Ref_Date = DateTime.Now;

            if (Ref_Date_ != DateTime.MinValue)
            {
                Ref_Date = Ref_Date_;
            }

            // If Autodebit is NOT enabled and expense is active (we need to alert only if its not auto-debit
            if (AutoDebit != "1" && Expense_Status == "1")
            {

                DateTime StartDate = Expense_Start_Date;
                // Adjust start date to next payment time
                while (StartDate.Date < Ref_Date.Date)
                {
                    if (this.Expense_Frequency == "Weekly") { StartDate = StartDate.AddDays(7); }
                    else if (this.Expense_Frequency == "Bi-Weekly") { StartDate = StartDate.AddDays(14); }
                    else if (this.Expense_Frequency == "Monthly") { StartDate = StartDate.AddMonths(1); }
                    else if (this.Expense_Frequency == "Bi-Monthly") { StartDate = StartDate.AddMonths(2); }
                    else if (this.Expense_Frequency == "Quarterly") { StartDate = StartDate.AddMonths(3); }
                    else if (this.Expense_Frequency == "Semi-Annually") { StartDate = StartDate.AddMonths(6); }
                    else if (this.Expense_Frequency == "Annually") { StartDate = StartDate.AddYears(1); }
                }

                if (Ref_Date.Date >= StartDate.AddDays(-Check_Days_Before).Date && Ref_Date.Date <= StartDate.Date && (Ref_Date_ != DateTime.MinValue || Alert_Off_Date.Date != Ref_Date.Date))
                {
                    // Set to current date as to not repeat (only actual alert check, not for external boolean checks)
                    if (Ref_Date_ == DateTime.MinValue)
                    {
                        Alert_Off_Date = DateTime.Now;
                    }

                    Next_Pay_Date_IUO = StartDate;

                    return true;
                    // Alert
                }
            }
            return false;
        }
        
        // Return the amount based on the requested search period
        //
        // User searches for monthly recurring amount, get frequency based on num of weeks
        //
        public double Get_Amount_From_Weeks(double Num_Of_Weeks)
        {
            double Weekly_Amount = 0;

            // Get base amount per week based on current expense frequency and amount (exact amounts)
            if (this.Expense_Frequency == "Weekly") { Weekly_Amount = this.Expense_Amount; }
            else if (this.Expense_Frequency == "Bi-Weekly") { Weekly_Amount = this.Expense_Amount / Weeks_In_BiWeekly; }
            else if (this.Expense_Frequency == "Monthly") { Weekly_Amount = this.Expense_Amount / Weeks_In_Monthly; }
            else if (this.Expense_Frequency == "Bi-Monthly") { Weekly_Amount = this.Expense_Amount / Weeks_In_BiMonthly; }
            else if (this.Expense_Frequency == "Quarterly") { Weekly_Amount = this.Expense_Amount / Weeks_In_Quarterly; }
            else if (this.Expense_Frequency == "Semi-Annually") { Weekly_Amount = this.Expense_Amount / Weeks_In_SemiAnually; }
            else if (this.Expense_Frequency == "Annually") { Weekly_Amount = this.Expense_Amount / Weeks_In_Anually; }

            // Return the search amount frequency
            return Weekly_Amount * Num_Of_Weeks;
        }

        /// <summary>
        /// Return the total date from x date. If null, return since start date!
        ///
        /// If To_date is provided, then get the total paid between the two dates
        /// </summary>
        /// <param name="From_Date"></param>
        /// <param name="To_Date"></param>
        /// <returns>Amount</returns>
        public double Get_Total_Paid(object From_Date = null, object To_Date = null)
        {
            double Weekly_Amount = 0;

            if (Expense_Name == "Mortgage") Diagnostics.WriteLine("");

            // Get base amount per week based on current expense frequency and amount (exact amounts)
            if (this.Expense_Frequency == "Weekly") { Weekly_Amount = this.Expense_Amount; }
            else if (this.Expense_Frequency == "Bi-Weekly") { Weekly_Amount = this.Expense_Amount / Weeks_In_BiWeekly; }
            else if (this.Expense_Frequency == "Monthly") { Weekly_Amount = this.Expense_Amount / Weeks_In_Monthly; }
            else if (this.Expense_Frequency == "Bi-Monthly") { Weekly_Amount = this.Expense_Amount / Weeks_In_BiMonthly; }
            else if (this.Expense_Frequency == "Quarterly") { Weekly_Amount = this.Expense_Amount / Weeks_In_Quarterly; }
            else if (this.Expense_Frequency == "Semi-Annually") { Weekly_Amount = this.Expense_Amount / Weeks_In_SemiAnually; }
            else if (this.Expense_Frequency == "Annually") { Weekly_Amount = this.Expense_Amount / Weeks_In_Anually; }

            // Divide by 7 days a week;
            double Daily_Amount = Weekly_Amount / 7;

            // Set start date
            DateTime Start_Date = this.Expense_Start_Date;

            if (From_Date != null && (From_Date is DateTime))
            {
                Start_Date = (DateTime)From_Date;
            }

            // If start date & end date is before the expense start date return 0
            if (Start_Date < Expense_Start_Date && To_Date != null && (DateTime)To_Date <= Expense_Start_Date)
            {
                return 0;
            }

            if (Start_Date < Expense_Start_Date && To_Date != null && Date_Sequence.Count == 0)
            {
                return ((To_Date != null ? (DateTime)To_Date : DateTime.Now) - Expense_Start_Date).TotalDays * Daily_Amount;
            }

            if (Start_Date < (To_Date != null ? (DateTime)To_Date : DateTime.Now))
            {
                if (Date_Sequence.Count == 0)
                {
                    return ((To_Date != null ? (DateTime)To_Date : DateTime.Now) - Start_Date).TotalDays * Daily_Amount;
                }
                else
                {
                    double Total_Paid_Amount = 0;

                    List<DateTimeRange> Sequence_Range_List = new List<DateTimeRange>();
                    for (int i = 0; i < Date_Sequence.Count; i++)
                    {
                        if (i == 0)
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Expense_Start_Date, End_Date = Date_Sequence[0] };
                            Sequence_Range_List.Add(temp);
                        }
                        else if (i + 2 <= Date_Sequence.Count) // if not end 
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Date_Sequence[i], End_Date = Date_Sequence[i + 1] };
                            Sequence_Range_List.Add(temp);
                            i++;
                        }
                        else if (i + 1 == Date_Sequence.Count)
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Date_Sequence[i], End_Date = To_Date != null ? (DateTime)To_Date : DateTime.Now };
                            Sequence_Range_List.Add(temp);
                        }
                    }
                    
                    DateTimeRange ref_Date = new DateTimeRange() { Start_Date = Start_Date, End_Date = To_Date != null ? (DateTime)To_Date : DateTime.Now };

                    Total_Paid_Amount += Sequence_Range_List.Sum(x => x.DateTime_Overlap(ref_Date) * Daily_Amount);

                    return Total_Paid_Amount;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Return if the expense was active ever within this Ref_Date month
        /// </summary>
        /// <param name="Ref_Date_Month"></param>
        /// <returns></returns>
        public bool Check_Expense_Active_Date(DateTime Ref_Date)
        {
            if (Date_Sequence.Count > 1 && Ref_Date.Month == 8)
            {
                Diagnostics.WriteLine("");
            }
            if (Expense_Start_Date < Ref_Date) 
            {
                // Active and unchanged since start date
                if (Expense_Status == "1" && Date_Sequence.Count == 0)
                {
                    return true;
                }
                else
                {

                    List<DateTimeRange> Sequence_Range_List = new List<DateTimeRange>();
                    for (int i = 0; i < Date_Sequence.Count; i++)
                    {
                        if (i == 0)
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Expense_Start_Date, End_Date = Date_Sequence[0] };
                            Sequence_Range_List.Add(temp);
                        }
                        else if (i + 2 <= Date_Sequence.Count) // if not end 
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Date_Sequence[i], End_Date = Date_Sequence[i + 1] };
                            Sequence_Range_List.Add(temp);
                            i++;
                        }
                        else if (i + 1 == Date_Sequence.Count)
                        {
                            DateTimeRange temp = new DateTimeRange() { Start_Date = Date_Sequence[i], End_Date = DateTime.Now };
                            Sequence_Range_List.Add(temp);
                        }
                    }

                    foreach (DateTimeRange DTR in Sequence_Range_List)
                    {
                        foreach (DateTime Month_Dates in GetDates(Ref_Date.Year, Ref_Date.Month))
                        {
                            if (DTR.InBetween_Dates(Month_Dates)) return true;
                        }
                    }
                }
            }
            return false;
        }

        public static List<DateTime> GetDates(int year, int month)
        {
           return Enumerable.Range(1, DateTime.DaysInMonth(year, month))  // Days: 1, 2 ... 31 etc.
                            .Select(day => new DateTime(year, month, day)) // Map each day to a date
                            .ToList(); // Load dates into a list
        }
    }

    public class DateTimeRange
    {
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }

        // Return the total number of days (double) overlapped between two DTR
        public double DateTime_Overlap(DateTimeRange dtr)
        {
            if (dtr.Start_Date < End_Date && dtr.End_Date > Start_Date)
            {
                if (dtr.Start_Date <= Start_Date && dtr.End_Date >= End_Date) // If this range lies within entire range of dtr
                {
                    return (End_Date - Start_Date).TotalDays;
                }
                else if (dtr.Start_Date <= Start_Date && dtr.End_Date <= End_Date) // only dtr.end_date lies within range
                {
                    return (dtr.End_Date - Start_Date).TotalDays;
                }
                else if (dtr.Start_Date >= Start_Date && dtr.End_Date >= End_Date) // only dtr.start_date lies within range
                {
                    return (End_Date - dtr.Start_Date).TotalDays;
                }
                else if (dtr.Start_Date >= Start_Date && dtr.End_Date <= End_Date) // Entire dtr date lies within range
                {
                    return (dtr.End_Date - dtr.Start_Date).TotalDays;
                }
            }
            return 0;
        }

        public bool InBetween_Dates(DateTime dt)
        {
            return dt > Start_Date && dt < End_Date;
        }
    }
}
