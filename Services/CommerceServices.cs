using Microsoft.EntityFrameworkCore;
using slowfit.DBModels;
using slowfit.DTORequest;

namespace slowfit.Services;

public sealed class ProductService(SlowFitContext context) : CrudServiceBase<Product, ProductRes>(context)
{
    protected override DbSet<Product> Set => ((SlowFitContext)DbContext).Products;
    protected override string EntityCode => "product";
    protected override string EntityName => "Product";
    protected override int GetId(Product entity) => entity.ProductId;
    protected override int GetDtoId(ProductRes dto) => dto.ProductId;
    protected override ProductRes ToDto(Product entity) => new() { ProductId = entity.ProductId, Name = entity.Name, Price = entity.Price, ExpirationDate = entity.ExpirationDate, Description = entity.Description };
    protected override Product CreateEntity(ProductRes dto) => new() { Name = dto.Name.Trim(), Price = dto.Price, ExpirationDate = dto.ExpirationDate.Date, Description = dto.Description.Trim(), TypePlanId = 1 };
    protected override void UpdateEntity(Product entity, ProductRes dto)
    {
        entity.Name = dto.Name.Trim();
        entity.Price = dto.Price;
        entity.ExpirationDate = dto.ExpirationDate.Date;
        entity.Description = dto.Description.Trim();
    }
    protected override bool IsValid(ProductRes dto) => dto != null && !string.IsNullOrWhiteSpace(dto.Name) && !string.IsNullOrWhiteSpace(dto.Description) && dto.Price > 0 && dto.ExpirationDate != default;
}

public interface IOrderService : ICrudService<OrderRes>
{
    Task<ServiceResult<IReadOnlyList<OrderRes>>> GetByUserAsync(int userId);
}

public sealed class OrderService(SlowFitContext context) : CrudServiceBase<Order, OrderRes>(context), IOrderService
{
    protected override DbSet<Order> Set => ((SlowFitContext)DbContext).Orders;
    protected override string EntityCode => "order";
    protected override string EntityName => "Order";
    protected override int GetId(Order entity) => entity.OrderId;
    protected override int GetDtoId(OrderRes dto) => dto.OrderId;
    protected override OrderRes ToDto(Order entity) => new() { OrderId = entity.OrderId, ProductId = entity.ProductId, PaymentTypeId = entity.PaymentTypeId, UserId = entity.UserId, Amount = entity.Amount };
    protected override Order CreateEntity(OrderRes dto) => new() { ProductId = dto.ProductId, PaymentTypeId = dto.PaymentTypeId, UserId = dto.UserId, Amount = dto.Amount };
    protected override void UpdateEntity(Order entity, OrderRes dto)
    {
        entity.ProductId = dto.ProductId;
        entity.PaymentTypeId = dto.PaymentTypeId;
        entity.UserId = dto.UserId;
        entity.Amount = dto.Amount;
    }
    protected override bool IsValid(OrderRes dto) => dto != null && dto.ProductId > 0 && dto.PaymentTypeId > 0 && dto.UserId > 0 && dto.Amount > 0;

    public async Task<ServiceResult<IReadOnlyList<OrderRes>>> GetByUserAsync(int userId)
    {
        var entities = await Set.AsNoTracking().Where(o => o.UserId == userId).ToListAsync();
        var orders = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<OrderRes>>.Ok(orders);
    }
}

public interface IBillingService : ICrudService<BillingRes>
{
    Task<ServiceResult<IReadOnlyList<BillingRes>>> GetByUserAsync(int userId);
}

public sealed class BillingService(SlowFitContext context) : CrudServiceBase<Billing, BillingRes>(context), IBillingService
{
    protected override DbSet<Billing> Set => ((SlowFitContext)DbContext).Billings;
    protected override string EntityCode => "billing";
    protected override string EntityName => "Billing";
    protected override int GetId(Billing entity) => entity.BillingId;
    protected override int GetDtoId(BillingRes dto) => dto.BillingId;
    protected override BillingRes ToDto(Billing entity) => new() { BillingId = entity.BillingId, PaymentTypeId = entity.PaymentTypeId, OrderId = entity.OrderId, UserId = entity.UserId, Date = entity.Date, Amount = entity.Amount };
    protected override Billing CreateEntity(BillingRes dto) => new() { PaymentTypeId = dto.PaymentTypeId, OrderId = dto.OrderId, UserId = dto.UserId, Date = dto.Date.Date, Amount = dto.Amount };
    protected override void UpdateEntity(Billing entity, BillingRes dto)
    {
        entity.PaymentTypeId = dto.PaymentTypeId;
        entity.OrderId = dto.OrderId;
        entity.UserId = dto.UserId;
        entity.Date = dto.Date.Date;
        entity.Amount = dto.Amount;
    }
    protected override bool IsValid(BillingRes dto) => dto != null && dto.PaymentTypeId > 0 && dto.OrderId > 0 && dto.UserId > 0 && dto.Date != default && dto.Amount > 0;

    public async Task<ServiceResult<IReadOnlyList<BillingRes>>> GetByUserAsync(int userId)
    {
        var entities = await Set.AsNoTracking().Where(b => b.UserId == userId).ToListAsync();
        var billings = entities.Select(ToDto).ToList();
        return ServiceResult<IReadOnlyList<BillingRes>>.Ok(billings);
    }
}
