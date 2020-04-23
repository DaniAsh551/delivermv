using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Deliver.Web
{
    public static class IdentityExtensions
    {
        /// <summary>
        /// Gets the currently logged in user Id.
        /// </summary>
        /// <param name="claimsPrincipal"></param>
        /// <returns></returns>
        public static string GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if(claimsPrincipal.Identity.IsAuthenticated)
                return claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier).Value;

            return null;
        }

        public static X509Certificate2 LoadPemCertificate(string certificatePath, string privateKeyPath)
        {
            var publicKey = new X509Certificate2(certificatePath);

            var privateKeyText = File.ReadAllText(privateKeyPath);
            var privateKeyBlocks = privateKeyText.Split("-", StringSplitOptions.RemoveEmptyEntries);
            var privateKeyBytes = Convert.FromBase64String(privateKeyBlocks[1]);
            var rsa = RSA.Create();

            if (privateKeyBlocks[0] == "BEGIN PRIVATE KEY")
            {
                rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            }
            else if (privateKeyBlocks[0] == "BEGIN RSA PRIVATE KEY")
            {
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            }

            var keyPair = publicKey.CopyWithPrivateKey(rsa);
            var cert = new X509Certificate2(keyPair.Export(X509ContentType.Pfx));

            return cert;
        }
    }
}
