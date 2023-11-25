using System.Globalization;
using Microsoft.JSInterop;

namespace InspiredCalculator.Js;

public abstract class JSInterop {
    protected IJSRuntime JavaScript {get; private set;}

    public JSInterop(IJSRuntime js) {
        this.JavaScript = js;
    }
} 

public class Alerts : JSInterop {
    public Alerts(IJSRuntime js) : base(js) {}

    public async Task Alert(string message) {
        await JavaScript.InvokeVoidAsync("alert", message);
    }

    public async Task<string> Ask(string message, string @default = "") {
        return await JavaScript.InvokeAsync<string>("prompt", message, @default);
    }

    public async Task<T?> Ask<T>(string message, string @default = "") where T:IParsable<T> {
        var str = await JavaScript.InvokeAsync<string>("prompt", message, @default);
        T.TryParse(str, CultureInfo.CurrentCulture, out T? value);
        return value;
    }

    public async Task<bool> Confirm(string message) {
        return await JavaScript.InvokeAsync<bool>("confirm", message);
    }
}