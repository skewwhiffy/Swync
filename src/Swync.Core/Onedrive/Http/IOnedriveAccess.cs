using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Swync.Core.Onedrive.Http
{
    public interface IOnedriveAccess
    {
        Task<T> GetAsync<T>(string relativeUrl, CancellationToken ct);
        Task<Stream> GetContentStreamAsync(string relativeUrl, CancellationToken ct);
        Task<Stream> GetContentStreamAsync(Uri absoluteUri, Tuple<string, string> header, CancellationToken ct);
        Task<TResponsePayload> PostAsync<TPayload, TResponsePayload>(string relativeUrl, TPayload payload, CancellationToken ct);
        Task<T> PutAsync<T>(string relativeUrl, Byte[] bytes, CancellationToken ct);
        Task<T> PutAsync<T>(Uri absoluteUri, Byte[] bytes, CancellationToken ct, params Tuple<string, string>[] header);
        Task DeleteAsync(string relativeUrl, CancellationToken ct);
    }
}