<p align="center">
<img src="https://raw.githubusercontent.com/ruqqus/ruqqus/master/ruqqus/assets/images/logo/ruqqus_text_logo.png" width="250"/>
</p>

<hr>

# Ruqqus<span/>.NET

[![Nuget](https://img.shields.io/nuget/v/Ruqqus.NET)](https://www.nuget.org/packages/Ruqqus.NET)
[![Build Status](https://img.shields.io/github/workflow/status/ForeverZer0/Ruqqus.NET/.NET%20Core)](https://github.com/ForeverZer0/Ruqqus.NET/actions?query=workflow%3A%22.NET+Core%22)
[![Build status](https://ci.appveyor.com/api/projects/status/v4eth9ag7xonjebg?svg=true)](https://ci.appveyor.com/project/ForeverZer0/ruqqus-net)
[![OpenIssues](https://img.shields.io/github/issues/ForeverZer0/ruqqus)](https://github.com/ForeverZer0/ruqqus/issues)
[![License](https://img.shields.io/github/license/ForeverZer0/ruqqus)](https://opensource.org/licenses/MIT)


[Ruqqus](https://ruqqus.com/) is an [open-source](https://github.com/ruqqus/ruqqus) platform for online communities, free of censorship and moderator abuse by design. This project is a 
feature-rich .NET implementation of its API, with emphasis on ease-of-use, portability, and concurrency. 

## Features

* Built upon .NET Standard 2.1 with C# 8.0, can be used with any compatible .NET implementation, including .NET Core, .NET Framework, and Mono.
* Fully compliant with the Common Language Specification (CLS), allowing it to be consumed by any other CLS language, including C#,
 F#, Visual Basic, etc.
* Fully self-sufficient, no external dependencies or dependent NuGet packages.
 * Complete coverage of features, including creating posts/comments, voting, and enumeration of various types, such as guilds, posts,
 comments, and more (sorted and filtered as desired).
 * Nearly complete asynchronous API, leveraging the async/await pattern to take advantage of .NET's concurrency and multi-tasking
  power.
 * Included helper classes for anonymous Imgur uploads, "new post watchers" that fires events when new content is posted on Ruqqus,
 and more.
 * Automated system for generating an OAuth2 access token for desktop consumers (once authorization has been approved by user on Ruqqus). 
 
## Installation

Release versions are hosted on [NuGet](https://www.nuget.org/packages/Ruqqus.NET/) and can be pulled from there by package managers.

For .NET Core projects, you can use the [.NET Core CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/) to retrieve it with the following command from a terminal:

    $ dotnet add package Ruqqus.NET

Alternatively you can open the [Package Manager Console](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-powershell) and run the following command:

    PM> Install-Package Ruqqus.NET

 