document.addEventListener("keydown", function(event) {
    var focused = document.activeElement;
    if (focused.tagName === "INPUT") {
        // Don't listen for input if in a text area
        return;
    }
    if (!event.ctrlKey) {
        event.preventDefault();
    }

    try {
        DotNet.invokeMethodAsync('InspiredCalculator', 'Input.KeyboardIntercept', event.ctrlKey, event.shiftKey, event.code);
    } catch (e) {
        console.log(e);
    }
});