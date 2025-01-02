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