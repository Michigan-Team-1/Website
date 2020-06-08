import * as moment from 'moment';

/**
     * exports a csv Blob
     * @param rows data rows
     * @param headerColumns header columns, optional.  Default get properties from the data rows
     * @param showHeaderRow first row would be the header row
     */
export function exportToCsvBlob(rows: Array<any>, headerColumns: Array<string>, showHeaderRow: boolean): Blob {
  var processRow = function (headerColumns: Array<string>, row: any) {
    var finalVal = '';
    for (var j = 0; j < headerColumns.length; j++) {
      var propertyName = headerColumns[j];
      var innerValue = row[propertyName] == null ? '' : row[propertyName].toString();

      if (row[propertyName] instanceof Date) {
        innerValue = moment(row[propertyName].toISOString(), moment.ISO_8601).format("MM-DD-YYYY hh:mm:ss");
      }
      else if (!isNaN(row[propertyName])) {
        // do nothing
      }
      else {
        let possibleDate = moment(row[propertyName], moment.ISO_8601);
        if (possibleDate.isValid()) {
          innerValue = possibleDate.format("MM-DD-YYYY hh:mm:ss");
        }
      }
      var result = innerValue.replace(/"/g, '""');
      if (result.search(/("|,|\n)/g) >= 0)
        result = '"' + result + '"';
      if (j > 0)
        finalVal += ',';
      finalVal += result;
    }
    return finalVal;
  };

  var csvFile = [];
  // get properties to be headers if headerColumns not provided
  if (!headerColumns && rows != null && rows.length > 0) {
    headerColumns = [];
    for (let p in rows[0]) {
      headerColumns.push(p);
    }
  }
  if (showHeaderRow) {
    csvFile.push(headerColumns.join(","));
  }
  for (var i = 0; i < rows.length; i++) {
    csvFile.push(processRow(headerColumns, rows[i]));
  }

  return new Blob([csvFile.join("\n")], { type: 'text/csv;charset=utf-8;' });
}
