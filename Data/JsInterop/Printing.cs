using Microsoft.JSInterop;

namespace InspiredCalculator.Js;

public class Printing : JSInterop {

    public Printing(IJSRuntime js) : base(js) {}

    public async Task PrintPage() {
        await JavaScript.InvokeVoidAsync("window.print");
    }
}