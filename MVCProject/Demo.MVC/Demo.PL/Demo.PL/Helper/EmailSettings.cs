﻿using Demo.DAL.Entities;
using System.Net;
using System.Net.Mail;

namespace Demo.PL.Helper
{
    public class EmailSettings
    {
        public static void SendEmail(Email  email)
        {
            var client = new SmtpClient("smtp.ethereal.email", 587);

            client.EnableSsl = true;

            client.Credentials = new NetworkCredential("demarco.mueller@ethereal.email", "4VKGackfXUf2kMpjV6");

            client.Send("demarco.mueller@ethereal.email", email.To , email.Title, email.Body);
        }
    }
}
