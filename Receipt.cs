using System;
using System.Collections.Generic;

namespace aiscantech_demo
{

    public class ProcessResult
    {
        public string message { get; set; }
        public string status { get; set; }
        public string token { get; set; }
    }


    public class LineItem
    {

        public decimal price;
        public decimal lineTotal;
        public string desc;
        public int qty;

    }

    public class Receipt
    {
        public decimal total;
        public bool validatedTotal;
        public decimal subTotal;
        public bool validatedSubTotal;
        public decimal cash;
        public decimal change;
        public decimal tax;
        public decimal discount;
        public List<decimal> taxes;
        public List<decimal> discounts;
        public string establishment;
        public string date;
        public List<LineItem> lineItems;

    }

    public class ReceiptResult
    {

        public Receipt result;
        public string message;
        public string status;

    }

}