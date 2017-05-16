PartyWifi.Server
=============

<img align="right" src="https://github.com/dbeuchler/PartyWifi.Server/raw/master/src/PartyWifi.Server/wwwroot/img/logo.png">

This is the central server of the **PartyWifi** ecosystem. We do not offer any pre-build versions, as each member of the community should compile and deploy the application the way he likes.

# PartyWifi
The concept of **PartyWifi** is to create a community franchise of developers and applications around the world. The target market of these applications are parties and events with a local WiFi network and a dedicated server. Guests of the event can use a range of digital services as long as they are connected to the events network. Photosharing, Slideshows, chats or seating arrangments are some examples of things that could be offered within the network.

# Usage
You can use this application for free or even redistribute it commercially without restrictions or license fees. Whether you install it for free at your own wedding or score a huge contract with a local club is not just permitted, but actually encouraged. You can also sell hardware that comes preinstalled with **PartyWifi** or build client-devices intended to be used in a **PartyWifi** network.

If you distribute hardware, applications or services of the **PartyWifi** ecosystem, you can [contact us](mailto:reseller@partywifi.com) to be listed as on official reseller on [www.partywifi.com](https://www.partywifi.com). We will ask you to sign a reseller contract that includes a provision for all business you receive over the page. Business you make by yourself remains untouched.

When you modifiy the code, please keep in mind, that **PartyWifi** and all other repositiries maintained by us are licensed under [GPL v3](blob/master/LICENSE) and therefor you are legally obligated to make your changes open source as well. This includes the clause that software installed on hardware sold or lent to customers must be replacable. 

# Develop
````
// Clone the repository
git clone git@github.com:dbeuchler/PartyWifi.Server.git

// Go to project folder
cd src/PartyWifi.Server/

// Install gulp-cli if you do not already have it
npm install gulp-cli -g

// Install npm dependencies
npm install

// Run default gulp tasks
gulp default

// Build ASP.NET Core project
dotnet build
dotnet run
````

# MyGet Feed
We use [ImageSharp](https://github.com/JimBobSquarePants/ImageSharp) that requires an additional nuget feed in NuGet.Config until they reach beta stage.
