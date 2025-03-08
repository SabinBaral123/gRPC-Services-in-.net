using Grpc.Net.Client;
using PaymentCardgRPCService;
using PaymentCardgRPCService.Protos;
using System.Globalization;


// Connect to the service 

var channel = GrpcChannel.ForAddress("https://localhost:7112");

// Create a parameter of the type required by the service method 

var card = new PaymentCard.PaymentCardClient(channel);
// Create a parameter of the type required by the service method
CardRequest request = new();

// Repeadedly preocess card numbers from the user 

do
{
    //user input validation loop
    bool success = false;
    do
    {
        Console.Write("Enter a payment card Number (digits only) or 0 to quit : ");

        string inputText = Console.ReadLine() ?? "";

        try
        {
            request.Number = Convert.ToUInt64(inputText);
            success = true;
        }

        catch (FormatException){
            Console.WriteLine("A card number may only contain decimal digits.");

        }

    } while (!success);

    if (request.Number != 0)
    {
        // use the service and report the results 

        Console.WriteLine("\n Thank you. Here is your payment card nukmber Report ... \n");
        CardResult result  = card.CheckCard(request);

        Console.WriteLine($"The truncated card number is {result.Truncated}");

        Console.WriteLine($"The is a {(result.Valid ? " " : "n in ")} valid card number.");
        Console.WriteLine($"The type of industry that issued the card number is {result.IndustryType}.");
    }
} while (request.Number != 0);

Console.WriteLine("All done. Press a key to exit");
Console.ReadKey();

