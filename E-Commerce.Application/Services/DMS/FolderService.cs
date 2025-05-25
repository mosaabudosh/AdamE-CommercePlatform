using Adam.Generic.Generic;
using Adam.Logging.Interfaces;
using E_Commerce.Application.Interfaces.DMS;
using E_Commerce.Application.Model;
using E_Commerce.Domain.Domains;
using E_Commerce.Domain.Domains.DMS;
using E_Commerce.Domain.Enum;
using E_Commerce.Domain.IHttpServices;
using E_Commerce.Domain.RepositoryInterfaces;
using Generic.Response;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static E_Commerce.Domain.Enum.Enums;
using static System.Net.Mime.MediaTypeNames;
using static System.Net.WebRequestMethods;
using File = E_Commerce.Domain.Domains.DMS.File;

namespace E_Commerce.Application.Services.DMS
{
    public class FolderService : IFolderService
    {
        #region Fields
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileTypeService _fileTypeService;
        private readonly IFolderConfigurationService _folderConfigurationService;
        private readonly IExceptionService _exceptionService;
        private readonly IInformationService _informationService;
        private readonly IUserAccessor _userAccessor;
        #endregion

        #region Constructor
        public FolderService(IUnitOfWork unitOfWork, IFileTypeService fileTypeService, IFolderConfigurationService folderConfigurationService, IExceptionService exceptionService, IUserAccessor userAccessor, IInformationService informationService)
        {
            _unitOfWork = unitOfWork;
            _fileTypeService = fileTypeService;
            _folderConfigurationService = folderConfigurationService;
            _exceptionService = exceptionService;
            _userAccessor = userAccessor;
            _informationService = informationService;
        }
        #endregion


