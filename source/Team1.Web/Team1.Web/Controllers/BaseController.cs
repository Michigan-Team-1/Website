using Microsoft.AspNetCore.Mvc;
using Team1.Entities;
using Team1.Infrastructure.Services;
using Team1.Infrastructure.UserIdentity;
using Team1.Model.Attributes;

namespace Team1.Web.Controllers;

/// <summary>
/// Controller all other controllers are based on
/// </summary>
[TypeWriterIgnore]
[ApiExplorerSettings(IgnoreApi = false)]
public class BaseController : Controller
{
  private UserPermissionService? _userPermissionService;

  /// <summary>
  /// User permission service.  It is injected
  /// </summary>
  public UserPermissionService UserPermissionService
  {
    get
    {
      if (_userPermissionService == null)
        _userPermissionService = HttpContext.RequestServices.GetService(typeof(UserPermissionService)) as UserPermissionService;

      return _userPermissionService!;
    }
  }

  /// <summary>
  /// Base Url of the site
  /// </summary>
  public string? BaseUrl
  {
    get
    {
      if (Request == null)
        return null;

      return $"{Request.Scheme}://{Request.Host.ToString()}/";
    }
  }

  private DataContext? _dataContext;
  /// <summary>
  /// Data context injected in so we can do transactions easier.
  /// </summary>
  public DataContext DataContext
  {
    get
    {
      if (_dataContext == null)
      {
        _dataContext = HttpContext.RequestServices.GetService(typeof(DataContext)) as DataContext;
      }

      return _dataContext!;
    }
  }

  private ILoggingContext? _loggingContext;
  /// <summary>
  /// Data context used for logging only
  /// </summary>
  public ILoggingContext LoggingContext
  {
    get
    {
      if (_loggingContext == null)
      {
        _loggingContext = HttpContext.RequestServices.GetService(typeof(ILoggingContext)) as ILoggingContext;
      }

      return _loggingContext!;
    }
  }

  /// <summary>
  /// Looks a the response from the service and creates the corresponding response.
  /// </summary>
  /// <typeparam name="T">Data object that would be passed back on OK</typeparam>
  /// <param name="response">BaseServiceResponse</param>
  protected IActionResult CreateResponse<T>(BaseServiceResponse<T> response)
  {
    // there are many status codes
    // add http status codes as needed
    switch (response.Status)
    {
      case System.Net.HttpStatusCode.BadRequest:
        // if message is empty then check ModelState
        if (string.IsNullOrEmpty(response.Message) && !ModelState.IsValid)
        {
          response.Message = string.Join("<br/>", (from ms in ModelState
                                                   from e in ms.Value.Errors
                                                   select $"{e.ErrorMessage}"));
        }
        return StatusCode((int)response.Status, response.Message);
      case System.Net.HttpStatusCode.Conflict:
      case System.Net.HttpStatusCode.NotImplemented:
      case System.Net.HttpStatusCode.Unauthorized:
        return StatusCode((int)response.Status, response.Message);
      case System.Net.HttpStatusCode.OK:
        return Ok(response.Data);
    }
    return null;
  }

  /// <summary>
  /// Able to get an injected service and pre setup the Data context and User permission service
  /// </summary>
  /// <typeparam name="T">Type of the service</typeparam>
  /// <returns>returns the service</returns>
  protected T GetService<T>() where T : BaseService
  {
    var service = (T)HttpContext.RequestServices.GetService(typeof(T));
    service.SetupService(DataContext, LoggingContext, UserPermissionService);
    return service;
  }

  /// <summary>
  /// Gets the data object and file and sets the IFormFile to the data object
  /// </summary>
  /// <typeparam name="T">return Type</typeparam>
  //protected T GetPostedFileAndData<T>() where T : IFileUpload
  //{
  //    var model = Request.Form["model"].ToString();
  //    var dto = model.DeserializeJson<T>();

  //    if (Request.Form.Files != null && Request.Form.Files.Count > 0)
  //    {
  //        dto.FileUpload = Request.Form.Files[0];
  //    }

  //    TryValidateModel(dto);
  //    return dto;
  //}

  /// <summary>
  /// Gets the data object list and files and sets the IFormFile to the correct data object
  /// </summary>
  //protected X GetPostedFilesAndData<X, Y>() where X : IList<Y> where Y : IFileUpload
  //{
  //    var model = Request.Form["model"].ToString();
  //    var dto = model.DeserializeJson<X>();

  //    if (Request.Form.Files != null && Request.Form.Files.Count > 0)
  //    {
  //        for (int i = 0; i < Request.Form.Files.Count; i++)
  //        {
  //            var formFile = Request.Form.Files[i];
  //            var correctDto = dto.Single(w => w.FileKey == formFile.Name);
  //            correctDto.FileUpload = formFile;
  //        }
  //    }

  //    TryValidateModel(dto);
  //    return dto;
  //}
}
