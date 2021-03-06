﻿using System.Net.Http;

static class DeriveCacheStatus
{
    public static CacheStatus CacheStatus(this HttpResponseMessage response, bool staleIfError)
    {
        if (response.IsNotModified())
        {
            return global::CacheStatus.Revalidate;
        }

        if (response.IsNoStore())
        {
            return global::CacheStatus.Revalidate;
        }

        if (response.IsNoCache())
        {
            return global::CacheStatus.NoCache;
        }

        if (!response.IsSuccessStatusCode)
        {
            if (staleIfError)
            {
                return global::CacheStatus.UseStaleDueToError;
            }

            response.EnsureSuccess();
        }

        return global::CacheStatus.Stored;
    }
}