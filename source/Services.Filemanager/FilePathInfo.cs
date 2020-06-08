using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.FileManager
{
	/// <summary>
	/// Stores information related to a file path. Used so that a PathManager and UrlManager can be passed the same object and return the appropriate path/URL.
	/// </summary>
	public class FilePathInfo
	{
		string path;

		/// <summary>
		/// Path relative to the root of where files are stored, e.g. "subfolder/subfolder".
		/// </summary>
		public string Path
		{
			get { return path; }
			set { path = value.Trim('/', '\\'); }
		}

		/// <summary>
		/// Name of the file.
		/// </summary>
		public string FileName { get; set; }

		/// <summary>
		/// MIME type. Used in cases (such as Azure) where saving a file also requires a MIME type to be specified.
		/// </summary>
		public string MimeType { get; set; }
	}
}
