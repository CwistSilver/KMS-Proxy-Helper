# KMS Proxy Helper

# Download
Current Build can be found under [Releases](https://github.com/CwistSilver/KMS-Proxy-Helper/releases).

# Create own build
Command to publish in to a single file: 
```ruby
dotnet publish -r win-x64 /p:PublishSingleFile=true /p:IncludeNativeLibariesForSelfExtract=true --output "c:SingleFileDeploy"
```
