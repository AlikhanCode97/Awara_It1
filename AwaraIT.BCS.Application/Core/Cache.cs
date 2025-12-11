using AwaraIT.BCS.Infrastructure.Repositories.Crm;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace AwaraIT.BCS.Application.Core
{
    public class Cache
    {
        private const string GetEnvironmentVariablesCacheKey = "{417AFCFD-B316-48AE-BE3C-5251258346F1}_GetEnvironmentVariables";

        private const int CacheExpirationSec = 1800;

        private readonly IOrganizationService _service;
        private readonly ObjectCache _cache = MemoryCache.Default;
        private readonly EnvironmentVariableDefinitionRepository _environmentVariableDefinitionRepository;

        public Cache(IOrganizationService service)
        {
            _service = service;
            _environmentVariableDefinitionRepository = new EnvironmentVariableDefinitionRepository(service);
        }

        public Dictionary<string, string> GetEnvironmentVariables()
        {
            var result = GetOrAddValue(GetEnvironmentVariablesCacheKey, () =>
            {
                var variables = _environmentVariableDefinitionRepository.GetAllWithValue();
                return variables;
            });

            return result;
        }

        private T GetOrAddValue<T>(string key, int expirationSec, Func<T> getValue)
        {
            T value;
            if (_cache.Contains(key))
            {
                value = (T)_cache.Get(key);
            }
            else
            {
                value = getValue();
                if (value != null)
                    _cache.Add(key, value, DateTime.Now.AddSeconds(expirationSec));
            }

            return value;
        }

        private T GetOrAddValue<T>(string key, Func<T> getValue)
        {
            return GetOrAddValue(key, CacheExpirationSec, getValue);
        }
    }
}