        #region IFolderService
        public async Task<BaseResponse> CreateFolder(Folder folder)
        {
            try
            {
                folder.CreationUserId = _userAccessor.GetCurrentUserId();
                folder.CreationDate = DateTime.UtcNow;
                _unitOfWork.GetRepository<Folder>().Insert(folder);

                await _unitOfWork.CommitChanges();

                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolder), _userAccessor.GetCurrentUserId(), folder);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteFolder(int folderId, bool isCommit = true)
        {
            try
            {
                BaseResponse response = await DeleteFilesByFolderId(folderId);
                if (!response.IsSuccess)
                {
                    return new BaseResponse(false, response.Message);
                }
                IQueryable<Folder> folders = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(z => z.FolderId == folderId);
                Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(folders);
                _unitOfWork.GetRepository<Folder>().Delete(folder);

                if (isCommit)
                {
                    await _unitOfWork.CommitChanges();
                }
                await DeletePhsiycalFolder(folder.Path);
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteFolder), _userAccessor.GetCurrentUserId(), folderId);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<Folder>> GetFolderByParentPathId(string parentPathId)
        {
            try
            {
                Folder folder = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(p => p.Path == parentPathId).FirstOrDefault();
                return new BaseResponse<Folder>(true, string.Empty, folder);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderByParentPathId), _userAccessor.GetCurrentUserId(), parentPathId);
                return new BaseResponse<Folder>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<Folder>>> GetFoldersByParentFolderId(int parentFolderId)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(z => z.ParentFolderId == parentFolderId, includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files });
                List<Folder> folders = await _unitOfWork.GetRepository<Folder>().ToListAsync(foldersQuery);
                return new BaseResponse<List<Folder>>(true, string.Empty, folders);
            }
            catch (Exception ex) 
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFoldersByParentFolderId), _userAccessor.GetCurrentUserId(), parentFolderId);
                return new BaseResponse<List<Folder>>(false, ex.Message);
            }     
        }

        public async Task<BaseResponse<Folder>> GetFolderByFolderId(int folderId)
        {
            try
            {
                Folder folder = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(z => z.FolderId == folderId).SingleOrDefault();
                return new BaseResponse<Folder>(true,string.Empty, folder);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderByFolderId), _userAccessor.GetCurrentUserId(), folderId);
                return new BaseResponse<Folder>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<Folder>> GetFolderById(int folderId)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(z => z.FolderId == folderId,
                    includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files });
                Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(foldersQuery);
                return new BaseResponse<Folder>(true, string.Empty, folder);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderById), _userAccessor.GetCurrentUserId(), folderId);
                return new BaseResponse<Folder>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<Folder>>> GetRootFolders()
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(z => !z.ParentFolderId.HasValue);
                List<Folder> folders = await _unitOfWork.GetRepository<Folder>().ToListAsync(foldersQuery);
                return new BaseResponse<List<Folder>>(true,string.Empty,folders);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetRootFolders), _userAccessor.GetCurrentUserId());
                return new BaseResponse<List<Folder>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<Folder>>> GetFolderByPath(string folderPath)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpression(x => x.Path.Contains(folderPath));
                List<Folder> folders = await _unitOfWork.GetRepository<Folder>().ToListAsync(foldersQuery);
                return new BaseResponse<List<Folder>>(true, string.Empty, folders);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderByPath), _userAccessor.GetCurrentUserId(), folderPath);
                return new BaseResponse<List<Folder>>(false, ex.Message);
            }            
        }

        public async Task<BaseResponse<Folder>> RenameFolderAsync(int folderId, string newName)
        {
            bool isFolderNameChanged = false;
            BaseResponse<Folder> folder = null;
            string oldPath = null;
            try
            {
                folder = await GetFolderById(folderId);
                if (!folder.IsSuccess)
                {
                    return folder;
                }
                oldPath = folder.DataObject.Path;
                BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.DataObject.Path);
                List<File> files = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(x => x.Path.Contains(folder.DataObject.Path)).ToList();

                folders.DataObject = folders.DataObject.Select(f =>
                {
                    f.Path = f.Path.Replace("\\" + folder.DataObject.Name + "\\", "\\" + newName + "\\");
                    return f;
                }).ToList();

                files = files.Select(f =>
                {
                    f.Path = f.Path.Replace("\\" + folder.DataObject.Name + "\\", "\\" + newName + "\\");
                    return f;
                }).ToList();

                _unitOfWork.GetRepository<Folder>().UpdateRange(folders.DataObject);
                folder.DataObject.Path = folder.DataObject.Path.Replace("\\" + folder.DataObject.Name, "\\" + newName);
                folder.DataObject.Name = newName;
                _unitOfWork.GetRepository<Folder>().Update(folder.DataObject);

                Directory.Move(oldPath, folder.DataObject.Path);
                isFolderNameChanged = true;
             
                await _unitOfWork.CommitChanges();

                BaseResponse<List<Folder>> childFolders = await GetFoldersByParentFolderId(folderId);
                if (!childFolders.IsSuccess)
                {
                    return new BaseResponse<Folder>(false, childFolders.Message,childFolders.ErrorCode);
                }
                folder.DataObject.ChildFolders = childFolders.DataObject;
                return folder;
            }
            catch (Exception ex)
            {
                if (isFolderNameChanged)
                {
                    Directory.Move(folder.DataObject.Path, oldPath);
                }
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(RenameFolderAsync), _userAccessor.GetCurrentUserId(), folderId, newName);
                return new BaseResponse<Folder>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> DeletePhsiycalFolder(string path)
        {
            try
            {
                System.IO.DirectoryInfo di = new DirectoryInfo(path);
                di.Delete(true);
                return new BaseResponse(true);
            }
            catch (System.IO.IOException ioExp)
            {            
                await _exceptionService.LogErrorAsync(ioExp, nameof(DeletePhsiycalFolder), _userAccessor.GetCurrentUserId(), path);
                return new BaseResponse(false, ioExp.Message);
            }
        }

        public async Task<BaseResponse> CreateFolderAndFiles(Folder folder, List<FileCreationModel> fileCreationModels)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
                if (!folderConfigurationPath.IsSuccess)
                {
                    return folderConfigurationPath;
                }
                string pathFromFolderConfiguration = folderConfigurationPath.DataObject.RootFolderPath;
                string folderPath = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                string path = $"{folder.Path}\\{folder.Name}";

                BaseResponse<List<Folder>> oldfolders = await GetFolderByPath($"{pathFromFolderConfiguration}{folder.Path}");
                if (!oldfolders.IsSuccess) 
                {
                    return new BaseResponse(false, oldfolders.Message);
                }
                if (fileCreationModels.Any())
                {
                    if (oldfolders.DataObject.Any())
                    {
                        oldfolders.DataObject.ForEach(async oldFolder =>
                        {
                            if (oldFolder.Name == folder.Name)
                            {
                                BaseResponse responseDelete = await DeleteFolder(oldFolder.FolderId, false);
                            }
                        });
                    }
                    BaseResponse<List<File>> filesResponse = await GenerateFiles(folder, fileCreationModels, folderPath);
                    if (!filesResponse.IsSuccess)
                    {
                        return new BaseResponse(false, filesResponse.Message);
                    }
                    folder.Files = new List<File>(filesResponse.DataObject);
                    folder.CreationUserId = _userAccessor.GetCurrentUserId();
                    folder.IsActive = true;
                    folder.CreationDate = DateTime.UtcNow;

                    string[] paths = path.Split("\\");
                    List<Folder> folders = new List<Folder>();
                    List<Folder> newFolders = new List<Folder>();

                    BaseResponse<List<Folder>> responseFolder = await GenerateFolders(folders, newFolders, folder, paths, pathFromFolderConfiguration);
                    if (!responseFolder.IsSuccess)
                    {
                        return new BaseResponse(false, responseFolder.Message);
                    }
                    _unitOfWork.GetRepository<Folder>().AddRange(responseFolder.DataObject);            

                    await _unitOfWork.CommitChanges();
              
                    return new BaseResponse(true);
                }
                else
                {
                    folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                    BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.Path);
                    if (!folders.IsSuccess) 
                    { 
                        return new BaseResponse(false,folders.Message);
                    }
                    if (folders.DataObject.Any())
                    {
                        folders.DataObject.ForEach(oldFolder =>
                        {
                            BaseResponse responseDelete = DeleteFolder(oldFolder.FolderId).Result;
                        });
                    }
                    return new BaseResponse(true);
                }
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolderAndFiles), _userAccessor.GetCurrentUserId(), folder, fileCreationModels);

                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse> CreateFolderAndFilesPhysically(Folder folder, FileCreationModel fileCreationModel)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
                if (!folderConfigurationPath.IsSuccess)
                {
                    return folderConfigurationPath;
                }
                string pathFromFolderConfiguration = folderConfigurationPath.DataObject.RootFolderPath;
                string path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                if (Directory.Exists(path))
                {
                    if (fileCreationModel is not null)
                    {
                        fileCreationModel.FileName = $"{fileCreationModel.FileName}.{fileCreationModel.FileType}";
                        string newPath = $"{path}\\{fileCreationModel.FileName}";
                        if (!System.IO.File.Exists(newPath))
                        {
                            byte[] fileContentBytes = ConvertString64ToByte(fileCreationModel.FileContent);
                            System.IO.File.WriteAllBytes(newPath, fileContentBytes);
                        }
                        return new BaseResponse(true);
                    }
                }
                else
                {
                    DirectoryInfo di = Directory.CreateDirectory(path);
                    if (fileCreationModel is not null)
                    {
                        fileCreationModel.FileName = $"{fileCreationModel.FileName}\\{fileCreationModel.FileType}";
                        string newPath = $"{path}\\{fileCreationModel.FileName}";
                        if (!System.IO.File.Exists(newPath))
                        {
                            byte[] fileContentBytes = ConvertString64ToByte(fileCreationModel.FileContent);
                            System.IO.File.WriteAllBytes(newPath, fileContentBytes);
                        }
                        return new BaseResponse(true);
                    }
                }
                return new BaseResponse(false, "The file was not created.");
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolderAndFilesPhysically), _userAccessor.GetCurrentUserId(), folder, fileCreationModel);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetFilesByFolderPath(string folderPath)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Path.Contains(folderPath),
                includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files });
                Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(foldersQuery);
                List<FileCreationModel> files = new List<FileCreationModel>();
                if (folder is not null && folder.Files.Any())
                {
                    List<int> fileTypeIds = folder.Files.Select(s => { return s.FileTypeId; }).ToList();
                    List<FileTypeModel> fileTypes = await getExtensionsAsync(fileTypeIds);
                    folder.Files.ForEach(async file =>
                    {
                        string fileType = fileTypes.FirstOrDefault(p => p.FileTypeId == file.FileTypeId).Extension;
                        byte[] fileInByte = System.IO.File.ReadAllBytes($"{file.Path}\\{file.Name}.{fileType}");
                        files.Add(new FileCreationModel()
                        {
                            FileType = fileType,
                            FileName = file.Name,
                            FolderName = folder.Name,
                            FileTypeId = file.FileTypeId,
                            FilePath = file.Path,
                            File = string.Empty,
                            FileContent = ConvertByteToString64(fileInByte, (file.FileTypeId == (int)Enums.FileTypes.SVG ? "svg+xml" : fileType)),
                        });
                    });
                }
                return new BaseResponse<List<FileCreationModel>>(true, string.Empty, files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFilesByFolderPath), _userAccessor.GetCurrentUserId(), folderPath);
                return new BaseResponse<List<FileCreationModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetFolderByPathAndName(string folderPath, string folderName)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Path.Contains(folderPath) && x.Name == folderName,
                includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files });
                Folder folders = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(foldersQuery);
                List<FileCreationModel> images = new List<FileCreationModel>();
                if (folders is not null)
                {
                    if (folders.Files.Any())
                    {
                        folders.Files.ForEach(async file =>
                        {
                            FileCreationModel fileCreationModel = new FileCreationModel();
                            fileCreationModel.FileType = await getExtensionAsync(file.FileTypeId);
                            fileCreationModel.FileName = file.Name;
                            fileCreationModel.FolderId = file.FolderId;
                            fileCreationModel.FolderName = file.Folder.Name;
                            byte[] imageByte = System.IO.File.ReadAllBytes(file.Path + "\\" + fileCreationModel.FileName + "." + fileCreationModel.FileType);
                            fileCreationModel.FileContent = ConvertByteToString64(imageByte, fileCreationModel.FileType);
                            images.Add(fileCreationModel);
                        });
                    }
                }
                return new BaseResponse<List<FileCreationModel>>(true,string.Empty, images);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolderByPathAndName), _userAccessor.GetCurrentUserId(), folderPath, folderName);
                return new BaseResponse<List<FileCreationModel>>(false,ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetImagesByFilePath(string filePath)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
                if (!folderConfigurationPath.IsSuccess)
                {
                    return new BaseResponse<List<FileCreationModel>>(false,folderConfigurationPath.Message,folderConfigurationPath.ErrorCode);
                }
                filePath = $"{folderConfigurationPath.DataObject.RootFolderPath}{filePath}";
                IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(x => x.Path.Contains(filePath),
                includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>() {z => z.FileType });
                List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);
                List<FileCreationModel> images = new List<FileCreationModel>();
                if (files.Any())
                {
                    files.ForEach(async file =>
                    {
                        FileCreationModel fileCreationModel = new FileCreationModel();
                        fileCreationModel.FileType = file.FileType.Extension;
                        fileCreationModel.FileName = file.Name;
                        byte[] imageByte = System.IO.File.ReadAllBytes($"{file.Path}\\{file.Name}.{file.FileType.Extension}");
                        fileCreationModel.FileContent = ConvertByteToString64(imageByte, (file.FileTypeId == (int)FileTypes.SVG ? "svg+xml" : fileCreationModel.FileType));
                        images.Add(fileCreationModel);
                    });
                }
                return new BaseResponse<List<FileCreationModel>>(true, string.Empty, images);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetImagesByFilePath), _userAccessor.GetCurrentUserId(), filePath);
                return new BaseResponse<List<FileCreationModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse> CreateFolderAndOneFile(FolderAndFileCreationModel folderAndFileCreationModel)
        {
            BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
            if (!folderConfigurationPath.IsSuccess)
            {
                return folderConfigurationPath;
            }
            string pathFromFolderConfiguration = folderConfigurationPath.DataObject.RootFolderPath;
            Folder folder = new Folder()
            {
                Name = folderAndFileCreationModel.FolderName,
                Path = folderAndFileCreationModel.FolderPath,
            };
            try
            {
                string folderPath = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                string path = $"{folder.Path}\\{folder.Name}";
                BaseResponse<List<Folder>> oldfolders = await GetFolderByPath($"{pathFromFolderConfiguration}{path}");
                if (!oldfolders.IsSuccess)
                {
                    return new BaseResponse(false, oldfolders.Message);
                }
                if (folderAndFileCreationModel.FileCreationModels.Any())
                {
                    if (oldfolders.DataObject.Any())
                    {
                        oldfolders.DataObject.ForEach(async oldFolder =>
                        {
                            if (oldFolder.Name == folder.Name)
                            {
                                BaseResponse responseDelete = await DeleteFolder(oldFolder.FolderId);
                            }
                        });
                    }
                }

                IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpression(x => x.Path.Contains(folderPath),
               includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>() { z => z.Folder });
                List<File> oldfiles = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);

                if (oldfiles.Any())
                {
                    Folder oldfolder = filesQuery.FirstOrDefault().Folder;
                    if (oldfolder != null)
                    {
                        _unitOfWork.GetRepository<File>().RemoveRange(oldfolder.Files);
                        await DeletePhsiycalFolder(folderPath);
                    }
                }
             
                if (folderAndFileCreationModel.FileCreationModels.Any())
                {
                    BaseResponse<List<File>> filesResponse = await GenerateFiles(folder, folderAndFileCreationModel.FileCreationModels, folderPath);
                    if (!filesResponse.IsSuccess)
                    {
                        return new BaseResponse(false, filesResponse.Message);
                    }
                    folder.Files = new List<File>(filesResponse.DataObject);
                    folder.CreationUserId = _userAccessor.GetCurrentUserId();
                    folder.CreationDate = DateTime.UtcNow;
                    folder.IsActive = true;

                    string[] paths = path.Split("\\");
                    List<Folder> folders = new List<Folder>();
                    List<Folder> newFolders = new List<Folder>();
                    if (paths.Any())
                    {
                        int i = 0;
                        foreach (string newPath in paths)
                        {
                            if (i <= paths.Length - 2)
                            {
                                Folder newFolder = new Folder();
                                newFolder.Path = (i == 0) ? pathFromFolderConfiguration + newPath : folders[i - 1].Path + "\\" + newPath;
                                newFolder.CreationUserId = _userAccessor.GetCurrentUserId();
                                newFolder.IsActive = true;
                                newFolder.Name = newPath;
                                newFolder.CreationDate = DateTime.UtcNow;
                                newFolder.ParentFolderId = (i > 0) ? folders[i - 1].FolderId : newFolder.ParentFolderId;
                                newFolder.ParentFolder = (i > 0) ? folders[i - 1] : null;
                               
                                Folder existFolder = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Path.Contains(newFolder.Path)).FirstOrDefault();
                                if (existFolder == null)
                                {
                                    folders.Add(newFolder);
                                    newFolders.Add(newFolder);
                                }
                                else
                                {
                                    folders.Add(existFolder);
                                }
                            }
                            i++;
                        }
                    }
                    folder.ParentFolder = folders.LastOrDefault();
                    folder.ParentFolderId = folders.LastOrDefault().FolderId;
                    folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                    
                    _unitOfWork.GetRepository<Folder>().Insert(folder);                   

                    await _unitOfWork.CommitChanges();
                                       
                    return new BaseResponse(true);
                }
                else
                {
                    folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                    BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.Path);
                    if (!folders.IsSuccess)
                    {
                        return new BaseResponse(false,folders.Message);
                    }
                    if (folders.DataObject.Any())
                    {
                        folders.DataObject.ForEach(async oldFolder =>
                        {
                            BaseResponse responseDelete = await DeleteFolder(oldFolder.FolderId);
                        });
                    }
                    return new BaseResponse(true, string.Empty);
                }
            }
            catch (Exception ex)
            {
                folder.Path = $"{pathFromFolderConfiguration}{folder.Path}";
                BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.Path);
                if (!folders.IsSuccess)
                {
                    return new BaseResponse(false, folders.Message);
                }
                if (folders.DataObject.Any())
                {
                    folders.DataObject.ForEach(async oldFolder =>
                    {
                        BaseResponse responseDelete = await DeleteFolder(oldFolder.FolderId);
                    });
                }
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolderAndOneFile), _userAccessor.GetCurrentUserId(), folderAndFileCreationModel);

                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetFilesByFilePath(List<string> filePaths)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
                if (!folderConfigurationPath.IsSuccess)
                {
                    return new BaseResponse<List<FileCreationModel>>(false,folderConfigurationPath.Message,folderConfigurationPath.ErrorCode);
                }
                filePaths = filePaths.Select(s => { return $"{folderConfigurationPath.DataObject.RootFolderPath}{s}"; }).ToList();
                IQueryable<File> filesQuery = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(x => filePaths.Contains(x.Path),
                includedEntities: new List<System.Linq.Expressions.Expression<Func<File, object>>>() 
                { 
                    z => z.Folder,
                    z => z.FileType
                });
                List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(filesQuery);
                List<FileCreationModel> images = new List<FileCreationModel>();
                if (files.Any())
                {                 
                    files.ForEach(async file =>
                    {
                        byte[] fileInByte = System.IO.File.ReadAllBytes($"{file.Path}\\{file.Name}.{file.FileType.Extension}");

                        int startIndex = file.Path.IndexOf(folderConfigurationPath.DataObject.RootFolderPath) + folderConfigurationPath.DataObject.RootFolderPath.Length;
                        int pathLength = file.Path.Length;
                        int length = pathLength - startIndex;

                        images.Add(new FileCreationModel()
                        {
                            FileType = file.FileType.Extension,
                            FileName = file.Name,
                            FolderName = file.Folder.Name,
                            FilePath = file.Path.Substring(startIndex, length),
                            FileTypeId = file.FileTypeId,
                            File = string.Empty,
                            FileContent = ConvertByteToString64(fileInByte, (file.FileTypeId == (int)Enums.FileTypes.SVG ? "svg+xml" : file.FileType.Extension)),
                        });
                    });
                }
                return new BaseResponse<List<FileCreationModel>>(true, string.Empty, images);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFilesByFilePath), _userAccessor.GetCurrentUserId(), filePaths);
                return new BaseResponse<List<FileCreationModel>>(false, string.Empty);
            }
        }

        public async Task<BaseResponse> CreateFolderAndFilesWithoutDelete(Folder folder, List<FileCreationModel> fileCreationModels)
        {
            BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
            if (!folderConfigurationPath.IsSuccess)
            {
                return folderConfigurationPath;
            }
            string pathFromFolderConfiguration = folderConfigurationPath.DataObject.RootFolderPath;
            try
            {
                string folderPath = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                Folder existfolder = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Name == folder.Name && x.Path == folderPath,includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.Files }).FirstOrDefault();

                string path = $"{folder.Path}\\{folder.Name}";
              
                if (fileCreationModels.Any())
                {
                    BaseResponse<List<File>> filesResponse = await GenerateFiles(folder, fileCreationModels, folderPath);
                    if (!filesResponse.IsSuccess)
                    {
                        return new BaseResponse(false, filesResponse.Message);
                    }
                    folder.Files = new List<File>(filesResponse.DataObject);
                    folder.CreationUserId = _userAccessor.GetCurrentUserId();
                    folder.IsActive = true;
                    folder.CreationDate = DateTime.UtcNow;

                    string[] paths = path.Split("\\");
                    List<Folder> folders = new List<Folder>();
                    List<Folder> newFolders = new List<Folder>();
                    if (existfolder is null)
                    {
                        BaseResponse<List<Folder>> responseFolder = await GenerateFolders(folders, newFolders, folder, paths, pathFromFolderConfiguration);
                        if (!responseFolder.IsSuccess)
                        {
                            return new BaseResponse(false, responseFolder.Message);
                        }
                        _unitOfWork.GetRepository<Folder>().AddRange(responseFolder.DataObject);
                        await _unitOfWork.CommitChanges();
                        return new BaseResponse(true);
                    }
                    else
                    {
                        existfolder.Files.AddRange(filesResponse.DataObject);
                        _unitOfWork.GetRepository<Folder>().Update(existfolder);                      

                        await _unitOfWork.CommitChanges();
                    
                        return new BaseResponse(true);
                    }
                }
                else
                {
                    folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                    BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.Path);
                    if (!folders.IsSuccess)
                    {
                        return new BaseResponse(false, folders.Message);
                    }
                    if (folders.DataObject.Any())
                    {
                        folders.DataObject.ForEach(oldFolder =>
                        {
                            BaseResponse responseDelete = DeleteFolder(oldFolder.FolderId).Result;
                        });
                    }
                    return new BaseResponse(true);
                }
            }
            catch (Exception ex)
            {
                folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                BaseResponse<List<Folder>> folders = await GetFolderByPath(folder.Path);
                if (!folders.IsSuccess)
                {
                    return new BaseResponse(false, folders.Message);
                }
                if (folders.DataObject.Any())
                {
                    folders.DataObject.ForEach(oldFolder =>
                    {
                        BaseResponse responseDelete = DeleteFolder(oldFolder.FolderId).Result;
                    });
                }
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateFolderAndFilesWithoutDelete), _userAccessor.GetCurrentUserId(), folder, fileCreationModels);
                return new BaseResponse(true, ex.Message);
            }
        }

        public async Task<BaseResponse<FileCreationModel>> GetImagesByFileName(string fileName)
        {
            try
            {
                File file = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(x => x.Name.Contains(fileName),includedEntities:new List<System.Linq.Expressions.Expression<Func<File, object>>> { z => z.FileType}).FirstOrDefault();
                FileCreationModel fileCreationModel = new FileCreationModel();
                if (file is not null)
                {
                    fileCreationModel.FileType = file.FileType.Extension;
                    fileCreationModel.FileName = file.Name;
                    byte[] imageByte = System.IO.File.ReadAllBytes($"{file.Path}\\{fileCreationModel.FileName}.{fileCreationModel.FileType}");
                    fileCreationModel.FileContent = ConvertByteToString64(imageByte, (file.FileTypeId == (int)FileTypes.SVG ? "svg+xml" : fileCreationModel.FileType));
                }
                return new BaseResponse<FileCreationModel>(true, string.Empty, fileCreationModel);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetImagesByFileName), _userAccessor.GetCurrentUserId(), fileName);
                return new BaseResponse<FileCreationModel>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetImagesByFileNameList(List<string> filesName)
        {
            try
            {
                List<FileCreationModel> files = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(p => filesName.Contains(p.Name)).Select(s => new FileCreationModel
                {
                    FileName = s.Name,
                    FileTypeId = s.FileTypeId,
                }).ToList();
                return new BaseResponse<List<FileCreationModel>>(true, string.Empty, files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetImagesByFileNameList), _userAccessor.GetCurrentUserId(), filesName);
                return new BaseResponse<List<FileCreationModel>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<File>>> GenerateFiles(Folder folder, List<FileCreationModel> fileCreationModels, string folderPath)
        {
            try
            {
                List<File> files = new List<File>();
                foreach (FileCreationModel fileCreationModel in fileCreationModels)
                {
                    BaseResponse createFolderAndFilesPhysically = await CreateFolderAndFilesPhysically(folder, fileCreationModel);
                    if (createFolderAndFilesPhysically.IsSuccess)
                    {
                        BaseResponse<FileType> fileType = await _fileTypeService.GetFileTypeByFileExtension(fileCreationModel.FileType);
                        if (!fileType.IsSuccess)
                        {
                            return new BaseResponse<List<File>>(false,fileType.Message,fileType.ErrorCode);
                        }
                        byte[] fileContentBytes = ConvertString64ToByte(fileCreationModel.FileContent);

                        File file = new File()
                        {
                            IsActive = true,
                            CreationDate = DateTime.UtcNow,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            Name = fileCreationModel.FileName,
                            FileTypeId = fileType.DataObject.FileTypeId,
                            Path = folderPath,
                            Size = fileContentBytes.Length,
                        };
                        
                        if (fileCreationModel.FileContent.Length < 1024)
                        {
                            file.SizeType = "byte";
                        }
                        if (fileCreationModel.FileContent.Length == 1024)
                        {
                            file.SizeType = "KB";
                        }
                        if (fileCreationModel.FileContent.Length > (1024 * 1024))
                        {
                            file.SizeType = "MB";
                        }
                        if (fileCreationModel.FileContent.Length > (1024 * 1024 * 1024))
                        {
                            file.SizeType = "GB";
                        }
                        files.Add(file);
                    }
                    else
                    {
                        return new BaseResponse<List<File>>(false, createFolderAndFilesPhysically.Message);
                    }
                }
                return new BaseResponse<List<File>>(true, string.Empty, files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GenerateFiles), _userAccessor.GetCurrentUserId(), folder);
                return new BaseResponse<List<File>>(true, ex.Message);
            }
        }

        public async Task<BaseResponse<List<Folder>>> GenerateFolders(List<Folder> folders, List<Folder> newFolders, Folder folder, string[] paths, string pathFromFolderConfiguration)
        {
            try
            {             
                if (paths.Any())
                {
                    int i = 0;
                    foreach (string newPath in paths)
                    {
                        if (i <= paths.Length - 2)
                        {
                            Folder newFolder = new Folder();
                            newFolder.Path = (i == 0) ? pathFromFolderConfiguration + newPath : folders[i - 1].Path + "\\" + newPath;
                            newFolder.CreationUserId = _userAccessor.GetCurrentUserId();
                            newFolder.IsActive = true;
                            newFolder.Name = newPath;
                            newFolder.CreationDate = DateTime.Now;
                            newFolder.ParentFolderId = (i > 0) ? folders[i - 1].FolderId : newFolder.ParentFolderId;
                            newFolder.ParentFolder = (i > 0) ? folders[i - 1] : null;
                            
                            Folder existFolder = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Path.Contains(newFolder.Path)).FirstOrDefault();
                            if (existFolder == null)
                            {
                                newFolders.Add(newFolder);
                                folders.Add(newFolder);
                            }
                            else
                            {
                                folders.Add(existFolder);
                            }
                        }
                        i++;
                    }
                }
                folder.ParentFolder = folders.LastOrDefault();
                folder.ParentFolderId = folders.LastOrDefault().FolderId;
                folder.Path = $"{pathFromFolderConfiguration}{folder.Path}\\{folder.Name}";
                newFolders.Add(folder);

                return new BaseResponse<List<Folder>>(true, string.Empty, newFolders);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GenerateFolders), _userAccessor.GetCurrentUserId(), folders);
                return new BaseResponse<List<Folder>>(true, ex.Message);
            }
        }

        public async Task<BaseResponse> DeleteFilesByFolderId(int folderId)
        {
            try
            {
               
                IQueryable<File> foldersQuery = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(z => z.FolderId == folderId);
                List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(foldersQuery);              

                _unitOfWork.GetRepository<File>().RemoveRange(files);
                return new BaseResponse(true);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(DeleteFilesByFolderId), _userAccessor.GetCurrentUserId(), folderId);
                return new BaseResponse(false,ex.Message);
            }
        }

        public async Task<BaseResponse> UpdatFolderName(string path, string oldName, string newName)
        {
            try
            {
                BaseResponse<FolderConfiguration> folderConfigurationPath = await _folderConfigurationService.GetFolderConfiguration();
                if (!folderConfigurationPath.IsSuccess)
                {
                    return folderConfigurationPath;
                }
                path = $"{folderConfigurationPath.DataObject.RootFolderPath}{path}";
                BaseResponse<List<Folder>> foldersResponse = await GetFolders($"{path}\\{oldName}");
                if (!foldersResponse.IsSuccess)
                {
                    return new BaseResponse(false, foldersResponse.Message);
                }
                if (foldersResponse.DataObject.Any())
                {
                    foldersResponse.DataObject.ForEach(p =>
                    {
                        p.Path = $"{path}\\{newName}";
                        p.Name = $"{newName}";
                    });
                }
                else
                {
                    return new BaseResponse(true, string.Empty);
                }

                BaseResponse<List<File>> filesResponse = await GetFiles($"{path}\\{oldName}");
                if (!filesResponse.IsSuccess)
                {
                    return new BaseResponse(false, filesResponse.Message);
                }
                if (filesResponse.DataObject.Any())
                {
                    filesResponse.DataObject.ForEach(p => p.Path = $"{path}\\{newName}");
                }

                System.IO.Directory.Move($"{path}\\{oldName}", $"{path}\\{newName}");               
                await _unitOfWork.CommitChanges();
              
                return new BaseResponse(true, string.Empty);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(UpdatFolderName), _userAccessor.GetCurrentUserId(), oldName);
                return new BaseResponse(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<Folder>>> GetFolders(string path)
        {
            try
            {
                IQueryable<Folder> folderQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(p => p.Path.Contains(path));
                
                List<Folder> folders = await _unitOfWork.GetRepository<Folder>().ToListAsync(folderQuery);
            
                return new BaseResponse<List<Folder>>(true, string.Empty, folders);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFolders), _userAccessor.GetCurrentUserId(), path);
                return new BaseResponse<List<Folder>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<File>>> GetFiles(string path)
        {
            try
            {
                IQueryable<File> fileQuery = _unitOfWork.GetRepository<File>().GetByExpressionAsNoTracking(p => p.Path.Contains(path));
            
                List<File> files = await _unitOfWork.GetRepository<File>().ToListAsync(fileQuery);               

                return new BaseResponse<List<File>>(true, string.Empty, files);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFiles), _userAccessor.GetCurrentUserId(), path);
                return new BaseResponse<List<File>>(false, ex.Message);
            }
        }

        public async Task<BaseResponse<List<FileCreationModel>>> GetFilesByRootFolderPath(string rootFolderPath)
        {
            try
            {
                IQueryable<Folder> foldersQuery = _unitOfWork.GetRepository<Folder>().GetByExpressionAsNoTracking(x => x.Path.Contains(rootFolderPath) && x.ChildFolders.Any(p => p.Files.Any()), includedEntities: new List<System.Linq.Expressions.Expression<Func<Folder, object>>>() { z => z.ChildFolders });          

                Folder folder = await _unitOfWork.GetRepository<Folder>().FirstOrDefaultAsync(foldersQuery);          

                List<FileCreationModel> fileCreationModels = new List<FileCreationModel>();
                if (folder is not null && folder.ChildFolders.Any())
                {
                    foreach (Folder chiled in folder.ChildFolders)
                    {
                        BaseResponse<List<FileCreationModel>> fileCreationModelResponse = await GetFilesByFolderPath($"{rootFolderPath}\\{chiled.Name}");
                        if (!fileCreationModelResponse.IsSuccess)
                        {
                            return fileCreationModelResponse;
                        }
                        fileCreationModels.AddRange(fileCreationModelResponse.DataObject);
                    }
                }
                return new BaseResponse<List<FileCreationModel>>(true, string.Empty, fileCreationModels);
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(GetFilesByRootFolderPath), _userAccessor.GetCurrentUserId(), rootFolderPath);
                return new BaseResponse<List<FileCreationModel>>(false, ex.Message);
            }
        }

        private async Task<string> CreateDirectory(List<Folder> folders)
        {
            try
            {
                foreach (Folder folder in folders)
                {
                    string path = folder.Path;
                    if (Directory.Exists(path))
                    {
                        return "The folder is exist";
                    }
                    else
                    {
                        DirectoryInfo di = Directory.CreateDirectory(path);
                        return "The directory was ceearted successfully.";
                    }
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                await _exceptionService.LogErrorAsync(ex, nameof(CreateDirectory), _userAccessor.GetCurrentUserId(), folders);
                return ex.Message;
            }
        }

        private byte[] ConvertString64ToByte(string profileImageBase64String)
        {
            try
            {
                if (!string.IsNullOrEmpty(profileImageBase64String))
                {
                    if (profileImageBase64String.Contains(","))
                    {
                        profileImageBase64String = profileImageBase64String.Substring(profileImageBase64String.IndexOf(",") + 1);
                    }
                    return Convert.FromBase64String(profileImageBase64String);
                }
                return null;
            }
            catch (Exception ex)
            {
                while (ex.InnerException is not null)
                {
                    ex = ex.InnerException;
                }
                _exceptionService.LogErrorAsync(ex, nameof(ConvertString64ToByte), _userAccessor.GetCurrentUserId(), profileImageBase64String);
                return null;
            }
        }

        private string ConvertByteToString64(byte[] profileImage, string extension)
        {
            if (profileImage.Count() >= 0)
            {
                if (extension == "pdf")
                {
                    return $"data:application/{extension};base64,{Convert.ToBase64String(profileImage)}";
                }
                return $"data:image/{extension};base64,{Convert.ToBase64String(profileImage)}";
            }
            return null;
        }

        private async Task<string> getExtensionAsync(int fileTypeId)
        {
            BaseResponse<FileType> fileType = await _fileTypeService.GetFileTypeById(fileTypeId);
            if (!fileType.IsSuccess)
            {
                await _exceptionService.LogErrorAsync(fileType.Message, nameof(getExtensionAsync), _userAccessor.GetCurrentUserId(), fileTypeId);
                return string.Empty;
            }
            return fileType.DataObject.Extension;
        }

        private async Task<List<FileTypeModel>> getExtensionsAsync(List<int> fileTypeIds)
        {
            BaseResponse<List<FileType>> fileType = await _fileTypeService.GetFileTypeByIds(fileTypeIds);
            if (!fileType.IsSuccess)
            {
                await _exceptionService.LogErrorAsync(fileType.Message, nameof(getExtensionAsync), _userAccessor.GetCurrentUserId(), fileTypeIds);
                throw new Exception(fileType.Message);
            }
            return fileType.DataObject.Select(s => new FileTypeModel()
            {
                FileTypeId =s.FileTypeId,
                Extension =s.Extension,
            }).ToList();
        }
        #endregion

    }
}
