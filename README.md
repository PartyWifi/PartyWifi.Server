WeddiFi.Server
=============

Server of the **WeddiFi** ecosystem.

# MyGet Feed
We use [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) that requires an additional nuget feed in NuGet.Config until they reach beta stage.

````xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
    <!-- MyGet feed for ImageSharp -->
    <add key="imagesharp" value="https://www.myget.org/F/imagesharp/api/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
````