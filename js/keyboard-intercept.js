document.addEventListener("keydown", function(event) {
    if (!event.ctrlKey) {
        event.preventDefault();
    }
    try {
        DotNet.invokeMethodAsync('InspiredCalculator', 'Input.KeyboardIntercept', event.ctrlKey, event.shiftKey, event.code);
    } catch (e) {
        console.log(e);
    }
});