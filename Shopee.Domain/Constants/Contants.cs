using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Constants;
public static class Constants
{
    public const int PAGE_SIZE_PRODUCT = 20;
    public const string CART_CACHE_KEY_PREFIX = "Cart_";
    public static readonly string[] ALLOWED_FILE_EXTENSIONS = { ".jpg", ".jpeg", ".png" };
}