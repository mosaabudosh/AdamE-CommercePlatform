using Adam.Logging.Interfaces;
using ArabazonPlatform.Application.Caching;
using ArabazonPlatform.Application.Model.Authorisation;
using ArabazonPlatform.Application.Services.DMSManagement.FolderManagement.Commands;
using ArabazonPlatform.Domain.Domains.Authentication;
using ArabazonPlatform.Domain.Domains.Authorisation;
using ArabazonPlatform.Domain.Enum;
using ArabazonPlatform.Domain.IHttpServices;
using ArabazonPlatform.Domain.RepositoryInterfaces;
using ArabazonPlatform.Generic.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArabazonPlatform.Domain.Enum.Enums;

namespace ArabazonPlatform.Application.Services.AuthenticationManagement.CustomerCardInfoManagement.Commands
{
    public class CustomerCardInfoCreateCommands
    {
        public class Command : IRequest<BaseResponse>
        {
            public CustomerCardInfoModel CustomerCardInfoModel { get; set; }
            public Command(CustomerCardInfoModel customerCardInfoModel)
            {
                CustomerCardInfoModel = customerCardInfoModel;
            }

            public class Handler(IUnitOfWork _unitOfWork, IUserAccessor _userAccessor, IExceptionService _exceptionService) : IRequestHandler<Command, BaseResponse>
            {
                public async Task<BaseResponse> Handle(Command request, CancellationToken cancellationToken)
                {
                    try
                    {
                        CustomerCardInfo customerCardInfo = new CustomerCardInfo()
                        {
                            CardNumber = request.CustomerCardInfoModel.CardNumber,
                            CustomerId = request.CustomerCardInfoModel.CustomerId,
                            Expiry = request.CustomerCardInfoModel.Expiry,
                            IsRememberCard = request.CustomerCardInfoModel.IsRememberCard,
                            CreationUserId = _userAccessor.GetCurrentUserId(),
                            CreationDate = DateTime.UtcNow,
                            IsActive = true
                        };

                        _unitOfWork.GetRepository<CustomerCardInfo>().Insert(customerCardInfo);
                        await _unitOfWork.CommitChanges();

                        return new BaseResponse(true, string.Empty);
                    }
                    catch (Exception ex)
                    {
                        while (ex.InnerException is not null)
                        {
                            ex = ex.InnerException;
                        }
                        await _exceptionService.LogErrorAsync(ex, "CustomerCardInfoCreateCommands", _userAccessor.GetCurrentUserId(), request);
                        return new BaseResponse<CustomerRegisterModel>(false, ex.Message);
                    }
                }

            }
        }
    }
}
