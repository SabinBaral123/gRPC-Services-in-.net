using Grpc.Core;
using Microsoft.Extensions.Logging;
using PaymentCardgRPCService.Protos;

namespace PaymentCardgRPCService.Services
{
    public class PaymentCardService: PaymentCard.PaymentCardBase
    {

        private readonly ILogger<PaymentCardService> _logger;

        public PaymentCardService(ILogger<PaymentCardService> logger)
        {
            _logger = logger;
        }
        public override Task<CardResult> CheckCard(CardRequest request, ServerCallContext
context)
        {
            CardResult result = new CardResult();
            string numberString = request.Number.ToString();
            result.Valid = DoLuhn(numberString);
            result.IndustryType = GetIndustry(numberString);
            int i = numberString.Length - 4;
            if (i > 0)
                result.Truncated = new string('*', i) + numberString.Substring(i, 4);
            else
                result.Truncated = numberString;
            return Task.FromResult(result); // Result must be returned via a Task object
        }
        /* ------------------------------ Helper methods ------------------------------ */
        // Returns true if the payment card 'num' is valid according to the Luhn algorithm
        private bool DoLuhn(string num)
        {
            // From the rightmost digit, moving left through every digit
            int sum = 0;
            for (int i = num.Length - 1; i >= 0; i--)
            {
                // Get the digit's numeric value
                int value = num[i] - '0';
                // If this is an digit that is an odd number of positions from the last digit
                int offsetFromLast = num.Length - 1 - i;
                if (offsetFromLast % 2 != 0)
                {
                    // Double the digit's value
                    value *= 2;
                    // If the product of this doubling is > 9, sum the digits of the product
                    // (NOTE: same as subtracting 9 for sums less than 20 and greater than 9)
                    if (value > 9)
                        value -= 9;
                }
                // Add it to the sum
                sum += value;
            }
            // If the sum modulo 10 is equal to 0 then the number is valid
            return sum % 10 == 0;
        }
        private string GetIndustry(string num)
        {
            int mii = num[0] - '0';

            switch (mii)
            {
                case 0: return "ISO/TC 68 and other future industry assignments";
                case 1: return "Airlines";
                case 2: return "Airlines and other future industry assignments";
                case 3: return "Travel and entertainment and banking/financial";
                case 4: case 5: return "Banking and financial";
                case 6: return "Merchandising and banking/financial";
                case 7: return "Petroleum and other future industry assignments";
                case 8: return "Healthcare, telecommunications and other future industry assignments";

                case 9: return "For assignment by national standards bodies";
            }
            return "Unrecognized industry identifier";
        }

    }
}
