// service call error general helper

export function getErrorMessageFromServerResponse(error: any): string {
  let result = "";
  if (error.error != null && error.error.value != null) result = error.error.value;
  else if (error.error != null && error.error.length < 1000) result = error.error;
  else if (error.message != null) result = error.message;
  else result = error;

  if (result.startsWith('"')) result = JSON.parse(result);
  return result;
}

export function escapeRegExp(text: string) {
  return text.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');
}

/**
 * based on convention of IFileUpload
 * @param dto data that contains the file upload
 */
export function getFormDataForDtoAndFileUpload(dto: any) {
  var formData = new FormData();
  if (dto.fileUpload) {
    var fileKey = `file0`;
    formData.append(fileKey, dto.fileUpload);
    dto.fileKey = fileKey;
  }
  formData.append("model", JSON.stringify(dto));
  return formData;
}

/**
 * based on convention of IFileUpload
 * @param dtos data that contains the file upload
 */
export function getFormDataForDtosAndFileUploads(dtos: Array<any>): FormData {
  var formData = new FormData();
  for (var i = 0; i < dtos.length; i++) {
    if (dtos[i].fileUpload) {
      var fileKey = `file${i}`;
      formData.append(fileKey, dtos[i].fileUpload);
      dtos[i].fileKey = fileKey;
    }
  }
  formData.append("model", JSON.stringify(dtos));
  return formData;
}

export function parseFilenameFromContentDisposition(contentDisposition: string | null): string {
  if (contentDisposition == null) return "filenameNotParsed";
  var pieces = contentDisposition.split(';');
  for (var i = 0; i < pieces.length; i++) {
    if (pieces[i].indexOf("filename=") > -1) {
      return pieces[i].replace("filename=", "").replace(/"/g,"").trim();
    }
  }

  return "filenameNotParsed";
}

export function getRandomNameSuffix() : string {
  return new Date().getTime().toString() + getRandomNumber(0, 999).toString();
}

/**
 * Getting a random integer between two values, inclusive
 * @param min
 * @param max
 */
export function getRandomNumber(min: number, max: number) : number{
  min = Math.ceil(min);
  max = Math.floor(max);
  return Math.floor(Math.random() * (max - min + 1)) + min;
}
