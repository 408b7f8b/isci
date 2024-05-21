using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Serilog;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace isci.Sicherheit
{
    public static class Sicherheit
    {
        //erstmal übernommen aus https://stackoverflow.com/questions/2315257/how-to-create-a-completely-new-x509certificate2-in-net, muss aber verifiziert und möglicherweise angepasst werden
        public static X509Certificate2 GenerateSelfSignedCertificate()
        {
            string secp256r1Oid = "1.2.840.10045.3.1.7";  //oid for prime256v1(7)  other identifier: secp256r1

            string subjectName = "Self-Signed-Cert-Example";

            var ecdsa = ECDsa.Create(ECCurve.CreateFromValue(secp256r1Oid));

            var certRequest = new CertificateRequest($"CN={subjectName}", ecdsa, HashAlgorithmName.SHA256);

            //add extensions to the request (just as an example)
            //add keyUsage
            certRequest.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, true));

            X509Certificate2 generatedCert = certRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(10)); // generate the cert and sign!

            //hier ist jetzt ein Passwort eingetragen, das wahrscheinlich änderbar sein sollte? Wozu brauche ich es? Ist ein hartgecodetes Passwort für den Export eine Gefahr?
            X509Certificate2 pfxGeneratedCert = new X509Certificate2(generatedCert.Export(X509ContentType.Pfx, "passwort"), "passwort"); //has to be turned into pfx or Windows at least throws a security credentials not found during sslStream.connectAsClient or HttpClient request...

            return pfxGeneratedCert;
        }

        public static void ZertifikatAlsDatei(string pfad)
        {
            var cert = GenerateSelfSignedCertificate();
            System.IO.File.WriteAllBytes(pfad, cert.Export(X509ContentType.Pfx, "passwort"));
        }
    }
}