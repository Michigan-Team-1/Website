/**
    * gives a download for the blob
    * @param blob blob to provide a download for
    * @param fileName file name for the blob download
    */
export function blobToFile(blob: Blob, fileName: string) {
  if (navigator.msSaveOrOpenBlob) {
    return navigator.msSaveOrOpenBlob(blob, fileName);
    }

  var a = <HTMLAnchorElement>document.createElement("a");
  document.body.appendChild(a);
  a.style.display = "none";
  var url = URL.createObjectURL(blob);
  a.href = url;
  a.download = fileName;
  a.click();
  a.remove();
}
