﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Minder.Services.Extensions {

    public static class LinqExtension {

        public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source,
            [DoesNotReturnIf(true)] bool when,
            Expression<Func<TSource, bool>> predicateTrue,
            Expression<Func<TSource, bool>>? predicateFalse = null) {
            if (when) {
                return source.Where(predicateTrue);
            }
            if (predicateFalse != null) {
                return source.Where(predicateFalse);
            }
            return source;
        }

        public static IQueryable<TSource> WhereFunc<TSource>(this IQueryable<TSource> source,
            [DoesNotReturnIf(true)] bool when,
            Func<IQueryable<TSource>, IQueryable<TSource>> funcTrue,
            Func<IQueryable<TSource>, IQueryable<TSource>>? funcFalse = null) {
            if (when) {
                return funcTrue.Invoke(source);
            }
            if (funcFalse != null) {
                return funcFalse.Invoke(source);
            }
            return source;
        }

        public static async Task<int> CountIf<TSource, TResult>(this IQueryable<TSource> source,
            [DoesNotReturnIf(true)] bool when,
            Expression<Func<TSource, TResult>>? selector = null) {
            if (when) {
                if (selector != null)
                    return await source.Select(selector).CountAsync();
                return await source.CountAsync();
            }
            return 0;
        }
    }
}