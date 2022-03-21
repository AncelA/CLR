using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CLR
{
    public class CLR
    {
        [Microsoft.SqlServer.Server.SqlProcedure()]
        public static void ReadData(DateTime DateOfBirth)
        {
            // Declaration and default setting for the result variable.
            string result = "<EOF>";
            // The query being run against the database.
            string queryString = "SELECT * from Customer where DateOfBirth = '" + DateOfBirth + "' FOR JSON AUTO";
            // The connection string to the database.
            string connectionString = "Data Source=.;Initial Catalog=Customer;Integrated Security=True";
            // Using the sql connection to the database created by using the previously declared connection string.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // The query is run against the database
                SqlCommand command = new SqlCommand(queryString, connection);
                connection.Open();
                // The query resultset is read.
                SqlDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        // The data returned from the execution of the query is set to the result variable.
                        result = reader.GetString(0);
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
                // Closing of the connection made to the database.
                connection.Close();
            }

            // The result variable is sent to the ExecuteClient method.
            ExecuteClient(result);
        }

        static void ExecuteClient(string result)
        {
            try
            {
                // Establish the remote endpoint for the socket. The port 11111 on the local computer is used.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

                // Creation TCP/IP Socket using Socket Class Constructor.
                Socket sender = new Socket(ipAddr.AddressFamily,
                           SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Connect Socket to the remote endpoint using method Connect().
                    sender.Connect(localEndPoint);

                    // Creation of message that we will send to Server.
                    byte[] messageSent = Encoding.ASCII.GetBytes(result);

                    // Sending of the message
                    int byteSent = sender.Send(messageSent);

                    // Close Socket using the method Close().
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }

                // Manage of Socket's Exceptions
                catch (ArgumentNullException ane)
                {

                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }

                catch (SocketException se)
                {

                    Console.WriteLine("SocketException : {0}", se.ToString());
                }

                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }

            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }
        }
    }
}
