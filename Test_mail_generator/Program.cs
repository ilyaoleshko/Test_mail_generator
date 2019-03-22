using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;

namespace Test_mail_generator
{
    class Program
    {
        private static void GenerateLetter(int count, int bodySize, int countAttach)
        {
            string from = @"" + count + "test@test.com";
            string to = @"" + count + "test@test.com";
            string subject = @"Generated letter No " + count + ", has " + countAttach + " attachments";
            string body = @"This letter number " + count + " was generated to fill the mailbox with test letters. " +
                           "It may contain " + countAttach + " attachments as well as a different amount of textual load of different languages.\n" +
                           "Text:\n" + RandomString(bodySize);

            string emailDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\bin\Debug\Mails"));
            string attachPath = @"" + emailDir + "\\Attachments\\attach.csv";

            using (var client = new SmtpClient())
            {
                MailMessage msg = new MailMessage(from, to, subject, body);

                Parallel.For(0, countAttach, k =>
                {
                    msg.Attachments.Add(new Attachment(attachPath));
                });

                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = emailDir;
                client.Send(msg);
            }
        }

        private static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            int charPerStr = 100;
            int counter = charPerStr;

            if (size > charPerStr)
            {
                counter = (int)Math.Truncate((double)(size / charPerStr));
            }

            for (var i = 0; i < counter; i++)
            {
                for (int j = 0; j < charPerStr; j++)
                {
                    builder.Append((char)random.Next(48, 128));
                }
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        private static void CreateLetters()
        {
            Random r = new Random();
            int letterCount = 0;

            Console.Write("Enter letters count (10 by default): ");

            string value = Console.ReadLine();

            if (value.Length < 1)
                value = "10";

            var buffer = 0;

            for (int i = 0; i < value.Length; i++)
            {
                buffer = buffer * 10 + (value[i] - '0');
            }

            letterCount += buffer;

            ClearFolder();

            Console.Write("\nStarting generation ... \n\n");

            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var complitedTasks = 0;

            Parallel.For(0, letterCount, i =>
            {
                GenerateLetter(i, r.Next(100, 1000), r.Next(10, 1010));
                Console.Write("\rTasks complited: " + ++complitedTasks + " [ " + Math.Round(((double)complitedTasks / letterCount) * 100) + "% ]");
            });

            stopwatch.Stop();

            Console.WriteLine("\n\nAll tasks done: [ " + stopwatch.Elapsed + " ]\n\nPress key to close ... ");
            Console.ReadKey();
        }

        private static void ClearFolder()
        {
            Console.Write("\nDeleting old letters ...");

            DirectoryInfo directory = new DirectoryInfo(
                Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\bin\Debug\Mails")));

            Parallel.ForEach(directory.GetFiles(), file =>
            {
                file.Delete();
            });

            Console.Write("\rDeleting old letters ... Done\n");
        }

        static void Main()
        {
            CreateLetters();
        }
    }
}
