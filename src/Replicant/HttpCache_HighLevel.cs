using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Replicant
{
    public partial class HttpCache
    {
        public async Task<string> StringAsync(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            using var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            return await result.AsStringAsync(token);
        }

        public async Task<byte[]> BytesAsync(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            using var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            return await result.AsBytesAsync(token);
        }

        public async Task<Stream> StreamAsync(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            return await result.AsStreamAsync(token);
        }

        public async Task<HttpResponseMessage> ResponseAsync(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            return result.AsResponseMessage();
        }

        public async Task ToStreamAsync(
            string uri,
            Stream stream,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            using var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            await result.ToStreamAsync(stream, token);
        }

        public async Task ToFileAsync(
            string uri,
            string path,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            using var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            await result.ToFileAsync(path, token);
        }

        public async IAsyncEnumerable<string> LinesAsync(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            [EnumeratorCancellation] CancellationToken token = default)
        {
            using var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            using var stream = result.AsStream(token);
            using var reader = new StreamReader(stream);
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                yield return line;
            }
        }


        public async Task<HttpResponseMessage> Response(
            string uri,
            bool staleIfError = false,
            Action<HttpRequestMessage>? modifyRequest = null,
            CancellationToken token = default)
        {
            var result = await DownloadAsync(uri, staleIfError, modifyRequest, token);
            return result.AsResponseMessage();
        }

    }
}