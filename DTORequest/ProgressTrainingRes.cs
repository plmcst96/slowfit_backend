namespace slowfit.DTORequest;

public sealed class ProgressTrainingRes
{
    public int Id { get; set; }

    public int TrainingId { get; set; }

    public int ProgressValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime DateOfProgress { get; set; }

    public int? AvarageKg { get; set; }

    public int UserId { get; set; }

    public int Duration { get; set; }
}
