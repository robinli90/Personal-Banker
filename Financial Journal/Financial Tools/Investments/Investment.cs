using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financial_Journal
{
    public class Investment
    {

        public string Name { get; set; }
        public bool Active { get; set; }
        public double Principal { get; set; }
        public double IRate { get; set; } // annual rate
        public string Frequency { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime End_Date { get; set; }
        public List<Investment_Transaction> Balance_Sequence { get; set; }

        // Values at the END of the period
        public List<Matrix_Entry> Matrix = new List<Matrix_Entry>();       // Internal use only
        public int Matrix_Extend_Years = 5;                                // Internal use only (extend the matrix depth by 5 years for foresight)
        public List<Investment_Transaction> Filtered_Sequence { get; set; } // Internal use only (used to filter out all but one transaction from period - keep newest, all others are non-essential

        // Base Memberwise Clone 
        public Investment Clone()
        {
            return System.MemberwiseClone.Copy(this);
        }

        // Filter most important transaction
        public void Get_Filtered_Sequence()
        {
            Filtered_Sequence = new List<Investment_Transaction>();

            Balance_Sequence.OrderByDescending(x => x.Entry_No).ToList().ForEach(x => Filtered_Sequence.Add(x));

            // Iterate through all periods
            foreach (Matrix_Entry ME in Matrix)
            {
                List<Investment_Transaction> IT = Get_Transactions_From_Period(ME.Entry_Date);

                // Remove subsequent sequences except first excerpt
                for (int i = 1; i < IT.Count; i++)
                {
                    Filtered_Sequence.Remove(IT[i]);
                }
            }

            Filtered_Sequence = Filtered_Sequence.OrderBy(x => x.Date).ToList();
        }

        /// <summary>
        /// Get from filtered list
        /// </summary>
        /// <param name="Start_Date"></param>
        /// <returns></returns>
        public List<Investment_Transaction> Get_Transactions_From_Period(DateTime Start_Date)
        {
            return Filtered_Sequence.Where(x => x.Date.Date >= Start_Date.Date && x.Date.Date < Get_Next_Period(Start_Date).Date).ToList();
        }

        /// <summary>
        /// Get from un-filtered list
        /// </summary>
        /// <param name="Start_Date"></param>
        /// <returns></returns>
        public List<Investment_Transaction> Get_Transactions_From_Period_Unfiltered(DateTime Start_Date)
        {
            return Balance_Sequence.Where(x => x.Date.Date >= Start_Date.Date && x.Date.Date < Get_Next_Period(Start_Date).Date).ToList();
        }

        /// <summary>
        /// Return the matrix entry closest to the Ref_Date (flooring Date)
        /// </summary>
        /// <param name="Ref_Date"></param>
        /// <returns></returns>
        public Matrix_Entry Get_Matrix_Entry(DateTime Ref_Date, bool Get_Next = false)
        {
            // Sort sequence in order, oldest to earliest
            Balance_Sequence = Balance_Sequence.OrderBy(x => x.Entry_No).ToList();

            if (Ref_Date.Date <= Start_Date.Date) return Matrix[0];

            for (int i = 1; i < Matrix.Count; i++)
            {
                if (Matrix[i].Entry_Date.Date > Ref_Date.Date) // Get the previous Matrix Entry before Ref_Date (we want ending balance of the previous period)
                {
                    return Matrix[i - 1 + (Get_Next ? 1 : 0)];
                }
            }

            return Matrix[Matrix.Count - 2];
        }

        /// <summary>
        /// Get balance of investment account; return principal amount if no matrix exists
        /// </summary>
        /// <param name="Ref_Date"></param>
        /// <returns></returns>
        public double Get_Balance_Since(DateTime Ref_Date)
        {
            if (Get_Matrix_Entry(DateTime.Now) == null)
                return Principal;

            return Get_Matrix_Entry(DateTime.Now).Total_Balance_Since;
        }

        public override string ToString()
        {
            string line = "";
            Matrix.ForEach(x => line += x.ToString() + Environment.NewLine);
            return line;
        }

        /// <summary>
        ///  Get amount since period start (mid period valuations)
        /// </summary>
        /// <param name="Ref_Date"></param>
        /// <returns></returns>
        public double Get_Amt_Since_Period_Start(DateTime Ref_Date)
        {
            // No backtracking
            if (Get_Next_Period(Ref_Date) < Start_Date) return 0;

            Matrix_Entry Start_Period = Get_Matrix_Entry(Ref_Date);
            Matrix_Entry Next_Period = Get_Matrix_Entry(Get_Next_Period(Start_Period.Entry_Date));

            double Days_Per_Period = (Next_Period.Entry_Date.Date - Start_Period.Entry_Date.Date).TotalDays;
            double Days_Since_Period = (Ref_Date.Date - Start_Period.Entry_Date.Date).TotalDays;

            // No negative days
            if (Days_Since_Period < 0) return Start_Period.Total_Principal_Since;

            double Period_Amount = Next_Period.Total_Balance_Since - Start_Period.Total_Balance_Since;

            double Current_Period_Interest = Period_Amount * (Days_Since_Period / Days_Per_Period);

            return Start_Period.Total_Balance_Since + Current_Period_Interest;
        }

        /// <summary>
        /// Carry over any increase/decrease to subsequent transactions NOT in current period (we neglect the same period valuations because they will be filtered later anyway and it skews with the history values)
        /// </summary>
        /// <param name="Start_Point"></param>
        /// <param name="Change_In_Value"></param>
        public void Adjust_Sequence(Investment_Transaction Start_Point, double Change_In_Value)
        {
            Balance_Sequence = Balance_Sequence.OrderBy(x => x.Date).ToList();

            for (int i = Balance_Sequence.IndexOf(Start_Point) + 1; i < Balance_Sequence.Count; i++)
            {
                if (Balance_Sequence[i].Date >= Get_Next_Period(Start_Point.Date))
                    Balance_Sequence[i].Principal_Carry_Over += Change_In_Value;
            }
        }

        public bool Withdraw(double Amt, DateTime Transaction_Date)
        {
            double Amount_Before = Get_Matrix_Entry(Transaction_Date, false).Total_Balance_Since;
            double Principal_Before = Get_Matrix_Entry(Transaction_Date, false).Total_Principal_Since;
            if (Amt <= Amount_Before)
            {
                Investment_Transaction IT = new Investment_Transaction() { Entry_No = Balance_Sequence.Count, Date = Transaction_Date, Principal_Carry_Over = Principal_Before - Amt };
                Balance_Sequence.Add(IT);
                Adjust_Sequence(IT, -Amt);
                Populate_Matrix();
                return true;
            }
            return false;
        }

        public bool Deposit(double Amt, DateTime Transaction_Date)
        {
            double Amount_Before = Get_Matrix_Entry(Transaction_Date, false).Total_Balance_Since;
            double Principal_Before = Get_Matrix_Entry(Transaction_Date, false).Total_Principal_Since;
            Investment_Transaction IT = new Investment_Transaction() { Entry_No = Balance_Sequence.Count, Date = Transaction_Date, Principal_Carry_Over = Principal_Before + Amt };
            Balance_Sequence.Add(IT);
            Adjust_Sequence(IT, Amt);
            Populate_Matrix();
            return true;
        }

        /// <summary>
        /// Check if okay to perform action on date matrix. If date doesnt exist, return false
        /// </summary>
        /// <param name="Ref_Date"></param>
        /// <returns></returns>
        public bool Check_Date_Validity(DateTime Ref_Date)
        {
            if (Ref_Date < Get_Next_Period(Matrix[0].Entry_Date)) return false;
            return (Get_Matrix_Entry(Ref_Date) != null);
        }

        /// <summary>
        /// Generate matrix (most important function in this class)
        /// </summary>
        public void Populate_Matrix()
        {
            Matrix = new List<Matrix_Entry>();

            // Get definition dates
            DateTime Ref_End_Date = End_Date.Year > 1800 ? End_Date : (DateTime.Now.AddYears(Matrix_Extend_Years));
            DateTime Curr_Period = Get_Next_Period(Start_Date);
            Ref_End_Date = Get_Next_Period(Ref_End_Date);

            // Populate null matrix
            while (Curr_Period < Ref_End_Date)
            {
                Matrix.Add(new Matrix_Entry() { Entry_Date = Curr_Period });
                Curr_Period = Get_Next_Period(Curr_Period);
            }

            double Current_Principal = Principal;
            int Current_Sequence_Index = 0;

            // If matrix actually has items
            if (Matrix.Count > 0)
            {
                Matrix_Entry Ref_ME = Matrix[0];

                Get_Filtered_Sequence();

                // Calculate first period
                Ref_ME.Period = 0;
                Ref_ME.Entry_Date = Matrix[0].Entry_Date;
                Ref_ME.Total_Principal_Since = Principal;
                Ref_ME.Total_Balance_Since = Calculate_Balance(Principal);
                Ref_ME.Total_Interest_Since = Calculate_Balance(Principal) - Principal;
                Ref_ME.Interest_Amount = Principal * Get_IRate();

                // Check for any changes within start date period
                if (Filtered_Sequence.Count > 0 && Filtered_Sequence.Count > Current_Sequence_Index && Filtered_Sequence[Current_Sequence_Index].Date < Get_Next_Period(Matrix[0].Entry_Date.Date))
                {
                    Ref_ME.Total_Principal_Since = Filtered_Sequence[Current_Sequence_Index].Principal_Carry_Over;
                    //Ref_ME.Total_Principal_Since = Matrix[i - 1].Total_Principal_Since + ((Filtered_Sequence[Current_Sequence_Index].Balance - Matrix[i - 1].Total_Principal_Since) * (1 - Get_IRate()) - Matrix[i - 1].Total_Interest_Since);
                    //Ref_ME.Total_Balance_Since = Calculate_Balance(Ref_ME.Total_Principal_Since);
                    Ref_ME.Total_Balance_Since = Calculate_Balance(Ref_ME.Total_Principal_Since + 0);
                    //Ref_ME.Total_Interest_Since = Matrix[i - 1].Total_Interest_Since + Ref_ME.Total_Balance_Since - Ref_ME.Total_Principal_Since;
                    Ref_ME.Total_Interest_Since = Ref_ME.Total_Balance_Since - Ref_ME.Total_Principal_Since;
                    //Ref_ME.Interest_Amount = Ref_ME.Total_Principal_Since * Get_IRate();
                    Ref_ME.Interest_Amount = Ref_ME.Total_Interest_Since;
                    Current_Sequence_Index++;
                }
                
                // Calculate subsequent Matrices
                for (int i = 1; i < Matrix.Count - 1; i++)
                {
                    Ref_ME = Matrix[i];
                    Ref_ME.Period = i;

                    // Skip inter_period changes

                    // Adjust balance if transaction happened before compound date
                    if (Filtered_Sequence.Count > 0 && Filtered_Sequence.Count > Current_Sequence_Index && Filtered_Sequence[Current_Sequence_Index].Date < Get_Next_Period(Matrix[i].Entry_Date.Date))
                    {
                        Ref_ME.Total_Principal_Since = Filtered_Sequence[Current_Sequence_Index].Principal_Carry_Over;
                        //Ref_ME.Total_Principal_Since = Matrix[i - 1].Total_Principal_Since + ((Filtered_Sequence[Current_Sequence_Index].Balance - Matrix[i - 1].Total_Principal_Since) * (1 - Get_IRate()) - Matrix[i - 1].Total_Interest_Since);
                        //Ref_ME.Total_Balance_Since = Calculate_Balance(Ref_ME.Total_Principal_Since);
                        Ref_ME.Total_Balance_Since = Calculate_Balance(Ref_ME.Total_Principal_Since + Matrix[i - 1].Total_Interest_Since);
                        //Ref_ME.Total_Interest_Since = Matrix[i - 1].Total_Interest_Since + Ref_ME.Total_Balance_Since - Ref_ME.Total_Principal_Since;
                        Ref_ME.Total_Interest_Since = Ref_ME.Total_Balance_Since - Ref_ME.Total_Principal_Since;
                        //Ref_ME.Interest_Amount = Ref_ME.Total_Principal_Since * Get_IRate();
                        Ref_ME.Interest_Amount = Ref_ME.Total_Interest_Since - Matrix[i - 1].Total_Interest_Since;
                        Current_Sequence_Index++;
                    }
                    else // Continue Principal from previous
                    {
                        Ref_ME.Total_Principal_Since = Matrix[i - 1].Total_Principal_Since;
                        Ref_ME.Total_Balance_Since = Calculate_Balance(Matrix[i - 1].Total_Balance_Since);
                        Ref_ME.Total_Interest_Since = Ref_ME.Total_Balance_Since - Ref_ME.Total_Principal_Since;
                        Ref_ME.Interest_Amount = Matrix[i - 1].Total_Balance_Since * Get_IRate();
                    }
                }

            }
            
        }

        /// <summary>
        /// Recursively grab the current balance
        /// </summary>
        /// <param name="Principal"></param>
        /// <param name="Period_Rate"></param>
        /// <param name="Period"></param>
        /// <returns></returns>
        public double Calculate_Balance(double Principal)
        {
            return Principal * (1 + Get_IRate());
        }

        /// <summary>
        /// Return the next appropriate period
        /// </summary>
        /// <param name="Curr_Period"></param>
        /// <returns></returns>
        public DateTime Get_Next_Period(DateTime Curr_Period)
        {
            

            switch (this.Frequency)
            {
                case "Monthly":
                    return AddMonths(Curr_Period, 1);
                case "Bi-monthly":
                    return AddMonths(Curr_Period, 2);
                case "Semi-annually":
                    return AddMonths(Curr_Period, 6);
                case "Annually":
                    return Curr_Period.AddYears(1);
                default:
                    return Curr_Period;
            }
        }

        public DateTime AddMonths(DateTime Ref_Date, int Months)
        {
            for (int i = 0; i < Months; i++)
            {
                Ref_Date = NextMonth(Ref_Date);
            }

            return Ref_Date;
        }

        public DateTime NextMonth(DateTime date)
        {
            if (date.Day != DateTime.DaysInMonth(date.Year, date.Month))
                return date.AddMonths(1);
            else
                return date.AddDays(1).AddMonths(1).AddDays(-1);
        }

        /// <summary>
        /// Return frequency interest rate
        /// </summary>
        /// <returns></returns>
        public double Get_IRate()
        {
            switch (this.Frequency)
            {
                case "Monthly":
                    return IRate / 12;
                case "Bi-monthly":
                    return IRate / 6;
                case "Semi-annually":
                    return IRate / 2;
                case "Annually":
                    return IRate;
                default:
                    return IRate;
            }
        }
    }

    public class Investment_Transaction
    {
        public int Entry_No { get; set; }
        public double Principal_Carry_Over { get; set; }
        public DateTime Date { get; set; }

        public Investment_Transaction Clone()
        {
            return System.MemberwiseClone.Copy(this);
        }
    }

    public class Matrix_Entry
    {
        public int Period { get; set; }
        public DateTime Entry_Date { get; set; }
        public double Interest_Amount { get; set; }
        public double Total_Interest_Since { get; set; }
        public double Total_Principal_Since { get; set; }
        public double Total_Balance_Since { get; set; }

        public override string ToString()
        {
            return Period + " | " +
                "$" + String.Format("{0:0.00}", Interest_Amount) + " | " +
                "$" + String.Format("{0:0.00}", Total_Interest_Since) + " | " +
                "$" + String.Format("{0:0.00}", Total_Principal_Since) + " | " +
                "$" + String.Format("{0:0.00}", Total_Balance_Since) + " | " +
                Entry_Date.ToShortDateString();
        }
    }
}
