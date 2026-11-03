using ArabazonPlatform.Generic.Generic;
using ArabazonPlatform.Application.Model.Settings;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Queries;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
using static ArabazonPlatform.Domain.Enum.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;
using File = ArabazonPlatform.Domain.Domains.DMS.File;

namespace ArabazonPlatform.Application.Caching
{
    public class CachingDataFromDMS
    {
        private Dictionary<PlatformSettingsCode, List<FileCreationModel>> DatasFromDMS { get; set; }
        private IUnitOfWork _unitOfWork { get; set; }

        private CachingPlatformSettings _cachingPlatformSettings;
        public CachingDataFromDMS(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cachingPlatformSettings = new CachingPlatformSettings(_unitOfWork);
            this.GetDataFromDMSAsync();
        }

        private async Task GetDataFromDMSAsync()
        {
            try
            {
                if (!_cachingPlatformSettings.isLodingData)
                {
                    Task.Delay(500).Wait();
                }

                DatasFromDMS = new Dictionary<PlatformSettingsCode, List<FileCreationModel>>();
                string pathFromFolderConfiguration = _cachingPlatformSettings[PlatformSettingsCode.DMSRootFolderPath];
                List<string> filePaths = new List<string>()
                {
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSAdPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSUserPath]}",
                     $"{_cachingPlatformSettings[PlatformSettingsCode.DMSShopPath]}"
                };
                IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(x => filePaths.Contains(x.Folder.ParentFolder.Name),
                 includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>()
                 {
                        z => z.Folder,
                        z => z.FileType
                 });
                List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);
                List<FileCreationModel> fileCreationModels = new List<FileCreationModel>();
                if (files.Any())
                {
                    files.ForEach(async file =>
                    {
                        string filePath = $"{file.Path}\\{file.Name}.{file.FileType.Extension}";
                        byte[] fileInByte = System.IO.File.Exists(filePath) ? System.IO.File.ReadAllBytes(filePath) : [];
                        int startIndex = file.Path.IndexOf(pathFromFolderConfiguration) + pathFromFolderConfiguration.Length;
                        int pathLength = file.Path.Length;
                        int length = pathLength - startIndex;

                        if(fileInByte.Length > 0)
                        {
                            fileCreationModels.Add(new FileCreationModel()
                            {
                                FileType = file.FileType.Extension,
                                FileName = file.Name,
                                FolderName = file.Folder.Name,
                                FilePath = file.Path.Substring(startIndex, length),
                                FileTypeId = file.FileTypeId,
                                File = string.Empty,
                                FileContent = await ConvertByteToString64(fileInByte, (file.FileTypeId == (int)Enums.FileTypes.SVG ? "svg+xml" : file.FileType.Extension))
                            });
                        }

                    });
                    DatasFromDMS = new Dictionary<PlatformSettingsCode, List<FileCreationModel>>();
                    this.DatasFromDMSFilling(fileCreationModels);
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_CachingDataFromDMS.txt", true))
                {
                    outputFile.WriteLine($"{nameof(GetDataFromDMSAsync)}: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                    outputFile.WriteLine("*****************");
                }
            }
        }

        private async Task<string> ConvertByteToString64(byte[] file, string extension)
        {
            if (file.Any())
            {
                if (extension == "pdf")
                {
                    return $"data:application/{extension};base64,{Convert.ToBase64String(file)}";
                }
                return $"data:image/{extension};base64,{Convert.ToBase64String(file)}";
            }
            return null;
        }

        private async Task DatasFromDMSFilling(List<FileCreationModel> fileCreationModels)
        {
            try
            {
                DatasFromDMS.Add(PlatformSettingsCode.DMSShopItemPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSAdPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSBrandPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSCategoryPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSCountryPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSCustomerPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSPaymentMethodPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSUserPath, new List<FileCreationModel>());
                DatasFromDMS.Add(PlatformSettingsCode.DMSShopPath, new List<FileCreationModel>());

                foreach (FileCreationModel item in fileCreationModels)
                {
                    if(item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSShopItemPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSShopItemPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSAdPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSAdPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSBrandPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSBrandPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSCategoryPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSCategoryPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSCountryPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSCountryPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSCustomerPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSCustomerPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSPaymentMethodPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSPaymentMethodPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSUserPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSUserPath].Add(item);
                    }
                    else if (item.FilePath.Contains(_cachingPlatformSettings[PlatformSettingsCode.DMSShopPath]))
                    {
                        DatasFromDMS[PlatformSettingsCode.DMSShopPath].Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                using (System.IO.StreamWriter outputFile = new System.IO.StreamWriter(System.AppDomain.CurrentDomain.BaseDirectory + "\\_DatasFromDMSFilling.txt", true))
                {
                    outputFile.WriteLine($"{nameof(DatasFromDMSFilling)}: {ex.Message}");
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        outputFile.WriteLine(ex.Message);
                    }
                    outputFile.WriteLine("*****************");
                }
            }
        }

        private List<FileCreationModel> GetDMSDataByFilePaths(PlatformSettingsCode platformSettingsCode, List<string> filePaths)
        {
            if (DatasFromDMS == null)
            {
                GetDataFromDMSAsync().Wait();
            }
            else
            {
                return DatasFromDMS[platformSettingsCode];
            }
            return DatasFromDMS[platformSettingsCode].Where(x => filePaths.Any(p => p == x.FilePath)).ToList();
        }

        public List<FileCreationModel> this[PlatformSettingsCode platformSettingsCode, List<string> filePaths]
        {
            get => GetDMSDataByFilePaths(platformSettingsCode, filePaths);
        }

        private List<FileCreationModel> GetDMSDataByFolderPath(PlatformSettingsCode platformSettingsCode, string folderPath)
        {
            if (DatasFromDMS == null)
            {
                GetDataFromDMSAsync().Wait();
            }
            return DatasFromDMS[platformSettingsCode].Where(x => x.FilePath == folderPath).ToList();
        }

        public List<FileCreationModel> this[PlatformSettingsCode platformSettingsCode, string folderPath]
        {
            get => GetDMSDataByFolderPath(platformSettingsCode, folderPath);
        }

        public void RefreshCachingData()
        {
            this.GetDataFromDMSAsync();
        }

    }
}
