Console.WriteLine("Enterprise ERP System");
int requestCount=0;
// requestCount +=2;
// Console.WriteLine($"Purchase requests:{requestCount}");

// if (requestCount >= 5)
// {
//     Console.WriteLine("Manager review required!");
// }
// else
// {
//     Console.WriteLine("No review required");
// }


Console.Write("Enter request count:");
string? input = Console.ReadLine();

bool isValid = int.TryParse(input , out requestCount);

if (requestCount >= 6)
{
    Console.WriteLine("Manager review required!");
}
else
{
    Console.WriteLine("No review required");
}