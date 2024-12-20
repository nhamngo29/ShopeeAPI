﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shopee.Domain.Common;
public class Pagination<T>
{
    public int Page { get; private set; }
    public int TotalPages { get; private set; }
    public int PageSize { get; private set; }
    public int TotalCount { get; private set; }
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
    public List<T>? Items { get; private set; }

    public Pagination(List<T> items, int count, int pageIndex, int pageSize)
    {
        Items = items;
        Page = pageIndex;
        TotalPages = count;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
    }

    public Pagination()
    {

    }
    public static async Task<Pagination<T>> ToPagedList(IQueryable<T> source, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        pageSize = pageSize <= 0 ? 10 : pageSize;

        var count = source.Count();
        var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
        return new Pagination<T>(items, count, pageIndex, pageSize);
    }
}