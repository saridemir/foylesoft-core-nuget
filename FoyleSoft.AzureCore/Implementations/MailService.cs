using FoyleSoft.AzureCore.Interfaces;
using FoyleSoft.AzureCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FoyleSoft.AzureCore.Implementations
{
    public class MailService : IMailService
    {

        private readonly IAzureConfigurationService _azureConfigurationService;

        public MailService(IAzureConfigurationService azureConfigurationService)
        {
            _azureConfigurationService = azureConfigurationService;
        }
        public IBaseResponse<bool> SendEvent(string mailAddress)
        {
            try
            {
                var mailConfig = _azureConfigurationService.MailConfig;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                MailAddress from = new MailAddress(mailConfig.MailUserName,
                    mailConfig.MailUserName);
                MailAddress receiver = new MailAddress(mailAddress, mailAddress);

                var msg = new MailMessage(from, receiver);

                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+330)));
                str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.UtcNow));
                str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.AddMinutes(+660)));
                str.AppendLine("LOCATION: " + "abcd");
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", msg.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", msg.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", msg.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", msg.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", msg.To[0].DisplayName, msg.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                byte[] byteArray = Encoding.ASCII.GetBytes(str.ToString());
                MemoryStream stream = new MemoryStream(byteArray);

                Attachment attach = new Attachment(stream, "test.ics");

                msg.Attachments.Add(attach);

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                //  contype.Parameters.Add("name", "Meeting.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                msg.AlternateViews.Add(avCal);
                
                var smtpClient = new SmtpClient(mailConfig.MailServer, mailConfig.MailServerPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(mailConfig.MailUserName, mailConfig.MailPassword);
                smtpClient.EnableSsl = mailConfig.SupportSSL;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Send(msg);
                return new BaseResponse<bool> { Data = true, ErrorMessage = string.Empty, IsSuccess = true };
            }
            catch (Exception ex)
            {

                return new BaseResponse<bool> { Data = false, ErrorMessage = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), IsSuccess = false };
            }
        }
        public IBaseResponse<bool> SendMail(string mailAddress, string subject, string messageBody, bool isHtml)
        {
            try
            {
                var mailConfig = _azureConfigurationService.MailConfig;
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                MailAddress from = new MailAddress(mailConfig.MailUserName,
                    mailConfig.MailUserName);
                MailAddress receiver = new MailAddress(mailAddress, mailAddress);

                var msg = new MailMessage(from, receiver);
                msg.IsBodyHtml = isHtml;
                msg.Subject = subject;
                msg.Body = messageBody;

                var smtpClient = new SmtpClient(mailConfig.MailServer, mailConfig.MailServerPort);
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(mailConfig.MailUserName, mailConfig.MailPassword);
                smtpClient.EnableSsl = mailConfig.SupportSSL;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Send(msg);
                return new BaseResponse<bool> { Data = true, ErrorMessage = string.Empty, IsSuccess = true };
            }
            catch (Exception ex)
            {

                return new BaseResponse<bool> { Data = false, ErrorMessage = ex.Message + (ex.InnerException != null ? ex.InnerException.Message : ""), IsSuccess = false };
            }
        }
    }
}
