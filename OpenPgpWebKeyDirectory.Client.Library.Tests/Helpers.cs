/*
*   This file is part of OpenPgpWebKeyDirectory.NET
*   
*   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
*   
*   Licensed under MIT License (see LICENSE)
*/

using CSharpFunctionalExtensions;
using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using OpenPgpWebKeyDirectory.Client.Library.Errors;
using OpenPgpWebKeyDirectory.Client.Library.Internals;
using OpenPgpWebKeyDirectory.Client.Library.Responses;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.Tests;

internal static class Helpers
{
    internal static T ShouldBeSuccess<T>(this Result<T, WkdErrorCollection> result)
    {
        Assert.IsNotNull(result);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.IsFailure);

        return result.Value;
    }

    internal static WkdErrorCollection ShouldBeFailure<T>(this Result<T, WkdErrorCollection> result)
    {
        Assert.IsNotNull(result);
        Assert.IsFalse(result.IsSuccess);
        Assert.IsTrue(result.IsFailure);

        return result.Error;
    }

    internal static T TestSuccessResponse<T>(this T result, Action<T> testAction)
    {
        testAction.Invoke(result);

        return result;
    }

    internal static WkdInternalResponse<T> TestSuccessResponse<T>(this WkdInternalResponse<T> response, Action<T> testAction)
    {
        testAction.Invoke(response.Value);
        return response;
    }

    internal static WkdErrorCollection TestFailureResponse(this WkdErrorCollection errors, Action<WkdErrorCollection> testAction)
    {
        testAction.Invoke(errors);

        return errors;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldHaveValidKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.IsTrue(response.Keys.Any());
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldHaveValidKeys<TKey>(this WkdDiscoveryResponse<TKey> response, int count)
    {
        Assert.AreEqual(count, response.Keys.Count);
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldNotHaveValidKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.IsFalse(response.Keys.Any());
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldHaveRejectedKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.IsTrue(response.HasRejectedKeys);
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldHaveRejectedKeys<TKey>(this WkdDiscoveryResponse<TKey> response, int count)
    {
        Assert.AreEqual(count, response.RejectedKeys.Count);
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldNotHaveRejectedKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.IsFalse(response.HasRejectedKeys);
        return response;
    }
    internal static WkdDiscoveryResponse<TKey> ShouldNotHaveExpiredKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.AreEqual(response.Keys.Count, response.Keys.ExcludeExpired().Count());
        return response;
    }

    internal static WkdDiscoveryResponse<TKey> ShouldNotHaveRevokedKeys<TKey>(this WkdDiscoveryResponse<TKey> response)
    {
        Assert.AreEqual(response.Keys.Count, response.Keys.ExcludeRevoked().Count());
        return response;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveFingerprint<TKey>(this IPgpKeyWrapper<TKey> key, string expectedFingerprint)
    {
        Assert.AreEqual(expectedFingerprint, key.Fingerprint);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveKeyId<TKey>(this IPgpKeyWrapper<TKey> key, string expectedKeyId)
    {
        Assert.AreEqual(expectedKeyId, key.KeyId);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveUserIds<TKey>(this IPgpKeyWrapper<TKey> key, params string[] expectedUserIds)
    {
        CollectionAssert.AreEquivalent(expectedUserIds, key.GetUserIds());
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, string address)
    {
        Assert.IsTrue(key.HasMailAddress(address));
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, MailAddress address)
    {
        Assert.IsTrue(key.HasMailAddress(address));
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveMailAddresses<TKey>(this IPgpKeyWrapper<TKey> key, params MailAddress[] addresses)
    {
        CollectionAssert.AreEquivalent(addresses, key.MailAddresses().ToList());
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldHaveMailAddresses<TKey>(this IPgpKeyWrapper<TKey> key, params string[] addresses)
    {
        CollectionAssert.AreEquivalent(addresses, key.MailAddresses().Select(a => a.Address).ToList());
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldNotHaveMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, string address)
    {
        Assert.IsFalse(key.HasMailAddress(address));
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldNotHaveMailAddress<TKey>(this IPgpKeyWrapper<TKey> key, MailAddress address)
    {
        Assert.IsFalse(key.HasMailAddress(address));
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveVersion<TKey>(this IPgpKeyWrapper<TKey> key, int version)
    {
        Assert.AreEqual(version, key.Version);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveAlgorithm<TKey>(this IPgpKeyWrapper<TKey> key, OpenPgpPublicKeyAlgorithm algorithm)
    {
        Assert.AreEqual(algorithm, key.Algorithm);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldHaveBitStrength<TKey>(this IPgpKeyWrapper<TKey> key, int bitStrength)
    {
        Assert.AreEqual(bitStrength, key.BitStrength);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldBeExpired<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsTrue(key.IsExpired);
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldNotBeExpired<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsFalse(key.IsExpired);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldBeRevoked<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsTrue(key.IsRevoked);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldNotBeRevoked<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsFalse(key.IsRevoked);
        return key;
    }

    internal static IPgpKeyWrapper<TKey> ShouldBeValidAt<TKey>(this IPgpKeyWrapper<TKey> key, int year, int month, int day, int hour = 0, int min = 0, int sec = 0)
    {
        Assert.IsTrue(key.IsValidAt(new DateTime(year, month, day, hour, min, sec)));
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldNotBeValidAt<TKey>(this IPgpKeyWrapper<TKey> key, int year, int month, int day, int hour = 0, int min = 0, int sec = 0)
    {
        Assert.IsFalse(key.IsValidAt(new DateTime(year, month, day, hour, min, sec)));
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldHaveCreationDate<TKey>(this IPgpKeyWrapper<TKey> key, int year, int month, int day)
    {
        Assert.AreEqual(new DateTime(year, month, day).Date, key.CreationTime.Date);
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldNotHaveExpirationTime<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsFalse(key.ExpirationTime.HasValue);
        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldHaveExpirationTime<TKey>(this IPgpKeyWrapper<TKey> key)
    {
        Assert.IsTrue(key.ExpirationTime.HasValue);

        return key;
    }
    internal static IPgpKeyWrapper<TKey> ShouldHaveExpirationDate<TKey>(this IPgpKeyWrapper<TKey> key, int year, int month, int day)
    {
        Assert.IsTrue(key.ExpirationTime.HasValue);
        Assert.AreEqual(new DateTime(year, month, day).Date, key.ExpirationTime.Value.Date);
        return key;
    }

    internal static WkdErrorCollection AllErrorsAreInstancesOf<T>(this WkdErrorCollection errors)
    {
        CollectionAssert.AllItemsAreInstancesOfType(errors.ToList(), typeof(T));
        return errors;
    }

    internal static WkdErrorCollection AllInnerNetworkErrorsAreInstancesOf<T>(this WkdErrorCollection errors)
    {
        var innerErrors = errors
            .OfType<WkdNetworkError>()
            .Select(err => err.NetworkException)
            .ToList();

        CollectionAssert.AllItemsAreInstancesOfType(innerErrors, typeof(T));
        return errors;
    }

    internal static WkdErrorCollection AllInnerNetworkErrorsAreInstancesOf<T1, T2>(this WkdErrorCollection errors)
    {
        var innerErrors = errors
            .OfType<WkdNetworkError>()
            .Select(err => err.NetworkException)
            .ToList();

        var innerExceptions = innerErrors
            .Select(err => err.InnerException)
            .ToList();

        CollectionAssert.AllItemsAreInstancesOfType(innerErrors, typeof(T1));
        CollectionAssert.AllItemsAreInstancesOfType(innerExceptions, typeof(T2));
        return errors;
    }
}
