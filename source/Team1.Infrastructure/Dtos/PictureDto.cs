using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Team1.Infrastructure.Dtos.Interfaces;
using Team1.Model;

namespace Team1.Infrastructure.Dtos
{
    public class PictureDto : PictureBase, IFileUpload
    {
        public bool IsUpdated { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        public string FileKey { get; set; }
        [Display(Name = "Picture Upload")]
        public DocumentObjDto Document { get; set; }
        [JsonIgnore]
        public IFormFile FileUpload { get; set; }
    }
}
