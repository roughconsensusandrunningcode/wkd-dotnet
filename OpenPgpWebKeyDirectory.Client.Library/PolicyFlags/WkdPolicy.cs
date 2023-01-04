/*
 *   This file is part of OpenPgpWebKeyDirectory.NET
 *   
 *   Copyright (c) 2022 Fabrizio Tarizzo <fabrizio@fabriziotarizzo.org>
 *   
 *   Licensed under MIT License (see LICENSE)
 */

using CSharpFunctionalExtensions;
using System.Net.Mail;

namespace OpenPgpWebKeyDirectory.Client.Library;

public class WkdPolicy
{
    /// <summary>
    /// "mailbox-only": The mail server provider does only accept keys
    /// with only a mailbox in the User ID.In particular User IDs with a
    /// real name in addition to the mailbox will be rejected as invalid.
    /// </summary>
    public bool MailboxOnly { get; internal set; } = false;

    /// <summary>
    /// "dane-only": The mail server provider does not run a Web Key
    /// Directory but only an OpenPGP DANE service.The Web Key Directory
    /// Update protocol is used to update the keys for the DANE service.
    /// The use of this keyword is deprecated.
    /// </summary>
    public bool DaneOnly { get; internal set; } = false;

    /// <summary>
    /// "auth-submit": The submission of the mail to the server is done
    /// using an authenticated connection.Thus the submitted key will be
    /// published immediately without any confirmation request.
    /// </summary>
    public bool AuthSubmit { get; internal set; } = false;

    /// <summary>
    /// "protocol-version": This keyword can be used to explicitly claim
    /// the support of a specific version of the Web Key Directory update
    /// protocol.This is in general not needed but implementations may
    /// have workarounds for providers which only support an old protocol
    /// version.If these providers update to a newer version they should
    /// add this keyword so that the implementation can disable the
    /// workaround.The value is an integer corresponding to the
    /// respective draft revision number.
    /// </summary>
    public Maybe<int> ProtocolVersion { get; internal set; } = Maybe.None;

    /// <summary>
    /// "submission-address": An alternative way to specify the submission
    /// address. The value is the addr-spec part of the address to send
    /// requests to this server. If this keyword is used in addition to
    /// the "submission-address" file, both MUST have the same value.
    /// </summary>
    public Maybe<MailAddress> SubmissionAddress { get; internal set; } = Maybe.None;

    private IEnumerable<string> Flags()
    {
        if (MailboxOnly)
            yield return "mailbox-only";

        if (DaneOnly)
            yield return "dane-only";

        if (AuthSubmit)
            yield return "auth-submit";

        if (ProtocolVersion.HasValue)
            yield return $"protocol-version: {ProtocolVersion.Value}";

        if (SubmissionAddress.HasValue)
            yield return $"submission-address: {SubmissionAddress.Value}";
    }

    public override string ToString()
        => string.Join("\n", Flags());

}
