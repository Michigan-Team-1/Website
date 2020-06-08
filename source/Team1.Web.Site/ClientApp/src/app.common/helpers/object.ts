/**
 * compares two objects to see if there are any differences.  newObj will isUpdated set to true if there is a difference.
 * Sets to false if there is no difference
 * does a deep check
 * @param newObj new object
 * @param oldObj original object
 * @param propertiesToIgnore list of properties to ignore
 */
export function setIsUpdatedIfChanged(newObj: any, oldObj: any, propertiesToIgnore: string[] = []): boolean {
    var isUpdatedProp = "isUpdated";
    let hasUpdatedProperty = newObj.hasOwnProperty(isUpdatedProp);

    if (newObj && !oldObj) {
        if (hasUpdatedProperty) newObj.isUpdated = true;
        return true;
    }

    // concurrencyStamp is always unique, must be ignored
  propertiesToIgnore.push("concurrencyStamp");
  propertiesToIgnore.push(isUpdatedProp);

  var result = false;

    for (var property in newObj) {
      if (propertiesToIgnore.some((item, index) => { return item == property; }))
            continue
        switch (Object.prototype.toString.call(newObj[property])) {
            case "[object Object]":
                if (setIsUpdatedIfChanged(newObj[property], oldObj[property], propertiesToIgnore)) {
                    if (hasUpdatedProperty) newObj.isUpdated = true;
                    result = true;
                }
                else {
                  if (hasUpdatedProperty) newObj.isUpdated = false;
                }
                break;
            case "[object Array]":
                var newLength = newObj[property].length;
                var arrayElementHasUpdatedProperty = false;
                if (newLength > 0) {
                    arrayElementHasUpdatedProperty = newObj[property][0].hasOwnProperty(isUpdatedProp)
                }
                if (oldObj[property] == null || newLength !== oldObj[property].length) {
                    if (hasUpdatedProperty) newObj.isUpdated = true;
                    if (arrayElementHasUpdatedProperty) {
                        // set all of them to updated if arrays are different
                        for (var x = 0; x < newLength; x++) {
                            newObj[property][x].isUpdated = true;
                        }
                    }
                    result = true;
                    continue;
                }

                for (var x = 0; x < newLength; x++) {
                    if (setIsUpdatedIfChanged(newObj[property][x], oldObj[property][x], propertiesToIgnore)) {
                        if (hasUpdatedProperty) newObj.isUpdated = true;
                        result = true;
                    }
                    else {
                      if (hasUpdatedProperty) newObj.isUpdated = false;
                    }
                }
                break;
            case "[object Date]":
                if (!oldObj[property] || newObj[property].getTime() !== oldObj[property].getTime()) {
                    if (hasUpdatedProperty) newObj.isUpdated = true;
                    result = true;
                }
                break;
            case "[object Function]":
                // do nothing
                break;
            default:
                if (newObj[property] != oldObj[property]) {
                    if (hasUpdatedProperty) newObj.isUpdated = true;
                    result = true;
                }
                break;
        }
    }

    if (hasUpdatedProperty) newObj.isUpdated = result;
    return result;
}

export function deepClone(obj:any, hash = new WeakMap()):any {
    if (Object(obj) !== obj) return obj; // primitives
    if (hash.has(obj)) return hash.get(obj); // cyclic reference
    const result = obj instanceof Date ? new Date(obj)
        : obj instanceof RegExp ? new RegExp(obj.source, obj.flags)
            : obj.constructor ? new obj.constructor()
                : Object.create(null);
    hash.set(obj, result);
    if (obj instanceof Map)
        Array.from(obj, ([key, val]) => result.set(key, deepClone(val, hash)));
    return Object.assign(result, ...Object.keys(obj).map(
        key => ({ [key]: deepClone(obj[key], hash) })));
}

export function jsonParseAndToCamelCase(stringToParse: string | undefined): any {
  if (stringToParse == null) return undefined;
  var dataObj = JSON.parse(stringToParse, camelCaseReviver);
  return dataObj;
}

function camelCaseReviver(key: any, value: any) {
  if (value && typeof value === 'object') {
    for (var k in value) {
      if (/^[A-Z]/.test(k) && Object.hasOwnProperty.call(value, k)) {
        value[k.charAt(0).toLowerCase() + k.substring(1)] = value[k];
        delete value[k];
      }
    }
  }
  return value;
}
