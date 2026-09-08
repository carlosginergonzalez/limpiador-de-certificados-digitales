using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace AsesoriaColon.LimpiarCertificados
{
    internal sealed class CertInfo
    {
        public string Thumbprint { get; set; }
        public string Subject { get; set; }
        public string Issuer { get; set; }
        public DateTime NotBefore { get; set; }
        public DateTime NotAfter { get; set; }
        public bool HasPrivateKey { get; set; }
        public string StoreLocation { get; set; }
        public string StoreName { get; set; }
        public bool IsExpired
        {
            get { return DateTime.Now > NotAfter; }
        }
        public string FriendlyName { get; set; }
    }

    internal static class CertificateService
    {
        public static readonly string[] CommonStores = new[]
        {
            "My",
            "Root",
            "CA",
            "TrustedPeople",
            "AddressBook"
        };

        public static List<CertInfo> List(StoreLocation location, StoreName storeName)
        {
            var list = new List<CertInfo>();
            using (var store = new X509Store(storeName, location))
            {
                store.Open(OpenFlags.ReadOnly);
                foreach (X509Certificate2 cert in store.Certificates)
                {
                    using (cert)
                    {
                        list.Add(ToInfo(cert, location, storeName));
                    }
                }
            }
            list.Sort((a, b) => string.Compare(a.Subject, b.Subject, StringComparison.OrdinalIgnoreCase));
            return list;
        }

        public static List<CertInfo> List(StoreLocation location, string storeName)
        {
            return List(location, (StoreName)Enum.Parse(typeof(StoreName), storeName, true));
        }

        public static int Delete(IEnumerable<CertInfo> targets)
        {
            int deleted = 0;
            foreach (CertInfo info in targets)
            {
                var location = (StoreLocation)Enum.Parse(typeof(StoreLocation), info.StoreLocation, true);
                var storeName = (StoreName)Enum.Parse(typeof(StoreName), info.StoreName, true);
                using (var store = new X509Store(storeName, location))
                {
                    store.Open(OpenFlags.ReadWrite);
                    var found = store.Certificates.Find(X509FindType.FindByThumbprint, info.Thumbprint, false);
                    foreach (X509Certificate2 cert in found)
                    {
                        store.Remove(cert);
                        cert.Dispose();
                        deleted++;
                    }
                }
            }
            return deleted;
        }

        static CertInfo ToInfo(X509Certificate2 cert, StoreLocation location, StoreName storeName)
        {
            return new CertInfo
            {
                Thumbprint = cert.Thumbprint ?? "",
                Subject = SimplifyDn(cert.Subject),
                Issuer = SimplifyDn(cert.Issuer),
                NotBefore = cert.NotBefore,
                NotAfter = cert.NotAfter,
                HasPrivateKey = cert.HasPrivateKey,
                StoreLocation = location.ToString(),
                StoreName = storeName.ToString(),
                FriendlyName = string.IsNullOrWhiteSpace(cert.FriendlyName) ? "" : cert.FriendlyName
            };
        }

        static string SimplifyDn(string dn)
        {
            if (string.IsNullOrEmpty(dn)) return "(sin asunto)";
            // Prefer CN=...
            foreach (var part in dn.Split(','))
            {
                var p = part.Trim();
                if (p.StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                    return p.Substring(3).Trim();
            }
            return dn;
        }
    }
}
