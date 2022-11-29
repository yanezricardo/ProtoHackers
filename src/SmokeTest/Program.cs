using SmokeTest;

var server = new TcpEchoServer("127.0.0.1", 2067);
try
{
    await server.Start();
}
catch (Exception ex)
{
    Console.WriteLine("Exception: {0}", ex);
}
finally
{
    await server.Stop();
}