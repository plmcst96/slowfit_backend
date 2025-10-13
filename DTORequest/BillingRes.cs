namespace slowfit.DTORequest
{
    public class BillingRes
    {
        public int BillingId { get; set; }

        public int PaymentTypeId { get; set; }

        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public decimal Amount { get; set; }

    }
}
