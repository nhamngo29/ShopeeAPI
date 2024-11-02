using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Application.Common.Interfaces;
public interface ICurrentUser
{
    public string GetCurrentUserId();
}
