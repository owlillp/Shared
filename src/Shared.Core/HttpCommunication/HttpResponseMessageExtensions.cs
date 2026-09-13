using System.Net.Http.Json;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Shared.SharedKernel;
using Shared.SharedKernel.Errors;
using Shared.SharedKernel.Serializations;

namespace Shared.Core.HttpCommunication;

public static class HttpResponseMessageExtensions
{
    extension(HttpResponseMessage httpResponse)
    {
        public async Task<Result<TResponse, Error>> HandleResponseAsync<TResponse>(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await httpResponse.Content.ReadFromJsonAsync<Envelope<TResponse>>(JsonOptionsProvider.Options, cancellationToken).ConfigureAwait(false);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return response?.Error ?? Error.Failure("http.error", "Error while reading http response");
                }

                if (response == null)
                {
                    return Error.Failure("http.error", "Error while reading http response");
                }

                if (response is { IsError: true, Error: not null })
                {
                    return response.Error;
                }

                if (response.Result == null!)
                {
                    return Error.Failure("http.error", "Error while reading http response");
                }

                return response.Result;
            }
            catch(Exception ex)
            {
                return Error.Failure("http.error", $"Unexpected error while reading http response: {ex.Message}");
            }
        }

        public async Task<UnitResult<Error>> HandleResponseAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await httpResponse.Content.ReadFromJsonAsync<Envelope>(JsonOptionsProvider.Options, cancellationToken).ConfigureAwait(false);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    return response?.Error ?? Error.Failure("http.error", "Error while reading http response");
                }

                if (response == null)
                {
                    return Error.Failure("http.error", "Error while reading http response");
                }

                if (response is { IsError: true, Error: not null })
                {
                    return response.Error;
                }

                return UnitResult.Success<Error>();
            }
            catch(Exception ex)
            {
                return Error.Failure("http.error", $"Unexpected error while reading http response: {JsonSerializer.Serialize(ex)}");
            }
        }
    }
}