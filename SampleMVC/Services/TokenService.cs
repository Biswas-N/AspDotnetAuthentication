using System;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace SampleMVC.Services
{
    public interface ITokenService
    {
        Task<TokenResponse> GetToken(string scope);
    }
    
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IOptions<IdentityServerSettings> _settings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;
        
        public TokenService(ILogger<TokenService> logger, IOptions<IdentityServerSettings> settings)
        {
            _logger = logger;
            _settings = settings;

            using var client = new HttpClient();
            _discoveryDocument = client.GetDiscoveryDocumentAsync(_settings.Value.DiscoveryUrl).Result;
            if (_discoveryDocument.IsError)
            {
                const string message = "Unable to fetch discovery document";
                _logger.LogError($"{message}. Error: {_discoveryDocument.Error}");
                throw new Exception(message, _discoveryDocument.Exception);
            }
        }

        public async Task<TokenResponse> GetToken(string scope)
        {
            using var client = new HttpClient();
            var tokenResp = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = _discoveryDocument.TokenEndpoint,
                
                ClientId = _settings.Value.ClientId,
                ClientSecret = _settings.Value.ClientSecret,
                Scope = scope
            });

            if (tokenResp.IsError)
            {
                const string message = "Unable to fetch token";
                _logger.LogError($"{message}. Error: {tokenResp.Error}");
                throw new Exception(message, tokenResp.Exception);
            }

            return tokenResp;
        }
    }
}