﻿using FoyleSoft.AzureCore.Interfaces;
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

                return new BaseResponse<bool> { Data = false, ErrorMessage = ex.Message + (ex.InnerException!=null?ex.InnerException.Message:""), IsSuccess = false };
            }
        }
    }
}