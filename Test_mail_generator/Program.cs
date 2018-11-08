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
        public static void GenerateLetter(int count, int bodySize)
        {
            string to = @"fenran@yandex.ru";
            string from = @"" + count + "test@test.com";
            string subject = @"Generated letter No " + count + "";
            string body = @"This letter number " + count + " was generated to fill the mailbox with test letters. It may contain different attachments as well as a different amount of textual load of different languages.\nText:\n" + RandomString(bodySize);
            string emailDir = @"D:\\StudioSolution\\Test_mail_generator\\Test_mail_generator\\bin\\Debug\\Mails";
            string msgName = @"" + count + "_test_letter.eml";

            Console.WriteLine("Saving letter...");

            using (var client = new SmtpClient())
            {
                MailMessage msg = new MailMessage(from, to, subject, body);
                client.UseDefaultCredentials = true;
                client.DeliveryMethod =
                   SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = emailDir;
                try
                {
                    client.Send(msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught: {0}", ex.ToString());
                    Console.ReadLine();
                    System.Environment.Exit(-1);
                }
            }
            var defaultMsgPath = new
            DirectoryInfo(emailDir).GetFiles()
               .OrderByDescending(f => f.LastWriteTime)
               .First();
            var realMsgPath = Path.Combine(emailDir, msgName);
            try
            {
                File.Move(defaultMsgPath.FullName, realMsgPath);
                Console.WriteLine("Letter " + count + " saved.");
            }
            catch (System.IO.IOException e)
            {
                Console.WriteLine("Overwriting existing file...");
                File.Delete(realMsgPath);
                File.Move(defaultMsgPath.FullName, realMsgPath);
                Console.WriteLine("Letter " + count + " overwrited.");
            }
        }

        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            int counter;

            if (size > 100)
            {
                counter = (int)Math.Floor((double)(size / 100));
            }
            else
            {
                counter = 100;
            }

            for (int i = 0; i < size / 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));

                    builder.Append(ch);
                }
                builder.Append("\n");
            }
            return builder.ToString();
        }

        static void Main()
        {
            int letterCount = 10;
            for (var i = 0; i < letterCount; i++)
            {
                GenerateLetter(i, 1000);
            }
            Console.WriteLine("All letters saved.");
            Console.ReadLine();
        }
    }
}
