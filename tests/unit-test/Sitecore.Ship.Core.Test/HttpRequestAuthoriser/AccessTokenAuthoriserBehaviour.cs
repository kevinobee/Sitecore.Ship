using System.Collections.Specialized;
using Xunit;

namespace Sitecore.Ship.Core.Test.HttpRequestAuthoriser
{
    public class AccessTokenAuthoriserBehaviour : HttpRequestAuthoriserTest
    {
        public AccessTokenAuthoriserBehaviour()
        {
            PackageInstallationSettings.AllowRemoteAccess = true;
            PackageInstallationSettings.AccessToken = "MyToken";
        }
        
        [Fact]
        public void Should_return_true_when_configuration_settings_token_is_empty()
        {
            // Arrange
            PackageInstallationSettings.AccessToken = string.Empty;

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_headers_are_not_defined()
        {
            // Arrange

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_authorization_header_is_not_defined()
        {
            // Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Accept-Language", "en-US"}
            });

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Theory]
        [InlineData("bearer MyToken")]
        [InlineData("Bearer MyToken")]
        [InlineData("Bearer    MyToken")]
        [InlineData("Bearer MyToken  ")]
        public void Should_return_true_when_tokens_match_each_other(string authorizationHeader)
        {
            // Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", authorizationHeader}
            });

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }

        [Fact]
        public void Should_return_false_when_different_authentication_sceme_used()
        {
            // Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Basic MyToken"}
            });

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Theory]
        //tokens are case sensitive
        [InlineData("mytoken")]
        [InlineData("wrong-values")]
        public void Should_return_false_when_wrong_token_provided(string token)
        {
            // Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Bearer " + token}
            });

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.False(isAllowed);
        }

        [Fact]
        public void Should_return_true_when_several_scemes_used_but_one_is_correct()
        {
            // Arrange
            CheckRequests.Setup(x => x.Headers).Returns(new NameValueCollection
            {
                {"Authorization", "Basic MyToken"},
                {"Authorization", "Bearer MyToken"}
            });

            // Act
            var isAllowed = RequestAuthoriser.IsAllowed();

            // Assert
            Assert.True(isAllowed);
        }

    }
}
