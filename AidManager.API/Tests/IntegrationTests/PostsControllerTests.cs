using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AidManager.API.Collaborate.Tests.IntegrationTests
{
    [TestFixture]
    public class PostsControllerTests
    {
        private HttpClient _client;

        // Producción (Railway):
        //   "https://aid-manager-general-backend-production-c63f.up.railway.app"
        // Local:
        //   "https://localhost:5001" (o el puerto que uses)
        private const string BaseUrl = "https://aid-manager-general-backend-production-c63f.up.railway.app";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _client = new HttpClient
            {
                BaseAddress = new System.Uri(BaseUrl)
            };
        }

        [Test]
        public async Task CreateNewPost_WithNoToken_ReturnsUnauthorized()
        {
            // Nos aseguramos de que NO haya token en el header
            _client.DefaultRequestHeaders.Authorization = null;

            // Arrange
            var createPostResource = new
            {
                Title = "Test Post",
                Content = "This is a test post.",
                AuthorId = 1,
                CompanyId = 1,
                ProjectId = 1,
                Status = "PUBLISHED"
            };

            var json = JsonSerializer.Serialize(createPostResource);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/posts", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task CreateNewPost_WithValidToken_ReturnsOk()
        {
           

            // 1. Authenticate and get token
            var signInResource = new { Username = "testuser", Password = "password123" };
            var authJson = JsonSerializer.Serialize(signInResource);
            var authContent = new StringContent(authJson, Encoding.UTF8, "application/json");

            var authResponse = await _client.PostAsync("/api/v1/authentication/sign-in", authContent);
            authResponse.EnsureSuccessStatusCode();

            var responseString = await authResponse.Content.ReadAsStringAsync();
            var authResource = JsonSerializer.Deserialize<JsonElement>(responseString);
            var token = authResource.GetProperty("token").GetString();

            // 2. Prepare request for the protected endpoint
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var createPostResource = new
            {
                Title = "Test Post",
                Content = "This is a test post.",
                AuthorId = 1,
                CompanyId = 1,
                ProjectId = 1,
                Status = "PUBLISHED"
            };

            var json = JsonSerializer.Serialize(createPostResource);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/posts", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
        }
    }
}
