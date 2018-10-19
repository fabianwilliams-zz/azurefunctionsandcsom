#From Kirk Evans Blog
 # https://blogs.msdn.microsoft.com/kaevans/2016/08/12/using-powershell-with-certificates/

 $cert = New-SelfSignedCertificate -KeyExportPolicy Exportable `
 -Provider "Microsoft Strong Cryptographic Provider" `
 -Subject "CN=FabianSPOOfficeFiles" `
 -NotBefore (Get-Date) `
 -NotAfter (Get-Date).AddYears(2) `
 -CertStoreLocation "cert:\CurrentUser\My" `
 -KeyLength 2048

Export-Certificate -Type CERT -Cert $cert -FilePath "C:\1fabsCert\FabsWillyPrivateCertDemo1.cer"
$cred = Get-Credential
Export-PfxCertificate -Cert $cert -Password $cred.Password -FilePath "C:\1fabsCert\FabsWillyPrivateCertDemo1.pfx"
