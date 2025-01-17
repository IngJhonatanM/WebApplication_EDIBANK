using System.Net;
using System.Net.Mail;

namespace EDIBANK.Models.Users_EdiWeb
{
    public class EmailHelper
    {

        // methods Confirm Email
        /*   public bool SendEmail(string userEmail, string confirmationLink)
           {
               MailMessage mailMessage = new MailMessage();
               mailMessage.From = new MailAddress("relay@eniac.com");
               mailMessage.To.Add(new MailAddress(userEmail));

               mailMessage.Subject = "Confirm your email";
               mailMessage.IsBodyHtml = true;
               mailMessage.Body = confirmationLink;

               SmtpClient client = new SmtpClient();
               client.Credentials = new System.Net.NetworkCredential("relay@eniac.com", "Lol11473");
               client.Host = "outlook.office365.com";
               client.Port = 587;
               client.EnableSsl = true;

               try
               {
                   client.Send(mailMessage);
                   return true;
               }
               catch (Exception ex)
               {
                   // log exception
               }
               return false;
           }*/



        // methods for 2fa #2


        public bool SendEmailTwoFactorCode(string userEmail, string code)
        {
            // Create a MailMessage object with basic configuration
            var mailMessage = new MailMessage
            {
                From = new MailAddress("relay@eniac.com", "Your Company Name"),
                To = { new MailAddress(userEmail) },
                Subject = "Two Factor Authentication Code",
                IsBodyHtml = true
            };

            // Build the HTML body content with user-friendly formatting
            var htmlBody = $@"
        <html>
        <head>
        </head>
        <body>
            <p>Hola,</p>
            <p>Aquí está su código de autenticación de dos factores para su cuenta EDIB@NK:</p>
            <p style=""font-weight: bold; font-size: 18px;"">{code}</p>
            <p>Ingrese este código en la página de inicio de sesión para completar su autenticación.</p>
            <p>Para su seguridad, este código es válido por tiempo limitado. Si no solicitó este código, ignore este correo electrónico.</p>
            <p>Atentamente,</p>
            <p>Eniac Corporation.</p>
        </body>
        </html>
    ";

            // Set the HTML body content
            mailMessage.Body = htmlBody;

            // Configure the SmtpClient for secure email sending
            var client = new SmtpClient
            {
                Credentials = new NetworkCredential("relay@eniac.com", "/TuPsy98\\1i"),
                Host = "outlook.office365.com",
                Port = 587,
                EnableSsl = true
            };

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }

        // methods for ResetPassword

        public bool SendEmailPasswordReset(string userEmail, string link)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress("relay@eniac.com");
            mailMessage.To.Add(new MailAddress(userEmail));

            mailMessage.Subject = "Password Reset";
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = link;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential("relay@eniac.com", "/TuPsy98\\1i");
            client.Host = "outlook.office365.com";
            client.Port = 587;
            client.EnableSsl = true;

            try
            {
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                // log exception
            }
            return false;
        }
    }
}