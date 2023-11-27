function scrollToBottom(el) {
    if (!el)
        return;
    el.scrollTop = el.scrollHeight;
}

function plotlyInit(div, data, layout) {
    console.log(layout);
    Plotly.newPlot(div, data, layout, {responsive: true});
}

async function downloadFileFromStream (fileName, contentStreamReference) {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}