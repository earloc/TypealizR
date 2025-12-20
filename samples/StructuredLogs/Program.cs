
using StructuredLogs;

var services = new ServiceCollection();
services.AddLogging(_ => _.AddJsonConsole());
services.AddLocalization();

var provider = services.BuildServiceProvider();

var localizer = provider.GetRequiredService<IStringLocalizer<Strings>>();
var logger = provider.GetRequiredService<ILogger<Strings>>();

// unconditionally log a localized, already interpolated message
logger.LogInformation(localizer.Hello__Name("Arthur"));


// unconditionally log a localized message with no compiler help in spotting missing/mismatching values, works troday
logger.LogInformation(localizer.Hello_Structured(), "Saphod", DateTimeOffset.Now); // works
try
{
    logger.LogInformation(localizer.Hello_Structured(), "Arthur"); //throws at runtime due to missing "Date" argument
}
catch
{
}

// unconditionally try to log a localized message, which throws due to invalid "format-string"
try
{
    logger.LogInformation(localizer.Hello_Invalid__Name("Arthur"));
}
catch (FormatException ex)    
{
    logger.LogError("failed to log due to {ex}", ex);
}

/* might work in the future with better compiler support for source-generated format strings
    
    // getting the raw key-name, then providing "known" arguments"
    logger.LogInformation(localizer.Hello_Invalid__Name_Key(), "Saphod", DateTimeOffset.Now); // might work seemless, but still no compiler help (throws at runtime, if f.e. arguments are missing, which is really bad for logging)
    logger.LogInformation(localizer.Keys().Hello_Invalid__Name(), "Saphod", DateTimeOffset.Now); // might work seemless, but still no compiler help (throws at runtime, if f.e. arguments are missing, which is really bad for logging)

    // use a (generated) extension for ILogger<>, providing a Func<>, which might require further arguments
    logger.LogInformation(localizer.Hello_Structured, "Saphod", DateTimeOffset.Now) // might give at least some compiler help (types) due to delegate signature demanding additional arguments
        might introdudce a lot of ambigiousity      

    // Similar to above, but with a more fluent interface / syntax...might call for overloads, so usual usage might become less obvious ( Hello_Structured_Log() vs. Hello_Structured_Log("Arthur", DateTimeOffset.Now) )
    localizer.Hello_Structured_Log().Information(logger, "Trillian", DateTimeOffset.Now); // might work with compiler help

*/