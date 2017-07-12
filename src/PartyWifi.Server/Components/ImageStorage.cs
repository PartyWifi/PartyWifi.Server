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
            var imageInfos = imageRepo.Linq.Select(i => new ImageInfo(i.Identifier)
            {
                ImageState = (ImageState)i.ImageState,
                UploadDate = i.UploadDate,
                Versions = i.Versions.Select(v => new ImageVersion((ImageVersions)v.Version, v.Size, v.Hash)).ToList()
            }).OrderBy(i => i.UploadDate); 
            
            return imageInfos.ToArray();
        }

        public static ImageEntity Add(IUnitOfWork uow, ImageInfo imageInfo)
        {
            var imageRepo = uow.GetRepository<IImageRepository>();
            var versionRepo = uow.GetRepository<IImageVersionRepository>();
            var imageEntity = imageRepo.Create();

            imageEntity.Identifier = imageInfo.Identifier;
            imageEntity.UploadDate = imageInfo.UploadDate;
            imageEntity.ImageState = (int) imageInfo.ImageState;

            foreach (var version in imageInfo.Versions)
            {
                var versionEntity = versionRepo.Create();
                versionEntity.Version = (int) version.Version;
                versionEntity.Size = version.Size;
                versionEntity.Hash = version.Hash;

                imageEntity.Versions.Add(versionEntity);
            }

            return imageEntity;
        }

        public static async Task<ImageEntity> Update(IUnitOfWork uow, ImageInfo imageInfo)
        {
            var imageRepo = uow.GetRepository<IImageRepository>();
            var imageEntity = await imageRepo.GetByIdentifier(imageInfo.Identifier);
            imageEntity.ImageState = (int)imageInfo.ImageState;

            return imageEntity;
        }
    }
}