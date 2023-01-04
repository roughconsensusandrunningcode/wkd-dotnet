# Examples

## Direct Client instantiation (deprecated)
```csharp
var client = new WkdClient<PgpPublicKeyRing>(
    new HttpClient(),
    NullLogger<WkdClient<PgpPublicKeyRing>>.Instance,
    new BouncyCastleKeyParser(),
    WkdKeyringValidatorBuilder<PgpPublicKeyRing>.Default);
```

## Client instantiation via dependency injection
```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddWebKeyDirectory<PgpPublicKeyRing, BouncyCastleKeyParser>();
var serviceProvider = serviceCollection.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IWkdClient<PgpPublicKeyRing>>();
```

## Client instantiation via dependency injection with customization
```csharp
var serviceCollection = new ServiceCollection();
serviceCollection.AddWebKeyDirectory<PgpPublicKeyRing, BouncyCastleKeyParser>(builder =>
    {
        // Set connection timeout
    	builder.SetTimeout(TimeSpan.FromSeconds(15));
    	  
    	// Set HTTP user agent
    	builder.SetUserAgent("WKD Client/0.1");
    	  
    	// Customize key validation
    	// e.g. rejects RSA keys weaker than 2048 bits
    	builder.ConfigureKeyValidation(opts => opts.RejectWeakKeys(
    	    2048,
    		Library.Contracts.OpenPgpPublicKeyAlgorithm.RsaEncryptOnly,
    		Library.Contracts.OpenPgpPublicKeyAlgorithm.RsaEncryptOrSign,
    		Library.Contracts.OpenPgpPublicKeyAlgorithm.RsaSignOnly));
    });
var serviceProvider = serviceCollection.BuildServiceProvider();
var client = serviceProvider.GetRequiredService<IWkdClient<PgpPublicKeyRing>>();
```

## Key discovery
```csharp
string email = "fabrizio@fabriziotarizzo.org";
Console.Write($"Checking Public Key(s) for {email}... ");
await client.DiscoverKeyAsync(email)
    .TapError(errors =>
        {
	        // Handle errors
            Console.WriteLine("FAILED.");
   	        foreach (var error in errors)
	        Console.WriteLine($"   {error.Message}");
        })
    .Tap(response =>
        {
	        Console.WriteLine($"OK ({response.Method} Method).");
		    var keys = response.Keys;
		    Console.WriteLine($"Found {keys.Count} key(s) for {email}.");
		 
		    short i = 1;
		    foreach (var key in keys)
		    {
		    Console.WriteLine($"   Key {i}: Fingerprint {key.Fingerprint}");
			i++;
            }
		
		if (response.HasRejectedKeys)
		{
		    var rejectedKeys = response.RejectedKeys;
		    Console.WriteLine($"Rejected {rejectedKeys.Count} key(s) for {email}.");

		    i = 1;
		    foreach (var rejectedKey in rejectedKeys)
		    {
		        Console.WriteLine($"   Key {i}: Fingerprint {rejectedKey.Key.Fingerprint} ({rejectedKey.RejectionReason.Message})");
			    i++;
		    }
		}
    });
```

## Policy flags
```csharp
string domain = "fabriziotarizzo.org";
Console.Write($"Checking policy file for domain {domain}... ");
await client.GetPolicyFlagsAsync(domain)
    .TapError(errors =>
    {
        // Handle errors
        Console.WriteLine("FAILED.");
        foreach (var error in errors)
            Console.WriteLine($"   {error.Message}");
    })
    .Tap(response =>
    {
        Console.WriteLine($"OK ({response.Method} Method).");
        if (response.Policy.MailboxOnly)
        {
            Console.WriteLine("The mail server provider does only accept keys with only a mailbox in the User ID");
        }
            
        var msg = response
            .Policy
            .ProtocolVersion
            .Map(version => $"This domain supports WDK protocol version {version}")
            .Or("No WDK protocol version specified");
                
        Console.WriteLine(msg);
    });
```

## Submission address
```csharp
string domain = "fabriziotarizzo.org";
Console.Write($"\nChecking Submission address for domain {domain}... ");
var submissionAddress = await client.GetSubmissionAddressAsync(domain);
submissionAddress
    .TapError(errors =>
    {
        // Handle errors
        Console.WriteLine("FAILED.");
        foreach (var error in errors)
            Console.WriteLine($"   {error.Message}");
    })
    .Tap(result =>
    {
        Console.WriteLine($"OK ({result.Method} Method).");
        var address = result.Address;
        Console.WriteLine($"   Submission address for {domain}: {address}");
    });
```

