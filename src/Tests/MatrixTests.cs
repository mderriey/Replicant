using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Replicant;
using VerifyTests;
using VerifyXunit;
using Xunit;

//TODO: can LastModified or Expires for a NotModified?
[UsesVerify]
public class MatrixTests
{
    static VerifySettings sharedSettings;
    static DateTimeOffset now = new(2000, 1, 1, 0, 0, 0, TimeSpan.Zero);
    static DateTimeOffset inPast = now.AddDays(-1);
    static DateTimeOffset inFuture = now.AddDays(1);
    static DateTimeOffset?[] expiries = {inFuture};
    static DateTimeOffset?[] mods = {null};

    static MatrixTests()
    {
        sharedSettings = new VerifySettings();
        sharedSettings.UseDirectory("../MatrixResults");
    }

    public static IEnumerable<object?[]> DataForIntegration()
    {
        foreach (var modDate in mods)
        foreach (var expiry in expiries)
        foreach (var etag in etagStrings)
        foreach (var response in Responses())
        {
            yield return new object?[]
            {
                new StoredData(expiry, modDate, etag),
                response
            };
        }
    }

    [Theory]
    [MemberData(nameof(DataForIntegration))]
    public async Task Integration(
        StoredData data,
        HttpResponseMessageEx response)
    {
        var fileName = $"Int_{response}_useStale={true}_exp={data.Expiry:yyyyMMdd}_mod={data.Modified:yyyyMMdd}_tag={data.Etag?.Replace('/','_').Replace('"','_')}";
        var settings = new VerifySettings(sharedSettings);
        settings.UseFileName(fileName);

        var directory = Path.Combine(Path.GetTempPath(),"HttpClientIntegrationTests"+Namer.Runtime);
        try
        {
            await using var cache = new HttpCache(directory, new MockHttpClient(response));
            cache.Purge();
            await cache.AddItemAsync("uri", "content", data.Expiry, data.Modified, data.Etag);
            var result = await cache.DownloadAsync("uri", true);
            await Verifier.Verify(result, settings);
        }
        catch (HttpRequestException exception)
        {
            await Verifier.Verify(exception, settings);
        }
        finally
        {
            Directory.Delete(directory, true);
        }
    }

    static IEnumerable<HttpResponseMessageEx> Responses()
    {
        foreach (var webExpiry in expiries)
        {
            foreach (var webMod in mods)
            foreach (var webEtag in etags)
            foreach (var cacheControl in cacheControls)
            {
                HttpResponseMessageEx response = new(HttpStatusCode.OK)
                {
                    Content = new StringContent("a")
                };
                response.Content.Headers.LastModified = webMod;
                response.Content.Headers.Expires = webExpiry;
                if (!webEtag.IsEmpty)
                {
                    response.Headers.TryAddWithoutValidation("ETag", webEtag.ForWeb);
                }

                response.Headers.CacheControl = cacheControl;
                yield return response;
            }
        }
    }

    static Etag[] etags = new Etag[]
    {
        Etag.Empty,
    };

    static string?[] etagStrings = new string?[]
    {
        null
    };

    static CacheControlHeaderValue[] cacheControls = new CacheControlHeaderValue[]
    {
        new()
        {
            NoStore = true,
        }
    };

}