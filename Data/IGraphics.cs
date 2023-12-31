namespace InspiredCalculator;

public interface IGraphics {}

public class PlotGraphic : IGraphics {

    public PlotGraphic(Js.Plotly.Layout layout, params Js.Plotly.Trace[] data) {
        this.Data = data;
        this.Layout = layout;
    }

    public Js.Plotly.Trace[] Data {get; private set;}
    public Js.Plotly.Layout Layout {get; private set;}
}

public class ImageUrl : IGraphics {
    public string Url {get; private set;}
    public ImageUrl(string url) {
        this.Url = url;
    }
}