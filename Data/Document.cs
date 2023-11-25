namespace InspiredCalculator;

public class Document {
    public string? Title {get; set;}    
    public History History {get; private set;} = new History();
}