window.setSource = async (elementId, stream, contentType) => {
    const arrayBuffer = await stream.arrayBuffer();
    let blobOptions = {};
    if (contentType) {
        blobOptions['type'] = contentType;
    }
    const blob = new Blob([arrayBuffer], blobOptions);
    const url = URL.createObjectURL(blob);
    const element = document.getElementById(elementId);
    element.classList.remove("d-none");

    element.onload = () => {
        URL.revokeObjectURL(url);
    }
    element.src = url;
}

function carouselInit(elementSelector) {
    var flkty = new Flickity(elementSelector, {
        wrapAround: true,
        groupCells: true,
        autoPlay: true,
        pageDots: false,
        lazyLoad: true
    });

    flkty.on('dragStart', flickityDragStart);
    flkty.on('settle', flickitySettle);
}

function flickityDragStart() {
    var jsObjectReference = DotNet.createJSObjectReference(window);
    DotNet.invokeMethodAsync('Team1.Web.Client', 'DragStarted', jsObjectReference);
    DotNet.disposeJSObjectReference(jsObjectReference);
}

function flickitySettle() {
    var jsObjectReference = DotNet.createJSObjectReference(window);
    DotNet.invokeMethodAsync('Team1.Web.Client', 'Settled', jsObjectReference);
    DotNet.disposeJSObjectReference(jsObjectReference);
}

function BlazorDownloadFile(filename, contentType, content) {
    // Create the URL
    const file = new File([content], filename, { type: contentType });
    const exportUrl = URL.createObjectURL(file);

    // Create the <a> element and click on it
    const a = document.createElement("a");
    document.body.appendChild(a);
    a.href = exportUrl;
    a.download = filename;
    a.target = "_self";
    a.click();

    // We don't need to keep the object URL, let's release the memory
    // On older versions of Safari, it seems you need to comment this line...
    URL.revokeObjectURL(exportUrl);
}