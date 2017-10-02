using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Financial_Journal
{
    class vCardParser : IEnumerable<vCard>
    {
        public List<vCard> vCardList = new List<vCard>();

        public vCard this[int index]
        {
            get { return vCardList[index]; }
            set { vCardList.Insert(index, value); }
        }

        public IEnumerator<vCard> GetEnumerator()
        {
            return vCardList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void PopulateCardsFromVCF(string vcfFilePath)
        {
            var text = File.ReadAllText(vcfFilePath).Trim();
            if (text.Length > 0)
            {
                List<string> temp = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).ToList();

                List<List<string>> Post_Parsed_Data = new List<List<string>>();

                int Begin_Index = 0;
                
                // First tier parse
                for (int i = 0; i < temp.Count; i++)
                {
                    if (temp[i].Trim().Contains("BEGIN:VCARD"))
                    {
                        Begin_Index = i;
                    }
                    if (temp[i].Trim().Contains("END:VCARD"))
                    {
                        Post_Parsed_Data.Add(temp.GetRange(Begin_Index + 1, i - 1 - Begin_Index));
                    }
                }

                foreach (List<string> g in Post_Parsed_Data)
                {
                    vCard tempVCard = new vCard();
                    foreach (string x in g)
                    {
                        if (x.StartsWith("VERSION"))
                        {
                            tempVCard.Version = Parse_Line_Information(x, "VERSION");
                        }
                        if (x.StartsWith("N:"))
                        {
                            string gg =  Parse_Line_Information(x, "N");
                            tempVCard.FirstName = gg.Split(new string[] { ";" }, StringSplitOptions.None)[1];
                            tempVCard.LastName = gg.Split(new string[] { ";" }, StringSplitOptions.None)[0];
                        }
                        if (x.StartsWith("FN:"))
                        {
                            tempVCard.FullName = Parse_Line_Information(x, "FN");
                        }
                        if (x.StartsWith("TEL;"))
                        {
                            tempVCard.PhoneNumbers.Add(x.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                        }
                        if (x.StartsWith("EMAIL;"))
                        {
                            tempVCard.Emails.Add(x.Split(new string[] { ":" }, StringSplitOptions.None)[1]);
                        }
                        if (x.StartsWith("PRODID"))
                        {
                            tempVCard.ProductID = Parse_Line_Information(x, "PRODID");
                        }
                    }
                    this.vCardList.Add(tempVCard);
                }
            }

            // Remove null values
            this.vCardList = this.vCardList.Where(x => (x.FirstName != "" && x.LastName != "") || x.FullName != "").ToList();
        }

        private string Parse_Line_Information(string input, string output, string parse_token = ":", string default_string = "")
        {
            List<string> Split_Layer_1 = input.Split(new string[] { parse_token }, StringSplitOptions.None).ToList();
            Split_Layer_1.RemoveAt(0);

            return String.Join(parse_token, Split_Layer_1);
            /*
            foreach (string Info_Pair in Split_Layer_1)
            {
                if (Info_Pair.Contains(output))
                {
                    return Info_Pair.Split(new string[] { ";" }, StringSplitOptions.None)[1];
                }
            }
            //Diagnostics.WriteLine("Potential error with Parse Line info for output: " + output);
            return default_string;
            */
            
        }
    }

    class vCard
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public List<string> Emails { get; set; }
        public List<string> PhoneNumbers { get; set; }
        public string Version { get; set; }
        public string ProductID { get; set; }

        public vCard()
        {
            Emails = new List<string>();
            PhoneNumbers = new List<string>();
        }

        public override string ToString()
        {
            return FirstName + " " + LastName + " (" + FullName + ")";
        }
    }
}
