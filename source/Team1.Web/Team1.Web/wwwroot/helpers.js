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