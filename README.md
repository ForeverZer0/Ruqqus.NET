<p align="center">
<img src="https://raw.githubusercontent.com/ruqqus/ruqqus/master/ruqqus/assets/images/logo/ruqqus_text_logo.png" width="250"/>
</p>

<hr>

# Ruqqus.NET

[![Nuget](https://img.shields.io/nuget/v/Ruqqus.NET)](https://www.nuget.org/packages/Ruqqus.NET)
[![Build Status](https://travis-ci.org/ForeverZer0/Ruqqus.NET.svg?branch=master)](https://travis-ci.org/ForeverZer0/Ruqqus.NET)

Ruqqus is an open-source platform for online communities, free of censorship and moderator abuse by design. This project is a 
feature-rich .NET implementation of its API, with emphasis on ease-of-use, portability, and concurrency. 

## Features

* Build upon .NET Standard 2.1 with C# 8.0, can be used with any compatible .NET implementation, including .NET Core, .NET Framework, and Mono.
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
 
 ## 