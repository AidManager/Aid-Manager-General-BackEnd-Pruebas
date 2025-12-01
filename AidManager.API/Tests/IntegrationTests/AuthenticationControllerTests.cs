using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;

namespace AidManager.API.Tests.IntegrationTests
{
    [TestFixture]
    public class AuthenticationControllerTests
    {
        private HttpClient _client;

       
        // - Producción (Railway): https://aid-manager-general-backend-production-c63f.up.railway.app
        // - Local:                https://localhost:5001  
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
        public async Task SignIn_WithInvalidCredentials_ReturnsUnauthorized()
        {
            // Arrange
            var signInResource = new { Username = "nonexistentuser", Password = "wrongpassword" };
            var json = JsonSerializer.Serialize(signInResource);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/authentication/sign-in", content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public async Task SignIn_WithValidCredentials_ReturnsOkAndToken()
        {
           

            // Arrange
            var signInResource = new { Username = "testuser", Password = "password123" };
            var json = JsonSerializer.Serialize(signInResource);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/v1/authentication/sign-in", content);

            // Assert
            response.EnsureSuccessStatusCode(); // 2xx
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var responseString = await response.Content.ReadAsStringAsync();
            var authResource = JsonSerializer.Deserialize<JsonElement>(responseString);

            Assert.That(authResource.GetProperty("token").GetString(), Is.Not.Null.And.Not.Empty);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client?.Dispose();
        }
    }
}
