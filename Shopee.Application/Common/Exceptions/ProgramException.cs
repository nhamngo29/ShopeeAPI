using Shopee.Domain.Constants;
using Shopee.Domain.Enums;

namespace Shopee.Application.Common.Exceptions;

public static class ProgramException
{
    public static UserFriendlyException AppsettingNotSetException()
        => new(ErrorCode.Internal, ErrorMessage.AppConfigurationMessage, ErrorMessage.Internal);
}