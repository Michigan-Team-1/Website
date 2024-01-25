tinyMceConf = {
    height: 400,
    toolbar: 'undo redo | styles | bold italic underline | table | link image paste | emoticons',
    plugins: 'autolink media link image emoticons table paste autoresize',
    paste_data_images: true,
    resize: true,
    min_height: 300,
    paste_postprocess: (editor, args) => {
        args.preventDefault();
        let allImages = args.node.getElementsByTagName("img");
        for (var blah = 0; blah < allImages.length; blah++) {
            let item = allImages[blah];
            if (item.tagName === "IMG" && (item.src.match(/^blob/) || item.src.match(/^data/))) {
                imageResizer(item).then(newImg => {
                    item.src = newImg.src;
                    editor.insertContent(item.outerHTML);
                });
                
            }
        }
    }
}

const maxFileSize = 1.5 * 1024 * 1024; // in Mb

async function imageResizer(imgTag) {
    // get src and see if it too big.
    let blob = await fetch(imgTag.src).then(r => r.blob());
    console.log(blob);
    if (blob.size < maxFileSize) {
        // nothing to do
        return imgTag;
    }

    var ratio = maxFileSize / blob.size;
    console.log(ratio);
    let canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');

    canvas.width = imgTag.width * ratio;
    canvas.height = imgTag.height * ratio;
    ctx.drawImage(imgTag, 0, 0, canvas.width, canvas.height);
    
    imgTag.src = canvas.toDataURL();
    return imgTag;
}