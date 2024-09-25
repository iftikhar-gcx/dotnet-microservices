
using Mango.Services.EmailAPI.Data;
using Mango.Services.EmailAPI.Message;
using Mango.Services.EmailAPI.Models;
using Microsoft.EntityFrameworkCore;
using RestSharp;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;

namespace Mango.Services.EmailAPI.Services
{
    public class EmailService : IEmailService
    {
        private DbContextOptions<AppDbContext> _dbContext;
        private readonly MailTrapSettings _mailSettings;       
        
        public EmailService(DbContextOptions<AppDbContext>? dbContext, MailTrapSettings mailSettings)
        {
            _dbContext = dbContext;
            _mailSettings = mailSettings;
            _mailSettings.ApiKey = Environment.GetEnvironmentVariable("MAILTRAP_API_KEY", EnvironmentVariableTarget.User) ?? string.Empty;
        }

        public async Task EmailCartAndLog(CartDTO cartDTO)
        {
            var messageBody = new StringBuilder();

            messageBody.AppendLine("<!DOCTYPE html>");
            messageBody.AppendLine("<html lang=\"en\">");
            messageBody.AppendLine("<head>");
            messageBody.AppendLine("    <meta charset=\"UTF-8\">");
            messageBody.AppendLine("    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            messageBody.AppendLine("    <title>Order Summary</title>");
            messageBody.AppendLine("    <style>");
            messageBody.AppendLine("        body { font-family: 'Arial', sans-serif; color: #f8f9fa; background-color: #343a40; margin: 0; padding: 0; }");
            messageBody.AppendLine("        .container { max-width: 600px; margin: 20px auto; background: #495057; border-radius: 8px; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2); overflow: hidden; }");
            messageBody.AppendLine("        .header { background: #212529; color: #ffffff; padding: 20px; text-align: center; border-bottom: 4px solid #28a745; }");
            messageBody.AppendLine("        .header h2 { margin: 0; font-size: 24px; font-weight: 600; }");
            messageBody.AppendLine("        .content { padding: 20px; }");
            messageBody.AppendLine("        table { width: 100%; border-collapse: collapse; margin-bottom: 20px; }");
            messageBody.AppendLine("        th, td { padding: 12px; text-align: left; border-bottom: 1px solid #6c757d; color: #e9ecef; }");
            messageBody.AppendLine("        th { background-color: #343a40; font-weight: 700; }");
            messageBody.AppendLine("        .total-section { margin-top: 20px; font-weight: 700; color: #ffc107; text-align: right; }");
            messageBody.AppendLine("        .total-section p { margin: 5px 0; padding: 5px 10px; }");
            messageBody.AppendLine("        .footer { text-align: center; padding: 15px; background: #495057; border-top: 1px solid #6c757d; font-size: 14px; color: #e9ecef; }");
            messageBody.AppendLine("    </style>");
            messageBody.AppendLine("</head>");
            messageBody.AppendLine("<body>");
            messageBody.AppendLine("    <div class=\"container\">");
            messageBody.AppendLine("        <div class=\"header\">");
            messageBody.AppendLine("            <h2>Order Summary</h2>");
            messageBody.AppendLine("        </div>");
            messageBody.AppendLine("        <div class=\"content\">");
            messageBody.AppendLine("            <table>");
            messageBody.AppendLine("                <thead>");
            messageBody.AppendLine("                    <tr><th>Item Name</th><th>Item Price</th><th>Quantity</th><th>Sub-Total</th></tr>");
            messageBody.AppendLine("                </thead>");
            messageBody.AppendLine("                <tbody>");

            decimal cartTotal = 0;

            if (cartDTO.CartDetails != null)
            {
                foreach (var item in cartDTO.CartDetails)
                {
                    if (item.Product != null)
                    {
                        var subTotal = item.Product.Price * item.Count;
                        messageBody.AppendLine($"                    <tr>");
                        messageBody.AppendLine($"                        <td>{HtmlEncoder.Default.Encode(item.Product.ProductName)}</td>");
                        messageBody.AppendLine($"                        <td>{string.Format("{0:c}", item.Product.Price)}</td>");
                        messageBody.AppendLine($"                        <td>{item.Count}</td>");
                        messageBody.AppendLine($"                        <td>{string.Format("{0:c}", subTotal)}</td>");
                        messageBody.AppendLine($"                    </tr>");

                        cartTotal += (decimal)subTotal;
                    }
                }
            }

            messageBody.AppendLine("                </tbody>");
            messageBody.AppendLine("            </table>");

            decimal taxRate = 0.1m; // Assuming 10% tax rate
            decimal taxAmount = taxRate * cartTotal;
            decimal totalWithTax = cartTotal + taxAmount;

            messageBody.AppendLine("            <div class=\"total-section\" style=\"font-weight: 700; color: #ffc107; margin-top: 20px; display: flex; flex-direction: column;\">");
            messageBody.AppendLine($"                <div style=\"display: flex; justify-content: space-between; margin: 5px 0;\"><span>Total before Tax:</span> <span>{string.Format("{0:c}", cartTotal)}</span></div>");
            messageBody.AppendLine($"                <div style=\"display: flex; justify-content: space-between; margin: 5px 0;\"><span>Tax Amount:</span> <span>{string.Format("{0:c}", taxAmount)}</span></div>");
            messageBody.AppendLine($"                <div style=\"display: flex; justify-content: space-between; margin: 5px 0;\"><span>Total with Tax:</span> <span>{string.Format("{0:c}", totalWithTax)}</span></div>");
            messageBody.AppendLine("            </div>");

            messageBody.AppendLine("        </div>");
            messageBody.AppendLine("        <div class=\"footer\">");
            messageBody.AppendLine("            Thank you for shopping with Mango! If you have any questions, feel free to contact us.");
            messageBody.AppendLine("        </div>");
            messageBody.AppendLine("    </div>");
            messageBody.AppendLine("</body>");
            messageBody.AppendLine("</html>");


            var subject = "Your Cart Details";
            await LogAndEmail(subject, messageBody.ToString(), cartDTO.CartHeader.Email);
        }

        private async Task<bool> LogAndEmail(string subject, string message, string email)
        {
            try
            {
                EmailLogger emailLogger = new EmailLogger()
                {
                    Email = email,
                    EmailSent = DateTime.Now,
                    Message = message
                };

                await using var _db = new AppDbContext(_dbContext);
                await _db.EmailLoggers.AddAsync(emailLogger);
                await _db.SaveChangesAsync();

                await SendEmailAsync(subject, message);

                return true;
            }
            catch (Exception) {}

            return false;
        }

        private async Task<bool> SendEmailAsync(string subject, string messageBody)
        {

            try
            {
                using (var client = new RestClient($"{_mailSettings.ApiBaseUrl.ToString()}"))
                {

                    var request = new RestRequest();
                    request.AddHeader("Authorization", $"Bearer {_mailSettings.ApiKey.ToString()}");
                    request.AddHeader("Content-Type", "application/json");

                    var requestBody = new
                    {
                        from = new
                        {
                            email = $"{_mailSettings.ApiSenderEmail.ToString()}",
                            name = "Mango - Support"
                        },
                        to = new[]
                        {
                        new
                        {
                            email = "miftikhar.gcx@outlook.com"
                        }
                    },
                        subject = subject.ToString(),
                        html = messageBody.ToString(),
                        category = "Integration Test"
                    };

                    request.AddJsonBody(requestBody);

                    var response = await client.PostAsync(request);

                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
                    {
                        return true;
                    }
                }
            }
            catch (Exception) { }


            return false;
        }

        public async Task RegisterUserEmailAndLog(string email)
        {
            var messageBody = new StringBuilder();

            messageBody.AppendLine(@"<!DOCTYPE html>");
            messageBody.AppendLine(@"<html lang=""en"">");
            messageBody.AppendLine(@"<head>");
            messageBody.AppendLine(@"    <meta charset=""UTF-8"">");
            messageBody.AppendLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">");
            messageBody.AppendLine(@"    <title>Registration Successful</title>");
            messageBody.AppendLine(@"    <style>");
            messageBody.AppendLine(@"        body {");
            messageBody.AppendLine(@"            font-family: Arial, sans-serif;");
            messageBody.AppendLine(@"            margin: 0;");
            messageBody.AppendLine(@"            padding: 0;");
            messageBody.AppendLine(@"            background-color: #f4f4f4;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .container {");
            messageBody.AppendLine(@"            width: 100%;");
            messageBody.AppendLine(@"            max-width: 600px;");
            messageBody.AppendLine(@"            margin: 0 auto;");
            messageBody.AppendLine(@"            background-color: #ffffff;");
            messageBody.AppendLine(@"            padding: 20px;");
            messageBody.AppendLine(@"            border-radius: 8px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-header {");
            messageBody.AppendLine(@"            background-color: #343a40;");
            messageBody.AppendLine(@"            color: #ffffff;");
            messageBody.AppendLine(@"            padding: 15px;");
            messageBody.AppendLine(@"            border-top-left-radius: 8px;");
            messageBody.AppendLine(@"            border-top-right-radius: 8px;");
            messageBody.AppendLine(@"            text-align: center;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-body {");
            messageBody.AppendLine(@"            padding: 20px;");
            messageBody.AppendLine(@"            text-align: center;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-title {");
            messageBody.AppendLine(@"            font-size: 24px;");
            messageBody.AppendLine(@"            margin-bottom: 10px;");
            messageBody.AppendLine(@"            color: #28a745;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-text {");
            messageBody.AppendLine(@"            font-size: 16px;");
            messageBody.AppendLine(@"            color: #555555;");
            messageBody.AppendLine(@"            margin-bottom: 20px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .btn {");
            messageBody.AppendLine(@"            display: inline-block;");
            messageBody.AppendLine(@"            padding: 10px 20px;");
            messageBody.AppendLine(@"            font-size: 16px;");
            messageBody.AppendLine(@"            color: #ffffff;");
            messageBody.AppendLine(@"            background-color: #007bff;");
            messageBody.AppendLine(@"            text-decoration: none;");
            messageBody.AppendLine(@"            border-radius: 4px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .btn:hover {");
            messageBody.AppendLine(@"            background-color: #0056b3;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"    </style>");
            messageBody.AppendLine(@"</head>");
            messageBody.AppendLine(@"<body>");
            messageBody.AppendLine(@"    <div class=""container"">");
            messageBody.AppendLine(@"        <div class=""card-header"">");
            messageBody.AppendLine(@"            <h3>Welcome to Mango!</h3>");
            messageBody.AppendLine(@"        </div>");
            messageBody.AppendLine(@"        <div class=""card-body"">");
            messageBody.AppendLine(@"            <h2 class=""card-title"">Registration Successful</h2>");
            messageBody.AppendLine(@"            <p class=""card-text"">Hello " + email + @"!,</p>");
            messageBody.AppendLine(@"            <p class=""card-text"">Congratulations! Your registration was successful. We’re thrilled to have you join our community.</p>");
            messageBody.AppendLine(@"            <ul style=""list-style-type: none; padding: 0; font-size: 16px; color: #555555;"">");
            messageBody.AppendLine(@"            </ul>");
            messageBody.AppendLine(@"            <p class=""card-text"">Thank you for joining Mango!</p>");
            messageBody.AppendLine(@"        </div>");
            messageBody.AppendLine(@"    </div>");
            messageBody.AppendLine(@"</body>");
            messageBody.AppendLine(@"</html>");

            string subject = "Registration Successful";
            await LogAndEmail(subject, messageBody.ToString(), _mailSettings.ApiSenderEmail);
        }

        public async Task LogOrderPlaced(RewardsMessage rewardsMessage)
        {
            StringBuilder messageBody = new StringBuilder();

            messageBody.AppendLine(@"<!DOCTYPE html>");
            messageBody.AppendLine(@"<html lang=""en"">");
            messageBody.AppendLine(@"<head>");
            messageBody.AppendLine(@"    <meta charset=""UTF-8"">");
            messageBody.AppendLine(@"    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">");
            messageBody.AppendLine(@"    <title>Order Success!</title>");
            messageBody.AppendLine(@"    <style>");
            messageBody.AppendLine(@"        body {");
            messageBody.AppendLine(@"            font-family: Arial, sans-serif;");
            messageBody.AppendLine(@"            margin: 0;");
            messageBody.AppendLine(@"            padding: 0;");
            messageBody.AppendLine(@"            background-color: #f4f4f4;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .container {");
            messageBody.AppendLine(@"            width: 100%;");
            messageBody.AppendLine(@"            max-width: 600px;");
            messageBody.AppendLine(@"            margin: 0 auto;");
            messageBody.AppendLine(@"            background-color: #ffffff;");
            messageBody.AppendLine(@"            padding: 20px;");
            messageBody.AppendLine(@"            border-radius: 8px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-header {");
            messageBody.AppendLine(@"            background-color: #343a40;");
            messageBody.AppendLine(@"            color: #ffffff;");
            messageBody.AppendLine(@"            padding: 15px;");
            messageBody.AppendLine(@"            border-top-left-radius: 8px;");
            messageBody.AppendLine(@"            border-top-right-radius: 8px;");
            messageBody.AppendLine(@"            text-align: center;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-body {");
            messageBody.AppendLine(@"            padding: 20px;");
            messageBody.AppendLine(@"            text-align: center;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-title {");
            messageBody.AppendLine(@"            font-size: 24px;");
            messageBody.AppendLine(@"            margin-bottom: 10px;");
            messageBody.AppendLine(@"            color: #28a745;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .card-text {");
            messageBody.AppendLine(@"            font-size: 16px;");
            messageBody.AppendLine(@"            color: #555555;");
            messageBody.AppendLine(@"            margin-bottom: 20px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .btn {");
            messageBody.AppendLine(@"            display: inline-block;");
            messageBody.AppendLine(@"            padding: 10px 20px;");
            messageBody.AppendLine(@"            font-size: 16px;");
            messageBody.AppendLine(@"            color: #ffffff;");
            messageBody.AppendLine(@"            background-color: #007bff;");
            messageBody.AppendLine(@"            text-decoration: none;");
            messageBody.AppendLine(@"            border-radius: 4px;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"        .btn:hover {");
            messageBody.AppendLine(@"            background-color: #0056b3;");
            messageBody.AppendLine(@"        }");
            messageBody.AppendLine(@"    </style>");
            messageBody.AppendLine(@"</head>");
            messageBody.AppendLine(@"<body>");
            messageBody.AppendLine(@"    <div class=""container"">");
            messageBody.AppendLine(@"        <div class=""card-header"">");
            messageBody.AppendLine(@"            <h3>Welcome to Mango!</h3>");
            messageBody.AppendLine(@"        </div>");
            messageBody.AppendLine(@"        <div class=""card-body"">");
            messageBody.AppendLine(@"            <h2 class=""card-title"">Order Successful</h2>");
            messageBody.AppendLine(@"            <p class=""card-text"">Hello " + rewardsMessage.UserEmail + @"!,</p>");
            messageBody.AppendLine(@"            <p class=""card-text"">Congratulations! Your have successfully placed your order. Your order id is: " + rewardsMessage.OrderId  + ".<br />Please wait while we make it fresh for you.</p>");
            messageBody.AppendLine(@"            <ul style=""list-style-type: none; padding: 0; font-size: 16px; color: #555555;"">");
            messageBody.AppendLine(@"            </ul>");
            messageBody.AppendLine(@"            <p class=""card-text"">Thank you for ordering with Mango!</p>");
            messageBody.AppendLine(@"        </div>");
            messageBody.AppendLine(@"    </div>");
            messageBody.AppendLine(@"</body>");
            messageBody.AppendLine(@"</html>");


            string subject = "Order Placed Successfully";
            await LogAndEmail(subject, messageBody.ToString(), _mailSettings.ApiSenderEmail);
        }
    }
}
