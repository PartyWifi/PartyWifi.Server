using System.Linq;
using System.Threading.Tasks;
using PartyWifi.Server.Model;

namespace PartyWifi.Server.Components
{
    public static class ImageStorage
    {
        public static ImageInfo[] GetAll(IUnitOfWork uow)
        {
            var imageRepo = uow.GetRepository<IImageRepository>();
            var imageInfos = imageRepo.Linq.Select(i => new ImageInfo
            {
                Id = i.Identifier,
                ImageState = (ImageState)i.ImageState,
                Size = i.Size,
                UploadDate = i.UploadDate,
                Versions = i.Versions.Select(v => new ImageVersion((ImageVersions)v.Version, v.Hash)).ToList()
            }).Where(ii => !ii.ImageState.HasFlag(ImageState.Deleted)); 
            
            //TODO: find better way to load from database - maybe load deleted also?

            return imageInfos.ToArray();
        }

        public static ImageEntity Add(IUnitOfWork uow, ImageInfo imageInfo)
        {
            var imageRepo = uow.GetRepository<IImageRepository>();
            var versionRepo = uow.GetRepository<IImageVersionRepository>();
            var imageEntity = imageRepo.Create();

            imageEntity.Identifier = imageInfo.Id;
            imageEntity.Size = imageInfo.Size;
            imageEntity.UploadDate = imageInfo.UploadDate;
            imageEntity.ImageState = (int) imageInfo.ImageState;

            foreach (var version in imageInfo.Versions)
            {
                var versionEntity = versionRepo.Create();
                versionEntity.Version = (int) version.Version;
                versionEntity.Hash = version.Hash;

                imageEntity.Versions.Add(versionEntity);
            }

            return imageEntity;
        }

        public static async Task<ImageEntity> Update(IUnitOfWork uow, ImageInfo imageInfo)
        {
            var imageRepo = uow.GetRepository<IImageRepository>();
            var imageEntity = await imageRepo.GetByIdentifier(imageInfo.Id);
            imageEntity.ImageState = (int)imageInfo.ImageState;

            return imageEntity;
        }
    }
}