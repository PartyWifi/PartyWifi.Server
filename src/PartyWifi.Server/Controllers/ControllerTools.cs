using System;
using System.Linq;
using PartyWifi.Server.Components;
using PartyWifi.Server.Models;

namespace PartyWifi.Server.Controllers
{
    /// <summary>
    /// Static class of logic shared between controllers
    internal static class ControllerTools
    {
        /// <summary>
        /// Build file list for a certain page as extension on the 
        /// image manager.
        /// </summary>
        public static TFileList FileList<TFileList>(this IImageManager imageManager, int page, int filesPerPage)
          where TFileList : FileList, new()
        {
            var total = imageManager.ImageCount;
            var start = (page - 1) * filesPerPage;

            // Get range of files
            var files = imageManager.GetRange(start, filesPerPage).Select(imageInfo => new FileListEntry
            {
                Id = imageInfo.Id,
                Size = imageInfo.Size,
                UploadDate = imageInfo.UploadDate
            }).ToArray();

            return new TFileList 
            {
                Files = files,
                CurrentPage = page,
                Pages = (int)(Math.Ceiling((double)total / filesPerPage))
            };
        }
    }
}