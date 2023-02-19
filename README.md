[![.NET](https://github.com/roughconsensusandrunningcode/wkd-dotnet/actions/workflows/dotnet.yml/badge.svg)](https://github.com/roughconsensusandrunningcode/wkd-dotnet/actions/workflows/dotnet.yml) [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=roughconsensusandrunningcode_wkd-dotnet&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=roughconsensusandrunningcode_wkd-dotnet) [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=roughconsensusandrunningcode_wkd-dotnet&metric=reliability_rating)](https://sonarcloud.io/summary/new_code?id=roughconsensusandrunningcode_wkd-dotnet) [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=roughconsensusandrunningcode_wkd-dotnet&metric=sqale_rating)](https://sonarcloud.io/summary/new_code?id=roughconsensusandrunningcode_wkd-dotnet) [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=roughconsensusandrunningcode_wkd-dotnet&metric=security_rating)](https://sonarcloud.io/summary/new_code?id=roughconsensusandrunningcode_wkd-dotnet) [![Coverage](https://sonarcloud.io/api/project_badges/measure?project=roughconsensusandrunningcode_wkd-dotnet&metric=coverage)](https://sonarcloud.io/summary/new_code?id=roughconsensusandrunningcode_wkd-dotnet)

# OpenPgpWebKeyDirectory for .NET
A C# implementation of the OpenPGP Web Key Directory (WKD) (https://datatracker.ietf.org/doc/draft-koch-openpgp-webkey-service/), a service to locate OpenPGP keys by mail address using a Web service and the HTTPS protocol.

### OpenPgpWebKeyDirectory.Client.Library
WKD Client library. Implements Key discovery, Policy file and Submission Address fetching, and basic key validation logic. It also defines two contract interfaces (`IPgpKeyParser` and `IPgpKeyWrapper`) for application-defined keyring parsing.

Uses: [CSharpFunctionalExtensions](https://github.com/vkhorikov/CSharpFunctionalExtensions)

### OpenPgpWebKeyDirectory.Client.Library.BouncyCastle
Implements `IPgpKeyParser` and `IPgpKeyWrapper` using the [Bouncy Castle 2.1](https://www.bouncycastle.org/csharp/) crypto library.

### OpenPgpWebKeyDirectory.Client.Library.Extensions.DependencyInjection
Methods and classes for configuring the WKD Client and related services into the Microsoft dependency injection container.

### OpenPgpWebKeyDirectory.Client.ConsoleApp.WkdChecker
An example commandline application, a small program that
* given a domain, verifies that the WELLKNOWN/policy file is present and well-formed, the WELLKNOWN/submission-address is present and there is a valid public key available for the submission address.
* given an email address, tries to discover the key(s)

### OpenPgpWebKeyDirectory.Client.Library.Tests
Test suite, mostly based on the Java implementation test suite (https://github.com/pgpainless/wkd-java/tree/main/wkd-test-suite)

## Maturity and Versioning
This library implements a protocol that is currently in [Internet-Draft](https://datatracker.ietf.org/doc/html/rfc2026#section-2.2) status. The library itself is a work in progress, so the API is unstable and may change anytime.
Currently, the library is versioned after the supported version of the [specification](https://datatracker.ietf.org/doc/draft-koch-openpgp-webkey-service/), with the scheme `0.{draft-version}.{patch}-draft`. The I-D is currently at version 15 (released 2022-11-14, expires on 2023-05-18), so the library version is `0.15.0-draft`. When the specification will be stable and published as an RFC, the usual semantic version will be used.

## Copyright
Copyright (c) 2022 [Fabrizio Tarizzo](https://www.fabriziotarizzo.org/)

## License
This project is licensed under the [MIT License](https://opensource.org/licenses/MIT)

## References
* [WKD Specification](https://datatracker.ietf.org/doc/html/draft-koch-openpgp-webkey-service)

## Guides and Tutorials
* [WKD on the GnuPG Wiki](https://wiki.gnupg.org/WKD)
* [Setting up OpenPGP Web Key Directory (WKD)](https://www.uriports.com/blog/setting-up-openpgp-web-key-directory/) (by uriports.com)
* [How to set up PGP WKD (Web Key Directory)](https://www.sindastra.de/p/1905/how-to-set-up-pgp-wkd-web-key-directory) (by sindastra.de)
* [How to setup your own WKD server](https://shibumi.dev/posts/how-to-setup-your-own-wkd-server/) (by Christian Rebischke)
* [WKD Checker](https://metacode.biz/openpgp/web-key-directory) (by metacode.biz)

## Other implementations
* [Java library](https://github.com/pgpainless/wkd-java)
* [JavaScript library](https://github.com/openpgpjs/wkd-client)
* [Express middleware](https://codeberg.org/yarmo/express-wkd)
* [Go library](https://github.com/emersion/go-openpgp-wkd)
* [WKD linter in Rust](https://gitlab.com/wiktor/wkd-checker)
