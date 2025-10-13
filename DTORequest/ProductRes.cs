namespace slowfit.DTORequest
{
    public class ProductRes
    {
        public int ProductId { get; set; }

        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public DateTime ExpirationDate { get; set; }

        public string Description { get; set; } = null!;
    }
}
