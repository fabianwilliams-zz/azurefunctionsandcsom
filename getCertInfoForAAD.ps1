#These first 3 lines are a repeat from the bottom of the createCert file for context

Export-Certificate -Type CERT -Cert $cert -FilePath "C:\1fabsCert\FabsWillyPrivateCertDemo1.cer"
$cred = Get-Credential
Export-PfxCertificate -Cert $cert -Password $cred.Password -FilePath "C:\1fabsCert\FabsWillyPrivateCertDemo1.pfx"
# the above is just for context and taken from the last 3 lines in the CreateCert file

$fabswillycer = New-Object System.Security.Cryptography.X509Certificates.X509Certificate2
$fabswillycer.Import("C:\1fabsCert\FabsWillyPrivateCertDemo1.cer")
$bin = $fabswillycer.GetRawCertData()
echo $bin
$base64Value = [System.Convert]::ToBase64String($bin)
echo $base64Value
$bin = $fabswillycer.GetCertHash()
$base64Thumbprint = [System.Convert]::ToBase64String($bin)
echo $base64Thumbprint
$keyid = [System.Guid]::NewGuid().ToString()
echo $keyid
$startDate = $($fabswillycer.NotAfter.ToString("s"))
echo $startDate