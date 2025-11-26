namespace PipeSelectorApp.Models;

public class PipeNomenclature
{
    public int Id { get; set; }
    public double pipe_diameter { get; set; }
    public double pipe_wall { get; set; }

    public string DisplayName => $"{pipe_diameter:F2} x {pipe_wall:F2}";
}