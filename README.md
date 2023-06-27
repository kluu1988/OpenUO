<p align="center">
    <img src="https://i.imgur.com/UbHNQdg.png" width="500" height="150" >
</p>

An open source implementation of the Ultima Online Enhanced Client which is based off of the ClassicUO game client, with the vision of enhancing the client to offer modern game experience with the same feel as the Ultima Online game client.

<a href="https://discord.gg/vCRauYPk8A">
<img src="https://img.shields.io/discord/1091497866457006152?logo=discord"
alt="chat on Discord"></a>

# Introduction
OpenUO should be considered a child project of the [ClassicUO](https://github.com/ClassicUO/ClassicUO) repository, we will continue to receive updates and ClassicUO is more than welcome to take any of our features. 

With the difference in vision of providing an experience which surpass the features offered by the original game client. 

New features will be able to detected through custom packets and file formats, but all features must be optional and be turned off by default, and original functionality in place.

Features such as custom file formats **must** be detected through client side detection of the custom formats.

Features added through custom packets **must** be enabled or disabled through a handshake with the server.

PRs will only be accepted if they come along with a PR on one of the accepted emulators. The emulator side PR must include basic functionality to operator your new feature. It will not require your full implementation as there is a reasonable expection that server side code is private.

The client is currently under heavy development but is functional. The code is based on the [FNA-XNA](https://fna-xna.github.io/) framework. C# is chosen because there is a large community of developers working on Ultima Online server emulators in C#, because FNA-XNA exists and seems reasonably suitable for creating this type of game.


OpenUO is natively cross platform and supports:
* Browser [Chrome]
* Windows [DirectX 11, OpenGL, Vulkan]
* Linux   [OpenGL, Vulkan]
* macOS   [Metal, OpenGL, MoltenVK]

# Download & Play!
| Platform | Link |
| --- | --- |
| Browser | Not Yet Available |
| Windows x64 | Not Yet Available |
| Linux x64 | Not Yet Available |
| macOS | Not Yet Available |

Or visit the [OpenUO Website](https://www.openuo.io/)

# How to build the project

Clone repository with:
```
git config --global url."https://".insteadOf git://
git clone --recursive https://github.com/Open-UO/OpenUO.git
```

Build the project:
```
dotnet build -c Release
```

**Note:** To build on windows visual studio the project currently requires Visual Studio 2022 and Visual Studio 2019. 


# Contribute
Everyone is welcome to contribute! But, will follow the requirements specified in the introduction. 

### ClassicUO

The project is heavily reliant on the continued work by Karasho and the ClassicUO development, you can support them in the following ways: 

Individuals/hobbyists: support continued maintenance and development via the monthly Patreon:
<br>&nbsp;&nbsp;[![Patreon](https://raw.githubusercontent.com/wiki/ocornut/imgui/web/patreon_02.png)](http://www.patreon.com/classicuo)

Individuals/hobbyists: support continued maintenance and development via PayPal:
<br>&nbsp;&nbsp;[![PayPal](https://www.paypalobjects.com/en_US/i/btn/btn_donate_LG.gif)](https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=9ZWJBY6MS99D8)

# Legal
The code itself has been written using the following projects as a reference:

* [ClassicUO](https://github.com/ClassicUO/ClassicUO)
* [OrionUO](https://github.com/hotride/orionuo)
* [Razor](https://github.com/msturgill/razor)
* [UltimaXNA](https://github.com/ZaneDubya/UltimaXNA)
* [ServUO](https://github.com/servuo/servuo)

Backend:
* [FNA](https://github.com/FNA-XNA/FNA)

This work is released under the BSD 4 license. This project does not distribute any copyrighted game assets. In order to run this client you'll need to legally obtain a copy of the Ultima Online Classic Client.
Using a custom client to connect to official UO servers is strictly forbidden. We do not assume any responsibility of the usage of this client.

Ultima Online(R) © 2022 Electronic Arts Inc. All Rights Reserved.
