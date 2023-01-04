/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using OpenPgpWebKeyDirectory.Client.Library.Contracts;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library.KeyValidation;

internal sealed class MissingUserIdKeyValidator<TKey> : AbstractKeyValidator<TKey>
{
    protected override bool IsValid(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
        => key.HasMailAddress(context.LookupMailAddress);

    protected override WkdKeyRejectionReason RejectionReason(KeyDiscoveryContext context, IPgpKeyWrapper<TKey> key)
        => new RejectionForMissingUserId(context.LookupMailAddress);
}

public static partial class WkdKeyringValidatorBuilderExtensions
{
    public static WkdKeyringValidatorBuilder<TKey> RejectMissingUserId<TKey>(this WkdKeyringValidatorBuilder<TKey> builder)
        => builder.AddValidator(new MissingUserIdKeyValidator<TKey>());
}

public class RejectionForMissingUserId : WkdKeyRejectionReason
{
    public MailAddress MissingUserId { get; private set; }
    internal RejectionForMissingUserId(MailAddress missingUserId)
        : base($"Key does not contain a valid user-id with email {missingUserId}")
    {
        MissingUserId = missingUserId;
    }
}
