using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Team1.Infrastructure.Dtos.Interfaces
{
    public interface IFileUpload
    {
        string FileKey { get; set; }
        DocumentObjDto Document { get; set; }

        [JsonIgnore]
        IFormFile FileUpload { get; set; }
    }
}
