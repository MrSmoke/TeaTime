namespace TeaTime.Common.Tests.Cache
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Common.Cache;
    using Microsoft.Extensions.Caching.Distributed;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Options;
    using Xunit;

    public class CacheIntegrationTests
    {
        [Fact]
        public async Task SetAsync_GetAsync_Bool_Returns()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await cache.SetAsync(nameof(SetAsync_GetAsync_Bool_Returns), true, new CacheEntryOptions(),
                CancellationToken.None);

            var value = await cache.GetAsync<bool>(nameof(SetAsync_GetAsync_Bool_Returns));

            Assert.NotNull(value);
            Assert.True(value.Value);
        }

        [Fact]
        public async Task GetAsync_ItemNotExist_ReturnsNull()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            var value = await cache.GetAsync<bool>(nameof(GetAsync_ItemNotExist_ReturnsNull));

            Assert.Null(value);
        }

        [Fact]
        public async Task SetAsync_NullValue_ThrowsArgumentNullException()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await Assert.ThrowsAsync<ArgumentNullException>(() => cache.SetAsync(
                nameof(SetAsync_NullValue_ThrowsArgumentNullException),
                null,
                new CacheEntryOptions(),
                CancellationToken.None));
        }

        [Fact]
        public async Task SetAsync_NullKey_ThrowsArgumentNullException()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await Assert.ThrowsAsync<ArgumentNullException>(() => cache.SetAsync(
                null,
                "",
                new CacheEntryOptions(),
                CancellationToken.None));
        }

        [Fact]
        public async Task GetAsync_NullKey_ThrowsArgumentNullException()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await Assert.ThrowsAsync<ArgumentNullException>(() => cache.GetAsync<string>(null));
        }

        [Fact]
        public async Task SetAsync_NoExpiration_DoesNotThrow()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await cache.SetAsync(nameof(SetAsync_NoExpiration_DoesNotThrow), "hello", new CacheEntryOptions
            {
                Sliding = true,
                Expiration = null
            }, CancellationToken.None);
        }

        [Fact]
        public async Task SetAsync_NonSliding_Expires()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await cache.SetAsync(nameof(SetAsync_NonSliding_Expires), "hello", new CacheEntryOptions
            {
                Sliding = false,
                Expiration = TimeSpan.FromSeconds(1)
            }, CancellationToken.None);

            Assert.NotNull(await cache.GetAsync<string>(nameof(SetAsync_NonSliding_Expires)));

            await Task.Delay(TimeSpan.FromSeconds(1.5));

            Assert.Null(await cache.GetAsync<string>(nameof(SetAsync_NonSliding_Expires)));
        }

        [Fact]
        public async Task SetAsync_Sliding()
        {
            var cache = new Cache(GetDistributedCache(), new SystemTextJsonCacheSerializer());

            await cache.SetAsync(nameof(SetAsync_Sliding), "hello", new CacheEntryOptions
            {
                Sliding = true,
                Expiration = TimeSpan.FromSeconds(0.5)
            }, CancellationToken.None);

            // ensure in cache
            Assert.NotNull(await cache.GetAsync<string>(nameof(SetAsync_Sliding)));

            // wait 0.5 sec
            await Task.Delay(TimeSpan.FromSeconds(0.3));

            // check again, this should slide it
            Assert.NotNull(await cache.GetAsync<string>(nameof(SetAsync_Sliding)));

            // wait again
            await Task.Delay(TimeSpan.FromSeconds(0.3));

            // check again, this should slide it. If sliding wasn't working, it should be expired by now
            Assert.NotNull(await cache.GetAsync<string>(nameof(SetAsync_Sliding)));

            // wait for it to expire
            await Task.Delay(TimeSpan.FromSeconds(0.6));

            // should not longer exist
            Assert.Null(await cache.GetAsync<string>(nameof(SetAsync_Sliding)));
        }

        private static IDistributedCache GetDistributedCache()
        {
            var options = new MemoryDistributedCacheOptions();

            return new MemoryDistributedCache(new OptionsWrapper<MemoryDistributedCacheOptions>(options));
        }
    }
}
