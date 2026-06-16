namespace slowfit.DTORequest;

public sealed class ProgressNutritionRes
{
    public int Id { get; set; }

    public int NutritionId { get; set; }

    public int ProgressValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime DateOfProgress { get; set; }

    public int? AvarageKcal { get; set; }

    public int UserId { get; set; }
}
