namespace Real_Estate_Laba.Models;

public class Property
{
    public int Id { get; set; }
    public string Status { get; set; }
    public string Type { get; set; }
    public int BedroomCount { get; set; }
    public int BathroomCount { get; set; }
    public double Price { get; set; }
    public double Area { get; set; }
    public Location? Location { get; set; }
    public string AgentId { get; set; }
    public Image? Image { get; set; }
}
